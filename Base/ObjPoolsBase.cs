using System.Collections.Generic;
using UnityEngine;

public class ObjPoolsBase : SingleMonoBase<ObjPoolsBase>
{

    [SerializeField]
    protected Transform inactiveRoot;

    Dictionary<GameObject, Stack<GameObject>> pools = new Dictionary<GameObject, Stack<GameObject>>();

    public GameObject GetObj(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new Stack<GameObject>());
        }

        if (pools[prefab].Count > 0)
        {
            GameObject obj = pools[prefab].Pop();
            obj.SetActive(true);
            if (inactiveRoot != null)
            {
                obj.transform.SetParent(null, true);
            }
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab);
            obj.name = prefab.name;
            if (inactiveRoot != null)
            {
                obj.transform.SetParent(null, true);
            }
            return obj;
        }
    }

    public void ReleaseObj(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new Stack<GameObject>());
        }
        pools[prefab].Push(prefab);
        prefab.SetActive(false);
        if (inactiveRoot != null)
        {
            prefab.transform.SetParent(inactiveRoot, true);
        }
    }

    public void ClearPools()
    {
        foreach (var pool in pools)
        {
            while (pool.Value.Count > 0)
            {
                GameObject obj = pool.Value.Pop();
                Destroy(obj);
            }
        }
        pools.Clear();
        base.OnDestroy();
    }

}


public class ObjPoolBase<T> : SingleMonoBase<ObjPoolBase<T>> where T : Component
{
    protected readonly Stack<T> pool = new Stack<T>();
    
    [SerializeField]
    protected Transform inactiveRoot;
}
