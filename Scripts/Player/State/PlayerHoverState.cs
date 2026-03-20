using UnityEngine;

public class PlayerHoverState : PlayerStateBase
{
    public override void Enter(){
        base.Enter();
        playerModel.PlayStateAnimation("Hover");
    }

    public override void Update(){
        base.Update();
        // 使用 isGrounded + 短距离射线，避免 CharacterController 落地检测延迟导致一直停在 Hover
        // if(playerModel.cc.isGrounded || playerModel.IsGroundNear(0.35f)){
        //     playerModel.verticalSpeed = 0f;
        //     playerModel.SwitchState(PlayerState.Idle);
        // }

        //得到跳跃动画的时间
        // float jumpAnimationTime = playerModel.animator.GetCurrentAnimatorStateInfo(0).length;
        // jumpTotalTime += Time.deltaTime;jumpTotalTime >= jumpAnimationTime && 

        if(playerModel.IsGroundNear()){
            playerModel.verticalSpeed = 0f;
            playerModel.SwitchState(PlayerState.Idle);
        }

    }
}
