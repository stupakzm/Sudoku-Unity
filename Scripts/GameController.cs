using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public event EventHandler<OnNumberChangedEventArgs> OnNumberChanged;
    public class OnNumberChangedEventArgs : EventArgs {
        public int number;
        public GridAction action;
    }
    //[SerializeField] private Transform numbersGridParent;
    [SerializeField] private readonly int gridCount = 9;
    [SerializeField] private MessageManager messageManager;
    [SerializeField] private Timer timer;
    [SerializeField] private SliderScript slider;
    [SerializeField] private TextMeshProUGUI currentNumber;
    [SerializeField] private GameObject numbers;
    [SerializeField] private GameObject actions;
    [SerializeField] private GameObject startNewGame;
    [SerializeField] private TextMeshProUGUI[] actionVisuals;

    private SudokuHandler sudokuHandler;
    private GridAction action;
    private GridGenerator gridGenerator;
    private int wrongPlacedNumbers;
    private int numberToChange;
    private int missingNumbers;
    private GridPosition[,] gridNumbers;
    private int[,] gridNumbersInt;
    private StatisticItem statisticItem;

    private void Start() {
        Statistics.LoadStatistics();
        missingNumbers = slider.LoadSliderValue();
        gridGenerator = GetComponent<GridGenerator>();
        sudokuHandler = GetComponent<SudokuHandler>();
        FixScaleOfGrid();
        gridNumbersInt = new int[gridCount, gridCount];
        if (!PlayerPrefs.HasKey("Attempts")){
            PlayerPrefs.SetInt("Attempts", 0);
            PlayerPrefs.Save();
        }
    }

    private void Update() {
        for (int i = 49; i <= 57; i++) {
            if (Input.GetKeyDown((KeyCode)i)) {
                numberToChange = i - 48;
                currentNumber.text = numberToChange.ToString();
                Debug.Log(i - 48);
            }
        }
        InputKeys();
    }

    private void InputKeys() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            UnhighlightAction();
            action = GridAction.SetMainNumber;
            HighlightAction();
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            UnhighlightAction();
            action = GridAction.ClearMainNumber;
            HighlightAction();
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            UnhighlightAction();
            action = GridAction.SetPossibleNumber;
            HighlightAction();
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            UnhighlightAction();
            action = GridAction.ClearPossibleNumber;
            HighlightAction();
        }
        else if (Input.GetKeyDown(KeyCode.T)) {
            UnhighlightAction();
            action = GridAction.ClearAllPossibleNumbers;
            HighlightAction();
        }
        else if (Input.GetKeyDown(KeyCode.H)) {
            UpdateGridNumbers();
            try {
                Hint hint = sudokuHandler.GetHint(gridNumbersInt);
                gridNumbers[hint.row, hint.col].SetNumberByHint(hint);
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
    }

    private void FixScaleOfGrid() {
        float screenHeight = Screen.height;
        float newScale = screenHeight / 1080;
        gameObject.transform.localScale = new Vector3(newScale, newScale, 1);
    }

    public void StartSudoku() {
        FixScaleOfGrid();
        statisticItem = new StatisticItem();
        gridNumbers = gridGenerator.InstantiateGridNumbers(gridCount, this.transform);
        int[,] startBoard = sudokuHandler.RemoveNumbers(sudokuHandler.GenerateSudoku(), missingNumbers);
        statisticItem.missedNumbers = missingNumbers;
        SetNumbersOnBoard(startBoard);
       // gridNumbersSafe = sudokuHandler.AsignSafePositionsBoard(startBoard);
        messageManager.ShowMessage($"Game started!\n{missingNumbers} missing numbers");
        numberToChange = 1;
        currentNumber.text = numberToChange.ToString();
        action = GridAction.SetMainNumber;
        HighlightAction();
        CheckIsFoundAllNumbersVisual();
        PlayerPrefs.SetInt("Attempts", PlayerPrefs.GetInt("Attempts")+1);
        PlayerPrefs.Save();
    }

    private void SetNumbersOnBoard(int[,] numbers) {
        for (int i = 0; i < gridCount; i++) {
            for (int j = 0; j < gridCount; j++) {
                if (numbers[i, j] != 0)
                    gridNumbers[i, j].SetStartNumber(numbers[i, j]);
            }
        }
    }

    private void UpdateGridNumbers() {
        for (int i = 0; i < gridCount; i++) {
            for (int j = 0; j < gridCount; j++) {
                gridNumbersInt[i, j] = gridNumbers[i, j].GetMainNumber();
            }
        }
    }

    public bool CheckIsFoundAllNumbersHelper(int num) {
        int counter = 0;
        for (int i = 0; i < gridCount; i++) {
            for (int j = 0; j < gridCount; j++) {
                if (gridNumbers[i, j].rowColNum.number == num) {
                    if (gridNumbers[i, j].IsCorrect) {
                        counter++;
                        if (counter == 9) return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        }
        return false;
    }

    public void CheckIsFoundAllNumbersVisual() {
        UpdateGridNumbers();
        var numbersVisual = numbers.GetComponent<FoundAllNumbers>();
        for (int i = 1; i <= gridCount; i++) {
            if (CheckIsFoundAllNumbersHelper(i)) {
                numbersVisual.DisableNumber(i);
            }
            else {
                numbersVisual.EnableNumber(i);
            }
        }
        ChangeNumberFoundAll();
    }

    public bool IsPositionInsertedGood(Hint position) {
        UpdateGridNumbers();
        if (sudokuHandler.IsSafePlaced(gridNumbersInt, position.row, position.col, position.number)){
            return true;
        }
        WrongPlacedNumber();
        return false;
    }

    public void ShowMessage(string text) {
        messageManager.ShowMessage(text);
    }

    public void IsWonSudoku() {
        if (IsWonSudokuHelper()) {
            SaveSolvedSudoku();
            messageManager.ShowMessage($"Congrats, solved in {timer.GetTime().ToString("#.0")}!", 10);
            numbers.SetActive(false);
            actions.SetActive(false);
            startNewGame.SetActive(true);
        }
    }

    private bool IsWonSudokuHelper() {
        for (int i = 0; i < gridNumbers.GetLength(0); i++) {
            for (int j = 0; j < gridNumbers.GetLength(1); j++) {
                if (gridNumbers[i, j].GetMainNumber() == 0) return false;
                if (gridNumbers[i, j].IsCorrect == false) return false;
            }
        }
        return true;
    }

    private void SaveSolvedSudoku() {
        statisticItem.board = StatisticItem.ConvertArrayToList(gridNumbersInt);
        statisticItem.wrongPlacedNumbers = wrongPlacedNumbers;
        statisticItem.time = timer.StopTimer();
        Statistics.AddItem(statisticItem);
    }

    private void WrongPlacedNumber() {
        wrongPlacedNumbers++;
    }

    private void ClearBoard() {
        for (int i = 0; i < gridNumbers.GetLength(0); i++) {
            for (int j = 0; j < gridNumbers.GetLength(1); j++) {
                Destroy(gridNumbers[i, j].gameObject);
            }
        }
    }

    private void ChangeNumberFoundAll() {
        for (int i = 1; i <= 9; i++) {
            if (!CheckIsFoundAllNumbersHelper(numberToChange)) return;
            if (i == numberToChange) continue;
            else if (CheckIsFoundAllNumbersHelper(i)) continue;
            else numberToChange = i;
            currentNumber.text = numberToChange.ToString();
            return;
        }
        numberToChange = 0;
    }

    private void HighlightAction() {
        actionVisuals[(int)action].fontStyle = FontStyles.Bold;
    }

    private void UnhighlightAction() {
        actionVisuals[(int)action].fontStyle = FontStyles.Normal;
    }

    public void OnButtonGridPosition() {
        OnNumberChanged?.Invoke(this, new OnNumberChangedEventArgs { number = numberToChange, action = this.action });
    }

    public void OnButtonChangeAction(int a) {
        UnhighlightAction();
        action = (GridAction)a;
        HighlightAction();
    }

    public void OnButtonChangeNumber(int n) {
        if (CheckIsFoundAllNumbersHelper(n)) return;
        numberToChange = n;
        currentNumber.text = numberToChange.ToString();
    }

    public void OnButtonStartGame() {
        timer.StartTimer();
        numbers.SetActive(true);
        actions.SetActive(true);
    }

    public void OnButtonStartNewGame() {
        ClearBoard();
        timer.StartTimer();
        numbers.SetActive(true);
        actions.SetActive(true);
    }

    public void OnButtonResetGame() {
        ClearBoard();
        timer.PauseTimer();
        timer.ResetTimer();
        wrongPlacedNumbers = 0;
        numbers.SetActive(false);
        actions.SetActive(false);
    }

    public void OnMissingNumbersChanged(float value) {
        missingNumbers = (int)value;
        PlayerPrefs.SetInt("MissingNumbers", missingNumbers);
        PlayerPrefs.Save();
    }
}
