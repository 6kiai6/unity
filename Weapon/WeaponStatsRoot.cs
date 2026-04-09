/// <summary>
/// 管道最内层：仅返回枪械自身基础属性（来自 PlayerWeapon）。
/// </summary>
public sealed class WeaponStatsRoot : IWeaponStatisticsPipeline
{
    private readonly PlayerWeapon _weapon;

    public WeaponStatsRoot(PlayerWeapon weapon)
    {
        _weapon = weapon;
    }

    public WeaponStatSnapshot GetSnapshot()
    {
        return _weapon.GetBaseSnapshot();
    }
}
