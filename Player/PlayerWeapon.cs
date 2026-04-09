using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Tooltip("子弹生成位置")]
    public Transform bulletSpawnPoint;
    [Tooltip("子弹预制体")]
    public PlayerWeaponBullet bulletPrefab;
    [Tooltip("枪管火花预制体")]
    public GameObject bulletSparkPrefab;

    [Header("基础数值（装饰链最内层，可被药水等装饰者修改）")]
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float baseFireInterval = 0.15f;
    [SerializeField] private float baseProjectileSpeed = 30f;

    [Header("药水 / Buff")]
    [SerializeField] private WeaponBuffTable weaponBuffTable;
    [Tooltip("额外常驻装饰者（可选，在 Inspector 里挂继承 WeaponStatDecorator 的自定义组件时使用）")]
    [SerializeField] private List<MonoBehaviour> extraDecoratorProviders;

    private float _lastFireTime;

    private readonly List<RuntimeWeaponBuff> _runtimeBuffs = new List<RuntimeWeaponBuff>();

    private struct RuntimeWeaponBuff
    {
        public WeaponBuffSnapshot Snapshot;
        /// <summary>&lt; 0 表示不限时 </summary>
        public float TimeRemaining;
    }

    private void Awake()
    {
        if (bulletPrefab != null)
        {
            if (baseDamage <= 0)
                baseDamage = bulletPrefab.damage;
            if (baseProjectileSpeed <= 0f)
                baseProjectileSpeed = bulletPrefab.flyPower;
        }

        if (baseFireInterval <= 0f)
            baseFireInterval = 0.15f;
    }

    private void Update()
    {
        for (int i = _runtimeBuffs.Count - 1; i >= 0; i--)
        {
            RuntimeWeaponBuff b = _runtimeBuffs[i];
            if (b.TimeRemaining < 0f)
                continue;

            b.TimeRemaining -= Time.deltaTime;
            if (b.TimeRemaining <= 0f)
                _runtimeBuffs.RemoveAt(i);
            else
                _runtimeBuffs[i] = b;
        }
    }

    /// <summary>供 WeaponStatsRoot 读取基础面板</summary>
    internal WeaponStatSnapshot GetBaseSnapshot()
    {
        return new WeaponStatSnapshot
        {
            damage = Mathf.Max(1, baseDamage),
            fireInterval = Mathf.Max(0.01f, baseFireInterval),
            projectileSpeed = Mathf.Max(0.01f, baseProjectileSpeed)
        };
    }

    /// <summary>构建完整装饰链并取最终数值</summary>
    public WeaponStatSnapshot GetCurrentStats()
    {
        IWeaponStatisticsPipeline pipe = new WeaponStatsRoot(this);

        for (int i = 0; i < _runtimeBuffs.Count; i++)
            pipe = new ConfiguredWeaponBuffDecorator(pipe, _runtimeBuffs[i].Snapshot);

        pipe = ApplyExtraDecorators(pipe);

        return pipe.GetSnapshot();
    }

    /// <summary>
    /// 允许在 Inspector 挂自定义 MonoBehaviour，实现 IWeaponStatisticsPipelineProvider 以插入装饰层。
    /// </summary>
    private IWeaponStatisticsPipeline ApplyExtraDecorators(IWeaponStatisticsPipeline pipe)
    {
        if (extraDecoratorProviders == null)
            return pipe;

        for (int i = 0; i < extraDecoratorProviders.Count; i++)
        {
            if (extraDecoratorProviders[i] is IWeaponStatisticsPipelineProvider provider)
                pipe = provider.Wrap(pipe);
        }

        return pipe;
    }

    /// <summary>
    /// 消耗背包 1 个道具并套用 WeaponBuffTable 中对应行；无配置或扣除失败返回 false。
    /// </summary>
    public bool TryApplyWeaponBuffFromInventory(int itemId)
    {
        if (weaponBuffTable == null || !weaponBuffTable.TryGetRow(itemId, out WeaponBuffRow row))
            return false;

        var pm = PackageManager.Instance;
        if (pm == null || !pm.TryRemoveItem(itemId, 1))
            return false;

        var snapshot = new WeaponBuffSnapshot(row);
        float timer = row.durationSeconds > 0f ? row.durationSeconds : -1f;
        _runtimeBuffs.Add(new RuntimeWeaponBuff { Snapshot = snapshot, TimeRemaining = timer });
        return true;
    }

    /// <summary>代码中直接叠一层药水效果（不扣背包），限时同上规则</summary>
    public void AddRuntimeBuff(WeaponBuffSnapshot snapshot, float durationSeconds)
    {
        float timer = durationSeconds > 0f ? durationSeconds : -1f;
        _runtimeBuffs.Add(new RuntimeWeaponBuff { Snapshot = snapshot, TimeRemaining = timer });
    }

    public void Fire(Vector3 target)
    {
        WeaponStatSnapshot stats = GetCurrentStats();

        if (Time.time - _lastFireTime < stats.fireInterval)
            return;

        _lastFireTime = Time.time;
        Vector3 direction = (target - bulletSpawnPoint.position).normalized;
        PlayerWeaponBullet bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.transform.forward = direction;
        bullet.ApplyWeaponStats(stats.damage, stats.projectileSpeed);

        GameObject spark = Instantiate(bulletSparkPrefab, bulletSpawnPoint.position, Quaternion.identity);
        spark.transform.forward = direction;
    }
}
