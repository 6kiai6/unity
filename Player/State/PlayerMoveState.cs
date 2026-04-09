using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private float MoveBlend = 0;
    private float runThreshold = 0;
    private float sprintThreshold = 1;
    private float transitionSpeed = 5;
    private int MoveBlendHash;
    
    public override void Init(IstateMachineOwner owner){
        base.Init(owner);
        MoveBlendHash = Animator.StringToHash("MoveBlend");
    }
    
    public override void Enter(){
        base.Enter();
        playerModel.PlayStateAnimation("Move");
    }

    public override void Update(){
        base.Update();

        //玩家控制
        if(IsBeControl()){
            if(playerController.jumpInput){
                SwitchToHover();
                return;
            }
            
            if(playerController.moveInput.magnitude == 0){
                playerModel.SwitchState(PlayerState.Idle);
                return;
            }

            if(playerController.sprintInput){
                MoveBlend = Mathf.Lerp(MoveBlend, sprintThreshold, transitionSpeed * Time.deltaTime);
            }
            else{
                MoveBlend = Mathf.Lerp(MoveBlend, runThreshold, transitionSpeed * Time.deltaTime);
            }
            playerModel.animator.SetFloat(MoveBlendHash, MoveBlend);


            // if (playerController.worldMovement.sqrMagnitude > 0.01f)  // 避免零向量导致 LookRotation 异常
            // {
            //     playerModel.transform.rotation = Quaternion.Lerp(playerModel.transform.rotation,
            //                                      Quaternion.LookRotation(playerController.worldMovement), 
            //                                      rotateSpeed * Time.deltaTime);
            // }

            float angle = Mathf.Atan2(playerController.localMovement.x, playerController.localMovement.z);
            playerModel.transform.Rotate(0, angle * playerController.rotationSpeed * Time.deltaTime, 0);
                
        }
        //人机
        else{
            if(playerModel.DistanceOfCurrentPlayerModel() - playerModel.stoppingDistance < 2f){
                MoveBlend = Mathf.Lerp(MoveBlend, runThreshold, transitionSpeed * Time.deltaTime);
            }
            else{
                MoveBlend = Mathf.Lerp(MoveBlend, sprintThreshold, transitionSpeed * Time.deltaTime);
            }
            playerModel.animator.SetFloat(MoveBlendHash, MoveBlend);

            if(playerModel.DistanceOfCurrentPlayerModel() <= playerModel.stoppingDistance){
                playerModel.SwitchState(PlayerState.Idle);
                return;
            }
            playerModel.navMeshAgent.SetDestination(PlayerController.Instance.currentPlayerModel.transform.position);

        }


    }

}
