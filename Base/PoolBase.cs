using System.Collections.Generic;
using UnityEngine;

public abstract class PoolBase<TPool, TItem> : SingleMonoBase<TPool>
    where TPool : PoolBase<TPool, TItem>
    where TItem : Component
{
    protected readonly Stack<TItem> pool = new Stack<TItem>();

    [SerializeField]
    protected Transform inactiveRoot;

    public TItem GetObj(TItem prefab)
    {
        while (pool.Count > 0)
        {
            TItem item = pool.Pop();
            if (item != null)
            {
                if (inactiveRoot != null)
                    item.transform.SetParent(null, true);
                item.gameObject.SetActive(true);
                return item;
            }
        }

        return Instantiate(prefab);
    }

    public TItem GetObj(TItem prefab, Vector3 position, Quaternion rotation)
    {
        while (pool.Count > 0)
        {
            TItem item = pool.Pop();
            if (item != null)
            {
                if (inactiveRoot != null)
                    item.transform.SetParent(null, true);
                item.transform.SetPositionAndRotation(position, rotation);
                item.gameObject.SetActive(true);
                return item;
            }
        }

        return Instantiate(prefab, position, rotation);
    }

    public void ReleaseObj(TItem obj)
    {
        if (obj == null)
            return;

        obj.gameObject.SetActive(false);
        if (inactiveRoot != null)
            obj.transform.SetParent(inactiveRoot, true);
        pool.Push(obj);
    }

    public void ClearPool()
    {
        while (pool.Count > 0)
        {
            TItem item = pool.Pop();
            if (item != null)
                Destroy(item.gameObject);
        }
    }

    protected override void OnDestroy()
    {
        ClearPool();
        base.OnDestroy();
    }
}
