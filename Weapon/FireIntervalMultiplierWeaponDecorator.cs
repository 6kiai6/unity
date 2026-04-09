using UnityEngine;

/// <summary>
/// 射击间隔乘区：小于 1 射得更快（如 0.8 = 间隔为原来的 80%）。
/// </summary>
public sealed class FireIntervalMultiplierWeaponDecorator : WeaponStatDecorator
{
    private readonly float _multiplier;

    public FireIntervalMultiplierWeaponDecorator(IWeaponStatisticsPipeline inner, float multiplier)
        : base(inner)
    {
        _multiplier = Mathf.Max(0.01f, multiplier);
    }

    protected override void Apply(ref WeaponStatSnapshot stats)
    {
        stats.fireInterval *= _multiplier;
    }
}
