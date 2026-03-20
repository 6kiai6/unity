using UnityEngine;
using System;
using System.Collections.Generic;

public class MonoManager : SingleMonoBase<MonoManager>
{
    
    private Action updateAction;

    public void AddUpdateAction(Action action){
        updateAction += action;
    }

    public void RemoveUpdateAction(Action action){
        updateAction -= action;
    }

    void Update()
    {
        updateAction?.Invoke();
    }
}
