using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Statistics {
    private static List<StatisticItem> list = new List<StatisticItem>();

    public static int ListCount { get { return list.Count; } }

    public static void AddItem(StatisticItem item) {
        list.Add(item);
        SaveStatistics();
    }

    public static List<StatisticItem> GetList() {
        return list;
    }

    public static StatisticItem GetItemAt(int index) {
        if(index < 0 || index >= list.Count) throw new System.ArgumentOutOfRangeException("given index must be more than 0 and less than list count");
        return list[index];
    }

    public static float GetAverageTime() {
        float result = 0;
        foreach (StatisticItem item in list) {
            result += item.time;
        }

        return result/ListCount;
    }

    public static void SaveStatistics() {
        string json = JsonUtility.ToJson(new StatisticItemListWrapper { items = list });
        PlayerPrefs.SetString("Statistics", json);
        PlayerPrefs.Save();
    }

    public static void LoadStatistics() {
        if (PlayerPrefs.HasKey("Statistics")) {
            string json = PlayerPrefs.GetString("Statistics");
            StatisticItemListWrapper wrapper = JsonUtility.FromJson<StatisticItemListWrapper>(json);
            list = wrapper.items ?? new List<StatisticItem>();
        }
    }

    [System.Serializable]
    private class StatisticItemListWrapper {
        public List<StatisticItem> items;
    }
}