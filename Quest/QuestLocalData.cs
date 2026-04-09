using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestRuntimeProgress
{
    public string questId;
    public int currentCount;
    public bool completed;
}

[System.Serializable]
public class QuestSaveData
{
    public QuestRuntimeProgress[] items;
}

/// <summary>
/// 任务进度存档（PlayerPrefs）。
/// </summary>
public class QuestLocalData
{
    private const string PrefsKey = "QuestLocalData_v1";

    private static QuestLocalData _instance;

    public static QuestLocalData Instance
    {
        get
        {
            if (_instance == null)
                _instance = new QuestLocalData();
            return _instance;
        }
    }

    private Dictionary<string, QuestRuntimeProgress> _map;

    private Dictionary<string, QuestRuntimeProgress> Map
    {
        get
        {
            if (_map == null)
                Load();
            return _map;
        }
    }

    public QuestRuntimeProgress GetOrCreate(string questId)
    {
        if (!Map.TryGetValue(questId, out var p))
        {
            p = new QuestRuntimeProgress { questId = questId, currentCount = 0, completed = false };
            Map[questId] = p;
        }

        return p;
    }

    public bool IsCompleted(string questId)
    {
        return Map.TryGetValue(questId, out var p) && p.completed;
    }

    public void Save()
    {
        var list = new List<QuestRuntimeProgress>(Map.Values);
        var blob = new QuestSaveData { items = list.ToArray() };
        string json = JsonUtility.ToJson(blob);
        PlayerPrefs.SetString(PrefsKey, json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        _map = new Dictionary<string, QuestRuntimeProgress>(StringComparer.Ordinal);
        if (!PlayerPrefs.HasKey(PrefsKey))
            return;

        string json = PlayerPrefs.GetString(PrefsKey);
        var blob = JsonUtility.FromJson<QuestSaveData>(json);
        if (blob?.items == null)
            return;

        foreach (var e in blob.items)
        {
            if (e != null && !string.IsNullOrEmpty(e.questId))
                _map[e.questId] = e;
        }
    }

    public void ResetAll()
    {
        _map = new Dictionary<string, QuestRuntimeProgress>(StringComparer.Ordinal);
        PlayerPrefs.DeleteKey(PrefsKey);
        PlayerPrefs.Save();
    }
}
