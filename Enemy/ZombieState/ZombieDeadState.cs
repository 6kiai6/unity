using UnityEngine;

public class ZombieDeadState : EnemyStateBase
{
    public override void Enter()
    {
        base.Enter();
        enemyBase.PlayStateAnimation("Dead");
    }

    public override void Update()
    {
        base.Update();
        if (IsAnimationBreak())
        {
            //enemyBase.Clear();
            EnemyPool.Instance.ReleaseObj(enemyBase as ZombieEnemy);
        }
    }

}
