using System;
using UnityEngine;

/// <summary>
/// 对话流程：可选通过 UISettings 打开 DialoguePanel；支持节点/选项发放道具。
/// </summary>
public class DialogueSystem : SingleMonoBase<DialogueSystem>
{
    [SerializeField] private bool unlockCursorWhileOpen = true;
    [Tooltip("开启时从 Resources 打开 DialoguePanel，结束自动关闭")]
    [SerializeField] private bool useUISettingsPanel = true;

    public bool IsPlaying { get; private set; }

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    /// <summary>speaker, body, choices（null 或 Length==0 表示用继续键）</summary>
    public event Action<string, string, DialogueChoice[]> OnNodeShown;

    private DialogueData _data;
    private int _index;
    private CursorLockMode _savedLockMode;
    private bool _savedCursorVisible;
    private bool _openedDialoguePanel;

    public void StartDialogue(DialogueData data, int startIndex = 0)
    {
        if (IsPlaying)
            EndDialogue();

        if (data == null || data.nodes == null || data.nodes.Length == 0)
        {
            Debug.LogWarning("DialogueSystem: 对话数据为空");
            return;
        }

        if (startIndex < 0 || startIndex >= data.nodes.Length)
        {
            Debug.LogWarning("DialogueSystem: startIndex 越界");
            return;
        }

        if (!TryEnsureDialoguePanel())
            return;

        _data = data;
        _index = startIndex;
        IsPlaying = true;

        if (unlockCursorWhileOpen)
        {
            _savedLockMode = Cursor.lockState;
            _savedCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        OnDialogueStarted?.Invoke();
        ShowCurrentNode();
    }

    private bool TryEnsureDialoguePanel()
    {
        if (!useUISettingsPanel || UISettings.Instance == null)
        {
            _openedDialoguePanel = false;
            return true;
        }

        if (UISettings.Instance.GetPanel(UIConst.DialoguePanel) != null)
            UISettings.Instance.ClosePanel(UIConst.DialoguePanel);

        BasePanel panel = UISettings.Instance.OpenPanel(UIConst.DialoguePanel);
        if (panel == null)
        {
            Debug.LogWarning("DialogueSystem: 无法打开对话面板，请确认 Resources/Prefab/Panel/Dialogue/DialoguePanel 预制体存在且含 DialoguePanel 组件");
            return false;
        }

        _openedDialoguePanel = true;
        return true;
    }

    public void TryAdvance()
    {
        if (!IsPlaying || _data == null)
            return;

        var node = _data.nodes[_index];
        if (node.choices != null && node.choices.Length > 0)
            return;

        int next = ResolveNextIndex(node, _index);
        GoToIndex(next);
    }

    public void PickChoice(int choiceIndex)
    {
        if (!IsPlaying || _data == null)
            return;

        var node = _data.nodes[_index];
        if (node.choices == null || choiceIndex < 0 || choiceIndex >= node.choices.Length)
            return;

        var choice = node.choices[choiceIndex];
        ApplyGrants(choice.grantsOnPick);
        GoToIndex(choice.nextNodeIndex);
    }

    public void EndDialogue()
    {
        if (!IsPlaying)
            return;

        IsPlaying = false;
        _data = null;
        _index = 0;

        if (unlockCursorWhileOpen)
        {
            Cursor.lockState = _savedLockMode;
            Cursor.visible = _savedCursorVisible;
        }

        OnDialogueEnded?.Invoke();

        if (_openedDialoguePanel && UISettings.Instance != null)
            UISettings.Instance.ClosePanel(UIConst.DialoguePanel);

        _openedDialoguePanel = false;
    }

    private void ShowCurrentNode()
    {
        var node = _data.nodes[_index];
        OnNodeShown?.Invoke(node.speakerName, node.text, node.choices);
        ApplyGrants(node.grantsOnEnter);
    }

    private void GoToIndex(int next)
    {
        if (next == DialogueNodeConstants.End || next < 0 || next >= _data.nodes.Length)
        {
            EndDialogue();
            return;
        }

        _index = next;
        ShowCurrentNode();
    }

    private static int ResolveNextIndex(DialogueNode node, int currentIndex)
    {
        if (node.nextWhenNoChoice == DialogueNodeConstants.SequentialNext)
            return currentIndex + 1;

        return node.nextWhenNoChoice;
    }

    private static void ApplyGrants(DialogueItemGrant[] grants)
    {
        if (grants == null || grants.Length == 0)
            return;

        var pm = PackageManager.Instance;
        if (pm == null)
        {
            Debug.LogWarning("DialogueSystem: 场景中需要 PackageManager 才能发放对话道具");
            return;
        }

        foreach (var g in grants)
        {
            if (g.count <= 0)
                continue;
            pm.TryAddItem(g.itemId, g.count);
        }
    }
}
