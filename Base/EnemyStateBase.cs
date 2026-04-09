using UnityEngine;
using System;

public class EnemyStateBase : StateBase
{
    //protected EnemyBase enemyBase;
    protected EnemyBase enemyBase;

    public override void Init(IstateMachineOwner owner)
    {
        //enemyBase = owner as EnemyBase;
        enemyBase = owner as EnemyBase;
    }

    public override void Enter(){
        MonoManager.Instance.AddUpdateAction(Update);
    }

    public virtual void Update(){

    }

    public override void Exit(){
        MonoManager.Instance.RemoveUpdateAction(Update);
    }

    public override void Destroy(){
        
    }

    public bool IsAnimationBreak(int layer = 0){
        AnimatorStateInfo stateInfo = enemyBase.animator.GetCurrentAnimatorStateInfo(layer);
        return stateInfo.normalizedTime >= 1.0f && !enemyBase.animator.IsInTransition(layer);
    }

}
