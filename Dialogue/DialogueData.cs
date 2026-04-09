using UnityEngine;

/// <summary>
/// 对话中发放道具（依赖 PackageManager 与配置表 PackageTable）。
/// </summary>
[System.Serializable]
public class DialogueItemGrant
{
    public int itemId;
    [Min(1)]
    public int count = 1;
}

/// <summary>
/// 一条选项：显示文案与跳转节点下标（-1 表示结束对话）。
/// </summary>
[System.Serializable]
public class DialogueChoice
{
    public string text;
    [Tooltip("-1 表示结束对话")]
    public int nextNodeIndex = -1;

    [Tooltip("选择此项时发放道具")]
    public DialogueItemGrant[] grantsOnPick;
}

/// <summary>
/// 单句对话节点：可纯线性（无选项），或带分支选项。
/// </summary>
[System.Serializable]
public class DialogueNode
{
    public string speakerName;
    [TextArea(2, 8)]
    public string text;

    [Tooltip("非空时显示选项按钮；为空时用「继续」走 nextWhenNoChoice")]
    public DialogueChoice[] choices;

    /// <summary>-2：下一句为当前下标+1；-1：无选项时结束对话</summary>
    public int nextWhenNoChoice = DialogueNodeConstants.SequentialNext;

    [Tooltip("进入该句对话时发放道具")]
    public DialogueItemGrant[] grantsOnEnter;
}

public static class DialogueNodeConstants
{
    public const int End = -1;
    public const int SequentialNext = -2;
}

[CreateAssetMenu(menuName = "Dialogue/对话数据", fileName = "NewDialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueNode[] nodes;
}
