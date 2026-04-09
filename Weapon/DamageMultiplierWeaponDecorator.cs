using UnityEngine;

/// <summary>
/// 仅放大/缩小伤害的装饰者（可与其他装饰者任意嵌套）。
/// </summary>
public sealed class DamageMultiplierWeaponDecorator : WeaponStatDecorator
{
    private readonly float _multiplier;

    public DamageMultiplierWeaponDecorator(IWeaponStatisticsPipeline inner, float multiplier)
        : base(inner)
    {
        _multiplier = multiplier;
    }

    protected override void Apply(ref WeaponStatSnapshot stats)
    {
        stats.damage = Mathf.Max(1, Mathf.RoundToInt(stats.damage * _multiplier));
    }
}
