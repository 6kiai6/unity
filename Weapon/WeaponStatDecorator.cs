/// <summary>
/// 抽象装饰者：先取内层快照，再局部修改。
/// </summary>
public abstract class WeaponStatDecorator : IWeaponStatisticsPipeline
{
    private readonly IWeaponStatisticsPipeline _inner;

    protected WeaponStatDecorator(IWeaponStatisticsPipeline inner)
    {
        _inner = inner;
    }

    public WeaponStatSnapshot GetSnapshot()
    {
        WeaponStatSnapshot s = _inner.GetSnapshot();
        Apply(ref s);
        return s;
    }

    protected abstract void Apply(ref WeaponStatSnapshot stats);
}
