using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UISettings : SingleMonoBase<UISettings>
{
    private Transform _uiRoot;
    // 路径配置字典
    private Dictionary<string, string> pathDict;
    // 预制件缓存字典
    private Dictionary<string, GameObject> prefabDict;
    // 已打开界面的缓存字典
    public Dictionary<string, BasePanel> panelDict;

    protected override void Awake()
    {
        InitDicts();
        base.Awake();
    }

    public Transform UIRoot
    {
        get
        {
            if (_uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    _uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    _uiRoot = new GameObject("Canvas").transform;
                }
            };
            return _uiRoot;
        }
    }

    private void InitDicts()
    {
        if (pathDict != null)
            return;

        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();

        pathDict = new Dictionary<string, string>()
        {
            {UIConst.PackagePanel, "Package/PackagePanel"},
            {UIConst.LotteryPanel, "Lottery/LotteryPanel"},
            {UIConst.MainPanel, "MainPanel"},
            {UIConst.DialoguePanel, "Dialogue/DialoguePanel"},
        };
    }

    public BasePanel GetPanel(string name)
    {
        if (pathDict == null)
            InitDicts();

        BasePanel panel = null;
        // 检查是否已打开
        if (panelDict.TryGetValue(name, out panel))
        {
            return panel;
        }
        return null;
    }

    public BasePanel OpenPanel(string name)
    {
        if (pathDict == null)
            InitDicts();

        BasePanel panel = null;
        // 检查是否已打开
        if (panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面已打开: " + name);
            return null;
        }

        // 检查路径是否配置
        string path = "";
        if (!pathDict.TryGetValue(name, out path))
        {
            Debug.Log("界面名称错误，或未配置路径: " + name);
            return null;
        }

        // 使用缓存预制件
        GameObject panelPrefab = null;
        if (!prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = "Prefab/Panel/" + path;

            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            if (panelPrefab != null)
                prefabDict.Add(name, panelPrefab);
        }

        if (panelPrefab == null)
        {
            Debug.LogError("未找到界面预制体: Resources/" + path);
            return null;
        }

        // 打开界面
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObject.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        panel.OpenPanel(name);
        return panel;
    }

    public bool ClosePanel(string name)
    {
        if (pathDict == null)
            InitDicts();

        BasePanel panel = null;
        if (!panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面未打开: " + name);
            return false;
        }

        panel.ClosePanel();
        // panelDict.Remove(name);
        return true;
    }

}

public class UIConst
{
    // menu panels

    public const string PackagePanel = "PackagePanel";
    public const string LotteryPanel = "LotteryPanel";
    public const string MainPanel = "MainPanel";
    public const string DialoguePanel = "DialoguePanel";
}