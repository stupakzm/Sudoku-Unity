using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridPositionSimple : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mainNumber;

    public void SetMainNumber(int number) {
        if (number < 10 && number > 0) {
            mainNumber.text = number.ToString();
        }
    }
}
