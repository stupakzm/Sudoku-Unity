using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridPosition : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI[] possibleNumbers;
    [SerializeField] private TextMeshProUGUI mainNumber;

    private Image image;
    private GameController gameController;
    private bool startNumberAsigned;
    private bool isSafe = false;
    private bool isCorrect = false;

    public Hint rowColNum;

    public bool IsCorrect { get { return isCorrect; } }// need for cheching is board solved
    public bool IsSafe { get { return isSafe; } }//if setted from start(generation)

    private void Start() {
        startNumberAsigned = false;
        gameController = GetComponentInParent<GameController>();
        image = GetComponent<Image>();
        gameObject.GetComponent<Button>().onClick.AddListener(() => SubscribeEventOnNumberChanged());
        gameObject.GetComponent<Button>().onClick.AddListener(() => gameController.OnButtonGridPosition());
    }

    private void SetPossibleNumber(int number) {
        if (number < 10 && number > 0) {
            possibleNumbers[number - 1].gameObject.SetActive(true);
        }
    }

    private void ClearPossibleNumbers() {
        foreach (var possibleNumber in possibleNumbers) {
            possibleNumber.gameObject.SetActive(false);
        }
    }

    private void ClearPossibleNumber(int number) {
        if (number < 10 && number > 0) {
            possibleNumbers[number - 1].gameObject.SetActive(false);
        }
    }

    private void ShowPossibleNumbers() {
        foreach (var possibleNumber in possibleNumbers) {
            possibleNumber.gameObject.SetActive(true);
        }
    }

    private void ClearMainNumber() {
        mainNumber.text = "";
        UnHighlightPosition();
    }

    private void SetMainNumber(int number) {
        rowColNum.number = number;
        if (number < 10 && number > 0) {
            mainNumber.text = number.ToString();
        }
    }

    public void SetStartNumber(int number) {
        if (!startNumberAsigned) {
            SetMainNumber(number);
            isSafe = true;
            isCorrect = true;
        }
        startNumberAsigned = true;
    }

    public void SetNumberByHint(Hint hint) {
        SetMainNumber(hint.number);
        isCorrect = true;
    }

    public int GetMainNumber() {
        return mainNumber.text == "" ? 0 : int.Parse(mainNumber.text);
    }

    public void HighlightPosition() {
        image.color = Color.gray;
    }

    public void HighlightErrorPosition() {
        image.color = Color.red;
    }

    public void UnHighlightPosition() {
        image.color = Color.white;
    }

    public void SubscribeEventOnNumberChanged() {
        gameController.OnNumberChanged += GameController_OnNumberChanged;
    }

    private void GameController_OnNumberChanged(object sender, GameController.OnNumberChangedEventArgs e) {
        switch (e.action) {
            case GridAction.ClearMainNumber:
                if (!isSafe) {
                    ClearMainNumber();
                    isCorrect = false;
                    gameController.CheckIsFoundAllNumbersVisual();
                }
                else {
                    gameController.ShowMessage($"Number {mainNumber.text} is safe");
                }
                break;
            case GridAction.SetMainNumber:
                if (isSafe) {
                    gameController.ShowMessage("Possition is safe");
                    break;
                }
                SetMainNumber(e.number);
                if (gameController.IsPositionInsertedGood(rowColNum)) {
                    UnHighlightPosition();
                    isCorrect = true;
                    gameController.CheckIsFoundAllNumbersVisual();
                }
                else {
                    HighlightErrorPosition();
                    gameController.ShowMessage($"Number {e.number} inserted incorrectly!");
                    isCorrect = false;
                }
                gameController.IsWonSudoku();
                ClearPossibleNumbers();
                break;
            case GridAction.SetPossibleNumber:
                if(mainNumber.text == "")
                    SetPossibleNumber(e.number);
                break;
            case GridAction.ClearPossibleNumber:
                ClearPossibleNumber(e.number);
                break;
            case GridAction.ClearAllPossibleNumbers:
                ClearPossibleNumbers();
                break;
            default:
                break;
        }
        UnsubscribeEventOnNumberChanged();
    }

    public void UnsubscribeEventOnNumberChanged() {
        gameController.OnNumberChanged -= GameController_OnNumberChanged;
    }
}
