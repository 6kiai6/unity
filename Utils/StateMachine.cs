using UnityEngine;
using System.Collections.Generic;
using System;

public interface IstateMachineOwner{

}

public class StateMachine
{
    private StateBase currentState;
    private IstateMachineOwner owner;
    private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>();

    public StateMachine(IstateMachineOwner owner){
        this.owner = owner;
    }

    public void EnterState<T>() where T : StateBase, new(){
        if(currentState != null && currentState.GetType() == typeof(T)){
            return;
        }
        currentState?.Exit();
        currentState = LoadState<T>();
        currentState.Enter();
    }

    public StateBase LoadState<T>() where T : StateBase, new(){
        Type stateType = typeof(T);
        if(!stateDic.TryGetValue(stateType, out StateBase state)){
            state = new T();
            state.Init(owner);
            stateDic.Add(stateType, state);
        }
        return state;
    }

    public void Stop(){
        currentState?.Exit();
        foreach(var state in stateDic.Values){
            state.Destroy();
        }
        stateDic.Clear();
    }
}
