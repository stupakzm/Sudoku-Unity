using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatisticItem {
    public float time;
    public List<int> board; // Changed from List<List<int>> to List<int>
    public int wrongPlacedNumbers;
    public int missedNumbers;

    public override string ToString() {
        string toReturn;
        if (wrongPlacedNumbers == 0) {
            toReturn = $"accurately solved in {time.ToString("#.0")} seconds.";
        }
        else {
            toReturn = $"solved in {time.ToString("#.0")} seconds, mistakes - {wrongPlacedNumbers}.";
        }
        return toReturn;
    }

    // Helper methods to convert between List<int> and int[,]
    public static List<int> ConvertArrayToList(int[,] array) {
        List<int> list = new List<int>();
        for (int i = 0; i < array.GetLength(0); i++) {
            for (int j = 0; j < array.GetLength(1); j++) {
                list.Add(array[i, j]);
            }
        }
        return list;
    }

    public static int[,] ConvertListToArray(List<int> list, int rows, int cols) {
        int[,] array = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                array[i, j] = list[i * cols + j];
            }
        }
        return array;
    }
}