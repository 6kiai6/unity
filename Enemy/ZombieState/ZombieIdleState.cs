using UnityEngine;
using UnityEngine.AI;

public class ZombieIdleState : EnemyStateBase
{
    public override void Enter()
    {
        base.Enter();
        enemyBase.PlayStateAnimation("Idle");
        enemyBase.navMeshAgent.velocity = Vector3.zero;
    }

    public override void Update(){
        base.Update();
        if(!enemyBase.IsAttackTargetInAttackRange()){
            enemyBase.SwitchState(EnemyState.Move);
        }
    }

}
