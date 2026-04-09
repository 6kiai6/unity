using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 对话界面：继承 BasePanel，由 UISettings 从 Resources 打开/关闭。
/// </summary>
public class DialoguePanel : BasePanel
{
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private Text speakerText;
    [SerializeField] private Text bodyText;
    [SerializeField] private Button continueButton;
    [SerializeField] private RectTransform choiceContainer;
    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private bool advanceWithSpaceOrEnter = true;

    private readonly List<Button> _choicePool = new List<Button>();
    private DialogueSystem _system;
    private bool _waitingForChoice;

    protected override void Awake()
    {
        base.Awake();
        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnEnable()
    {
        BindSystem();
    }

    private void Start()
    {
        BindSystem();
    }

    private void OnDisable()
    {
        UnbindSystem();
    }

    private void BindSystem()
    {
        UnbindSystem();
        _system = DialogueSystem.Instance;
        if (_system == null)
            return;

        _system.OnDialogueStarted += HandleStarted;
        _system.OnDialogueEnded += HandleEnded;
        _system.OnNodeShown += HandleNodeShown;
    }

    private void UnbindSystem()
    {
        if (_system == null)
            return;

        _system.OnDialogueStarted -= HandleStarted;
        _system.OnDialogueEnded -= HandleEnded;
        _system.OnNodeShown -= HandleNodeShown;
        _system = null;
    }

    private void Update()
    {
        if (!advanceWithSpaceOrEnter || _system == null || !_system.IsPlaying || _waitingForChoice)
            return;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb == null)
            return;
        if (kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame)
            _system.TryAdvance();
#endif
    }

    public override void OpenPanel(string panelName)
    {
        base.OpenPanel(panelName);
        if (rootPanel != null)
            rootPanel.SetActive(true);
    }

    private void HandleStarted()
    {
        if (rootPanel != null)
            rootPanel.SetActive(true);
    }

    private void HandleEnded()
    {
        ClearChoices();
        if (rootPanel != null)
            rootPanel.SetActive(false);
        _waitingForChoice = false;
    }

    private void HandleNodeShown(string speaker, string body, DialogueChoice[] choices)
    {
        if (speakerText != null)
            speakerText.text = speaker ?? string.Empty;
        if (bodyText != null)
            bodyText.text = body ?? string.Empty;

        bool hasChoices = choices != null && choices.Length > 0;
        _waitingForChoice = hasChoices;

        if (continueButton != null)
            continueButton.gameObject.SetActive(!hasChoices);

        if (hasChoices)
            BuildChoices(choices);
        else
            ClearChoices();
    }

    private void OnContinueClicked()
    {
        if (_system != null)
            _system.TryAdvance();
    }

    private void BuildChoices(DialogueChoice[] choices)
    {
        ClearChoices();
        if (_system == null || choiceContainer == null || choiceButtonPrefab == null || choices == null)
            return;

        for (int i = 0; i < choices.Length; i++)
        {
            int captured = i;
            var btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.gameObject.SetActive(true);
            _choicePool.Add(btn);

            var label = btn.GetComponentInChildren<Text>();
            if (label != null)
                label.text = choices[i].text;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (_system != null)
                    _system.PickChoice(captured);
            });
        }
    }

    private void ClearChoices()
    {
        foreach (var b in _choicePool)
        {
            if (b != null)
                Destroy(b.gameObject);
        }
        _choicePool.Clear();
    }
}
