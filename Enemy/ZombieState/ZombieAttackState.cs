using UnityEngine;

public class ZombieAttackState : EnemyStateBase
{
    public override void Enter()
    {
        base.Enter();
        enemyBase.PlayStateAnimation("Attack");
    }
}
