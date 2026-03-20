using UnityEngine;

public class PlayerIdelState : PlayerStateBase
{
    public override void Enter(){
        base.Enter();
        playerModel.PlayStateAnimation("Idle");
    }

    public override void Update(){
        base.Update();
        if(IsBeControl()){
            if(playerController.moveInput.magnitude > 0){
                playerModel.SwitchState(PlayerState.Move);
            }

            if(playerController.jumpInput){
                SwitchToHover();
            }
        }
        else{
            if(playerModel.DistanceOfCurrentPlayerModel() > playerModel.stoppingDistance){
                playerModel.SwitchState(PlayerState.Move);
            }
        }
    }
}
