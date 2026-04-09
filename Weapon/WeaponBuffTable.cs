using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运行时拷贝，避免持有 ScriptableObject 引用被误改。
/// </summary>
public readonly struct WeaponBuffSnapshot
{
    public readonly float DamageMultiplier;
    public readonly float FireIntervalMultiplier;
    public readonly float ProjectileSpeedMultiplier;

    public WeaponBuffSnapshot(WeaponBuffRow row)
    {
        DamageMultiplier = row.damageMultiplier <= 0f ? 1f : row.damageMultiplier;
        FireIntervalMultiplier = row.fireIntervalMultiplier <= 0f ? 1f : row.fireIntervalMultiplier;
        ProjectileSpeedMultiplier = row.projectileSpeedMultiplier <= 0f ? 1f : row.projectileSpeedMultiplier;
    }
}

[System.Serializable]
public class WeaponBuffRow
{
    [Tooltip("与 PackageTableItem.id 对应，消耗 1 个后套用本行增益")]
    public int itemId;

    [Tooltip("伤害乘数，1 为不变")]
    public float damageMultiplier = 1f;

    [Tooltip("射击间隔乘数，小于 1 间隔更短（射更快）")]
    public float fireIntervalMultiplier = 1f;

    [Tooltip("子弹初速乘数")]
    public float projectileSpeedMultiplier = 1f;

    [Tooltip("持续时间（秒）；≤0 表示本局永久，直到被移除")]
    public float durationSeconds;
}

[CreateAssetMenu(menuName = "Weapon/WeaponBuffTable", fileName = "WeaponBuffTable")]
public class WeaponBuffTable : ScriptableObject
{
    public List<WeaponBuffRow> rows = new List<WeaponBuffRow>();

    public bool TryGetRow(int itemId, out WeaponBuffRow row)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] != null && rows[i].itemId == itemId)
            {
                row = rows[i];
                return true;
            }
        }

        row = null;
        return false;
    }
}
