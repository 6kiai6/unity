/// <summary>
/// 挂在武器同一角色上的扩展组件：向管道最外侧再包一层（如配件、光环）。
/// </summary>
public interface IWeaponStatisticsPipelineProvider
{
    IWeaponStatisticsPipeline Wrap(IWeaponStatisticsPipeline inner);
}
