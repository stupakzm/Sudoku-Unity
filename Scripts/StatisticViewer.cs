using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticViewer : MonoBehaviour {
    //6 elements in screen
    //each element is one StatictisItem
    //on bottom arrows to show next/previous items.

    [SerializeField] private GameObject[] item;
    [SerializeField] private TextMeshProUGUI[] text;
    [SerializeField] private TextMeshProUGUI textItem;
    [SerializeField] private TextMeshProUGUI[] textMissedNum;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private TextMeshProUGUI additionalStats;
    [SerializeField] private GameObject boardView;
    [SerializeField] private Transform boardParent;
    [SerializeField] private GridPositionSimple[] gridPositionsSimple;

    private int tempIndex;
    List<StatisticItem> list;

    private void ActivateItems() {
        UpdateAdditionalStats();

        int endPoint = Mathf.Max(0, Statistics.ListCount - ((tempIndex + 1) * 6));
        Debug.Log($"endPoint - {endPoint}");

        if (Statistics.ListCount > 6) {
            for (int i = 0; i < 6; i++) {
                int listIndex = Statistics.ListCount - 1 - (tempIndex * 6) - i;
                Debug.Log($"listIndex - {listIndex}");
                if (listIndex >= endPoint) {
                    item[i].SetActive(true);
                    AsingItem(listIndex, i);
                }
                else {
                    item[i].SetActive(false);
                }
            }
        }

        previousButton.SetActive(tempIndex != 0);
        nextButton.SetActive(Statistics.ListCount - (tempIndex * 6) > 1);
    }

    private void UpdateAdditionalStats() {
        int attempts = PlayerPrefs.GetInt("Attempts");
        //Debug.Log($"Statistics.ListCount - {Statistics.ListCount}");
        additionalStats.text = $"Attempts - {attempts}, Winrate - {((float)Statistics.ListCount / attempts * 100).ToString("#.0")}%, Average time - {Statistics.GetAverageTime().ToString("#.0")} seconds.";
    }

    private void AsingItem(int listIndex, int itemIndex) {
        text[itemIndex].text = list[listIndex].ToString();
        textMissedNum[itemIndex].text = list[listIndex].missedNumbers.ToString();
    }

    public void OnButtonNext() {
        tempIndex++;
        ActivateItems();
    }

    public void OnButtonPrevious() {
        tempIndex--;
        ActivateItems();
    }

    public void OnButtonShowStats() {
        tempIndex = 0;
        list = Statistics.GetList();
        ActivateItems();
        previousButton.SetActive(false);
    }

    public void OnButtonShowBoardItem(int itemIndex) {
        int index = Statistics.ListCount - itemIndex - (tempIndex * 6);

        boardView.SetActive(true);

        StatisticItem item = list[index];

        textItem.text = item.ToString();

        int j = 0;
        int k = 0;
        for (int i = 0; i < item.board.Count; i++) {
            if (i % 9 == 0 && i != 0) {
                j = 0;
                k++;
            }
            gridPositionsSimple[i].SetMainNumber(item.board[i]);
            j++;
        }
    }
}
