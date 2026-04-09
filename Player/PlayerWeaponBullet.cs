using UnityEngine;

public class PlayerWeaponBullet : MonoBehaviour
{
    [Tooltip("子弹伤害（预制体默认值；开火时由 PlayerWeapon 覆盖）")]
    public int damage = 10;
    [Tooltip("子弹存活时间")]
    public float lifeTime = 10f;
    [HideInInspector]
    public Rigidbody rb;
    [Tooltip("子弹推力（预制体默认值；开火时由 PlayerWeapon 覆盖）")]
    public float flyPower = 30f;

    public Vector3 prevPosition;

    private bool _statsApplied;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (!_statsApplied)
            ApplyWeaponStats(damage, flyPower);

        rb.linearVelocity = transform.forward * flyPower;
        prevPosition = transform.position;
        CheckInltOverlap();
        Destroy(gameObject, lifeTime);
    }

    /// <summary>由 PlayerWeapon 在生成后注入最终数值（经装饰链计算）</summary>
    public void ApplyWeaponStats(int finalDamage, float finalFlyPower)
    {
        damage = Mathf.Max(1, finalDamage);
        flyPower = Mathf.Max(0.01f, finalFlyPower);
        _statsApplied = true;
        if (rb != null)
            rb.linearVelocity = transform.forward * flyPower;
    }

    void Update()
    {
        CheckCollision();
        prevPosition = transform.position;
    }

    public void CheckInltOverlap()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                enemy.Hurt(this);
                Destroy(gameObject, 0.1f);
            }
        }
    }

    public void CheckCollision()
    {
        Vector3 direction = transform.position - prevPosition;
        if (Physics.Raycast(prevPosition, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                enemy.Hurt(this);
                Destroy(gameObject, 0.1f);
            }
        }
    }
}
