using UnityEngine;

/// <summary>
/// 武器一次射击相关的最终数值（经装饰链计算后）。
/// </summary>
[System.Serializable]
public struct WeaponStatSnapshot
{
    public int damage;
    public float fireInterval;
    public float projectileSpeed;
}
