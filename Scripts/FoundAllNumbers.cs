using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundAllNumbers : MonoBehaviour
{
    [SerializeField] private GameObject[] numbers;

    public void DisableNumber(int number) {
        numbers[number - 1].SetActive(false);
    }

    public void EnableNumber(int number) {
        numbers[number - 1].SetActive(true);
    }

    public void Reset() {
        foreach (var num in numbers) {
            num.SetActive(true);
        }
    }
}
