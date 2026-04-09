using UnityEngine;

public class PlayerAmingState : PlayerStateBase
{
    private int AmingXHash;
    private int AmingYHash;
    private float AmingX = 0;
    private float AmingY = 0;
    private float transitionSpeed = 5;


    public override void Init(IstateMachineOwner owner){
        base.Init(owner);
        AmingXHash = Animator.StringToHash("AmingX");
        AmingYHash = Animator.StringToHash("AmingY");
    }

    public override void Enter(){
        base.Enter();
        playerModel.PlayStateAnimation("Aming");
        UpdateAimTarget();
        if(IsBeControl()){
            playerController.EnterAim();
        }
    }

    public override void Update(){
        base.Update();

        if(IsBeControl()){
            playerModel.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            UpdateAimTarget();
            if(!playerController.aimInput && !playerController.attackInput){
                playerModel.SwitchState(PlayerState.Idle);
                return;
            }
            if(playerController.attackInput){
                playerModel.weapon.Fire(playerController.aimTarget.position);
                playerController.ShakeCamera();
            }

            AmingX = Mathf.Lerp(AmingX, playerController.moveInput.x, transitionSpeed * Time.deltaTime);
            AmingY = Mathf.Lerp(AmingY, playerController.moveInput.y, transitionSpeed * Time.deltaTime);
            playerModel.animator.SetFloat(AmingXHash, AmingX);
            playerModel.animator.SetFloat(AmingYHash, AmingY);

        }
    }

    public void UpdateAimTarget(){
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray , out hit , playerController.maxDistance, playerController.aimLayerMask)){
            playerController.aimTarget.position = hit.point;
        }
        else{
            playerController.aimTarget.position = ray.origin + ray.direction * playerController.maxDistance;
        }
    }


    public override void Exit(){
        base.Exit();
        if(IsBeControl()){
            playerController.ExitAim();
        }
    }
}
