using UnityEngine;

/// <summary>
/// 子弹初速乘区。
/// </summary>
public sealed class ProjectileSpeedMultiplierWeaponDecorator : WeaponStatDecorator
{
    private readonly float _multiplier;

    public ProjectileSpeedMultiplierWeaponDecorator(IWeaponStatisticsPipeline inner, float multiplier)
        : base(inner)
    {
        _multiplier = Mathf.Max(0.01f, multiplier);
    }

    protected override void Apply(ref WeaponStatSnapshot stats)
    {
        stats.projectileSpeed *= _multiplier;
    }
}
