using UnityEngine;

/// <summary>
/// 由配置表一行驱动的组合装饰者（伤害/射速/弹速可同时生效）。
/// </summary>
public sealed class ConfiguredWeaponBuffDecorator : WeaponStatDecorator
{
    private readonly float _damageMultiplier;
    private readonly float _fireIntervalMultiplier;
    private readonly float _projectileSpeedMultiplier;

    public ConfiguredWeaponBuffDecorator(IWeaponStatisticsPipeline inner, WeaponBuffSnapshot snapshot)
        : base(inner)
    {
        _damageMultiplier = snapshot.DamageMultiplier;
        _fireIntervalMultiplier = snapshot.FireIntervalMultiplier;
        _projectileSpeedMultiplier = snapshot.ProjectileSpeedMultiplier;
    }

    public ConfiguredWeaponBuffDecorator(IWeaponStatisticsPipeline inner, WeaponBuffRow row)
        : this(inner, new WeaponBuffSnapshot(row))
    {
    }

    protected override void Apply(ref WeaponStatSnapshot stats)
    {
        stats.damage = Mathf.Max(1, Mathf.RoundToInt(stats.damage * _damageMultiplier));
        stats.fireInterval *= _fireIntervalMultiplier;
        stats.projectileSpeed *= _projectileSpeedMultiplier;
    }
}
