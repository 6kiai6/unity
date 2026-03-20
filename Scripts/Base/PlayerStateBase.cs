using UnityEngine;

public class PlayerStateBase : StateBase
{
    protected PlayerController playerController;
    protected PlayerModel playerModel;


    public override void Init(IstateMachineOwner owner){
        playerController = PlayerController.Instance;
        playerModel = owner as PlayerModel;
    }

    public override void Enter(){
        MonoManager.Instance.AddUpdateAction(Update);
    }

    public override void Exit(){
        MonoManager.Instance.RemoveUpdateAction(Update);
    }

    public override void Destroy(){
        
    }

    public virtual void Update(){

        //画射线
        //Debug.DrawRay(playerModel.transform.position, Vector3.down * playerModel.fallHeight, Color.red);

        // if(!playerModel.cc.isGrounded){
        //     playerModel.verticalSpeed += playerModel.gravity * Time.deltaTime;
        //     if(playerModel.IsHover()){
        //         playerModel.SwitchState(PlayerState.Hover);
        //     }
        // }
        // else{
        //     playerModel.verticalSpeed = playerModel.gravity * Time.deltaTime;
        // }

        if(!playerModel.IsGroundNear()){
            playerModel.verticalSpeed += playerModel.gravity * Time.deltaTime;
            if(playerModel.IsHover() && playerModel.currentState != PlayerState.Hover){
                playerModel.SwitchState(PlayerState.Hover);
            }
        }
        if(IsBeControl() && (playerController.aimInput || playerController.attackInput)){
            playerModel.SwitchState(PlayerState.Aming);
        }

    }

    public void SwitchToHover(){
        playerModel.verticalSpeed = Mathf.Sqrt(-2 * playerModel.gravity * playerModel.jumpHeight);
        playerModel.SwitchState(PlayerState.Hover);
    }

    public bool IsBeControl(){
        return playerController.currentPlayerModel == playerModel;
    }
}
