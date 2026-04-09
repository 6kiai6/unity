/// <summary>
/// 武器数值管道：装饰者模式中的 Component，可层层包装。
/// </summary>
public interface IWeaponStatisticsPipeline
{
    WeaponStatSnapshot GetSnapshot();
}
