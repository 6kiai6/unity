using UnityEngine;

public class ZombieMoveState : EnemyStateBase
{
    public override void Enter()
    {
        base.Enter();
        enemyBase.PlayStateAnimation("Move");
        enemyBase.ChaseAttackTarget();
    }

    public override void Update(){
        base.Update();
        if(enemyBase.IsAttackTargetInAttackRange()){
            enemyBase.SwitchState(EnemyState.Idle);
        }
        else{
            enemyBase.ChaseAttackTarget();
        }
    }

}
