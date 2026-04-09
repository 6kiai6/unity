using UnityEngine;

public class SingleMonoBase<T> : MonoBehaviour where T : SingleMonoBase<T>
{
    public static T Instance;
    
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.Log(name + "不符合单例模式");
        }
        Instance = this as T;
    }
    protected virtual void OnDestroy(){
        Instance = null;
    }
}
