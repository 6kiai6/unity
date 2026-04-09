using UnityEngine;

/// <summary>
/// 进入碰撞体后按 Interact 开始对话（需场景中存在 DialogueSystem）。
/// </summary>
[RequireComponent(typeof(Collider))]
public class DialogueInteractable : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private int startNodeIndex;
    [SerializeField] private string playerTag = "Player";

    private bool _playerInside;
    private MyInputSystem _input;

    private void Awake()
    {
        _input = new MyInputSystem();
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void OnDestroy()
    {
        _input?.Dispose();
    }

    private void Update()
    {
        if (!_playerInside || dialogue == null || DialogueSystem.Instance == null)
            return;

        if (_input.Player.Interact.WasPressedThisFrame() && !DialogueSystem.Instance.IsPlaying)
            DialogueSystem.Instance.StartDialogue(dialogue, startNodeIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            _playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            _playerInside = false;
    }
}
