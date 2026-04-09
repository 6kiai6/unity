using System;
using System.Collections.Generic;
using UnityEngine;


public class PackageManager : SingleMonoBase<PackageManager>
{
    [SerializeField] private PackageTable runtimeTable;

    public event Action OnInventoryChanged;

    public PackageTable Table => runtimeTable;

    protected override void Awake()
    {
        base.Awake();
        PackageLocalData.Instance.LoadPackage();
    }

    public List<PackageLocalItem> GetRuntimeItems()
    {
        return PackageLocalData.Instance.LoadPackage();
    }

    public PackageTableItem GetDefinition(int itemId)
    {
        if (runtimeTable == null || runtimeTable.DataList == null)
            return null;

        for (int i = 0; i < runtimeTable.DataList.Count; i++)
        {
            if (runtimeTable.DataList[i].id == itemId)
                return runtimeTable.DataList[i];
        }

        return null;
    }

    public bool TryAddItem(int itemId, int count, int level = 1)
    {
        if (count <= 0)
            return false;

        if (GetDefinition(itemId) == null)
        {
            Debug.LogWarning("PackageManager: PackageTable 中不存在 id=" + itemId);
            return false;
        }

        var items = PackageLocalData.Instance.LoadPackage();
        if (items == null)
        {
            items = new List<PackageLocalItem>();
            PackageLocalData.Instance.items = items;
        }

        var stack = items.Find(x => x.id == itemId && x.level == level);
        if (stack != null)
        {
            stack.num += count;
            stack.isNew = true;
        }
        else
        {
            items.Add(new PackageLocalItem
            {
                uid = Guid.NewGuid().ToString("N"),
                id = itemId,
                num = count,
                level = level,
                isNew = true
            });
        }

        PackageLocalData.Instance.SavePackage();
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool TryRemoveItem(int itemId, int count, int level = 1)
    {
        if (count <= 0)
            return false;

        var items = PackageLocalData.Instance.LoadPackage();
        if (items == null)
            return false;

        var stack = items.Find(x => x.id == itemId && x.level == level);
        if (stack == null || stack.num < count)
            return false;

        stack.num -= count;
        if (stack.num <= 0)
            items.Remove(stack);

        PackageLocalData.Instance.SavePackage();
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetStackCount(int itemId, int level = 1)
    {
        var items = PackageLocalData.Instance.LoadPackage();
        if (items == null)
            return 0;

        var stack = items.Find(x => x.id == itemId && x.level == level);
        return stack != null ? stack.num : 0;
    }

    public bool HasEnough(int itemId, int count, int level = 1)
    {
        return GetStackCount(itemId, level) >= count;
    }

    public void ClearNewFlags()
    {
        var items = PackageLocalData.Instance.LoadPackage();
        if (items == null)
            return;

        foreach (var it in items)
            it.isNew = false;

        PackageLocalData.Instance.SavePackage();
        OnInventoryChanged?.Invoke();
    }
}
