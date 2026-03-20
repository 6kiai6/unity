using UnityEngine;
using UnityEngine.UIElements;

public class PlayerWeapon : MonoBehaviour
{
    [Tooltip("子弹生成位置")]
    public Transform bulletSpawnPoint;
    [Tooltip("子弹预制体")]
    public PlayerWeaponBullet bulletPrefab;
    [Tooltip("枪管火花预制体")]
    public GameObject  bulletSparkPrefab;
    [Tooltip("发射间隔")]
    public float bulletInterval = 0.15f;
    private float lastFireTime; //上次发射时间

    public void Fire(Vector3 Target){
        if(Time.time - lastFireTime < bulletInterval){
            return;
        }
        lastFireTime = Time.time;
        Vector3 dirction = (Target - bulletSpawnPoint.position).normalized;
        PlayerWeaponBullet bulletEffect = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        //GameObject bullet = ObjPoolManager.Instance.GetObj(
        //bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        
        GameObject Spark = Instantiate(bulletSparkPrefab, bulletSpawnPoint.position, Quaternion.identity);
        
        // GameObject Spark = ObjPoolManager.Instance.GetObj(
        // bulletSparkPrefab.gameObject, bulletSpawnPoint.position, Quaternion.identity);
        
        Spark.transform.forward = dirction;
        bulletEffect.transform.forward = dirction;
    }


}
