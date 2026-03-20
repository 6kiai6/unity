using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;
using System.Diagnostics;

public enum PlayerState{
    Idle,
    Move,
    Hover,
    Aming
}


public class PlayerModel : MonoBehaviour, IstateMachineOwner
{
    public PlayerWeapon weapon;
    private StateMachine stateMachine;
    [HideInInspector]
    public PlayerState currentState;
    [HideInInspector]
    public CharacterController cc;

    #region 约束相关
    public TwoBoneIKConstraint rightHandConstraint;
    public MultiAimConstraint rightHandAimConstraint;
    public MultiAimConstraint bodyAimConstraint;
    #endregion

    #region 垂直速度
    [Tooltip("重力")]
    public float gravity = -9.8f;
    [Tooltip("跳跃高度")]
    public float jumpHeight = 1.5f;
    [Tooltip("悬空高度判定")]
    public float fallHeight = 0.3f;

    private const int cacheSize = 3;
    private Vector3[] VelocityCache = new Vector3[cacheSize];
    private int cacheIndex = 0;
    private Vector3 averageVelocity = Vector3.zero;

    [HideInInspector]
    public float verticalSpeed;

    #endregion

    #region 导航相关
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    public float stoppingDistance = 2f;
    #endregion




    [HideInInspector]
    public Animator animator;

    private void Awake(){
        stateMachine = new StateMachine(this);
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.angularSpeed = PlayerController.Instance.rotationSpeed;
    }


    void Start()
    {
        SwitchState(PlayerState.Idle);
        ExitAim();
    }

    public void SwitchState(PlayerState state){
        switch(state){
            case PlayerState.Idle:
                stateMachine.EnterState<PlayerIdelState>();
                break;
            case PlayerState.Move:
                stateMachine.EnterState<PlayerMoveState>();
                break;
            case PlayerState.Hover:
                stateMachine.EnterState<PlayerHoverState>();
                break;
            case PlayerState.Aming:
                stateMachine.EnterState<PlayerAmingState>();
                break;
        }
        currentState = state;
    }


    /// <summary>
    /// 播放状态动画
    /// </summary>
    /// <param name="AnimationName">动画名称</param>
    /// <param name="transition">过渡时间</param>
    /// <param name="layer">层级</param>
    public void PlayStateAnimation(string AnimationName, float transition = 0.2f, int layer = 0){
        animator.CrossFadeInFixedTime(AnimationName, transition, layer);
    }

    private void OnAnimatorMove(){
        Vector3 PlayerDeltaMovement = animator.deltaPosition;
        if(currentState != PlayerState.Hover){
            UpdateVelocityCache(animator.velocity);
        }
        else{
            PlayerDeltaMovement = averageVelocity * Time.deltaTime;
        }
        
        PlayerDeltaMovement.y = verticalSpeed * Time.deltaTime;
        cc.Move(PlayerDeltaMovement);
    }

    public bool IsHover(){
        return !Physics.Raycast(transform.position, Vector3.down, fallHeight);
    }

    /// <summary> 检测脚下是否在很近的范围内有地面（用于落地判定） </summary>
    public bool IsGroundNear(float distance = 0.1f){
        return Physics.Raycast(transform.position, Vector3.down, distance);
    }
    public void UpdateVelocityCache(Vector3 velocity){
        VelocityCache[cacheIndex++] = velocity;
        cacheIndex %= cacheSize;
        Vector3 sum = Vector3.zero;
        foreach(Vector3 v in VelocityCache){
            sum += v;
        }
        averageVelocity = sum / cacheSize;
    }

    public void EnterAim(){
        rightHandConstraint.weight = 0;
        rightHandAimConstraint.weight = 1;
        bodyAimConstraint.weight = 1;
    }

    public void ExitAim(){
        rightHandConstraint.weight = 1;
        rightHandAimConstraint.weight = 0;
        bodyAimConstraint.weight = 0;
    }

    public float DistanceOfCurrentPlayerModel(){
        return Vector3.Distance(transform.position, PlayerController.Instance.currentPlayerModel.transform.position);
    }

    public void Enter(){
        navMeshAgent.enabled = false;
    }

    public void Exit(){
        navMeshAgent.enabled = true;
        SwitchState(PlayerState.Idle);
    }

}
