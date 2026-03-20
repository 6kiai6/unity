using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : SingleMonoBase<PlayerController>
{
    public PlayerModel currentPlayerModel; //当前角色控制的模型
    private Transform cameraTransform;

    #region 瞄准相关
    [Tooltip("正常视角相机")]
    public CinemachineFreeLook freeCamera;
    [Tooltip("瞄准相机")]
    public CinemachineFreeLook amingCamera;

    [Tooltip("瞄准目标")]
    public Transform aimTarget;
    [Tooltip("最大距离")]
    public float maxDistance = 100f;
    [Tooltip("检测图层")]
    public LayerMask aimLayerMask = ~0;
    private CinemachineImpulseSource impulseSourse;

    #endregion

    #region 输入系统
    private MyInputSystem input; //输入系统
    [HideInInspector]
    public Vector2 moveInput; //移动输入
    [HideInInspector]
    public bool sprintInput; //冲刺输入
    [HideInInspector]
    public bool attackInput; //攻击输入
    [HideInInspector]
    public bool aimInput; //瞄准输入
    [HideInInspector]
    public bool jumpInput; //跳跃输入
    #endregion

    [HideInInspector]
    public Vector3 localMovement;
    [HideInInspector]
    public Vector3 worldMovement;
    
    [Tooltip("转向速度")]
    public float rotationSpeed = 300f;


    protected override void Awake(){
        base.Awake();
        input = new MyInputSystem();
    }


    void Start(){
        cameraTransform = Camera.main.transform;
        //隐藏光标
        //Cursor.lockState = CursorLockMode.Locked;
        ExitAim();
        currentPlayerModel.Enter();
        //EnterAim();
        ResetCameraTarget();
        impulseSourse = amingCamera.GetComponent<CinemachineImpulseSource>();
    }

    public void ResetCameraTarget(){
        amingCamera.Follow = currentPlayerModel.transform;
        amingCamera.LookAt = currentPlayerModel.transform;
        freeCamera.Follow = currentPlayerModel.transform;
        freeCamera.LookAt = currentPlayerModel.transform;
    }

    void Update(){
        #region 更新玩家输入
        moveInput = input.Player.Move.ReadValue<Vector2>().normalized;
        sprintInput = input.Player.Sprint.IsPressed();
        attackInput = input.Player.Attack.IsPressed();
        aimInput = input.Player.Aim.IsPressed();
        jumpInput = input.Player.Jump.IsPressed();
        #endregion

        Vector3 CameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        worldMovement = CameraForward * moveInput.y + cameraTransform.right * moveInput.x;    
        localMovement = currentPlayerModel.transform.InverseTransformVector(worldMovement);

        if(input.Player.First.triggered){
            SwitchPlayerModel(0);
        }
        else if(input.Player.Second.triggered){
            SwitchPlayerModel(1);
        }
    }


    public void SwitchPlayerModel(int index){
        currentPlayerModel.Exit();
        currentPlayerModel = GameManager.Instance.playerModels[index];
        currentPlayerModel.Enter();
        ResetCameraTarget();
    }


/// <summary>
/// 进入瞄准
/// </summary>
    public void EnterAim(){
        amingCamera.m_XAxis.Value = freeCamera.m_XAxis.Value;
        amingCamera.m_YAxis.Value = freeCamera.m_YAxis.Value;

        currentPlayerModel.EnterAim();

        freeCamera.Priority = 0;
        amingCamera.Priority = 10;
    }

/// <summary>
/// 退出瞄准
/// </summary>
    public void ExitAim(){
        freeCamera.m_XAxis.Value = amingCamera.m_XAxis.Value;
        freeCamera.m_YAxis.Value = amingCamera.m_YAxis.Value;

        currentPlayerModel.ExitAim();

        freeCamera.Priority = 10;
        amingCamera.Priority = 0;
    }


    public void ShakeCamera(){
        impulseSourse.GenerateImpulse();
    }

    private void OnEnable(){
        input.Enable();
    }

    private void OnDisable(){
        input.Disable();
    }

}
