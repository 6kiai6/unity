using System;
using UnityEngine;

public class PlayerWeaponBullet : MonoBehaviour
{
    [Tooltip("子弹伤害")]
    public int damage = 10;
    [Tooltip("子弹存活时间")]
    public float lifeTime = 10f;
    [HideInInspector]
    public Rigidbody rb;
    [Tooltip("子弹推力")]
    public float flyPower = 30f;

    public Vector3 prevPosition;

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }

    void Start(){
        rb.linearVelocity = transform.forward * flyPower;
        prevPosition = transform.position;
        CheckInltOverlap();
        Destroy(gameObject, lifeTime);
        //ObjPoolManager.Instance.ReleaseObj(gameObject, lifeTime);
        //ObjPoolManager.Instance.PushObj(gameObject);
    }


    void Update(){
        CheckCollision();
        prevPosition = transform.position;
    }

    public void CheckInltOverlap(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach(var collider in colliders){
            if(collider.CompareTag("Enemy")){
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                enemy.Hurt(this);
                Destroy(gameObject, 0.1f);
                //ObjPoolManager.Instance.ReleaseObj(gameObject, 0.1f);
            }
        }
    }

    public void CheckCollision(){
        RaycastHit hit;
        Vector3 direction = transform.position - prevPosition;
        if(Physics.Raycast(prevPosition, direction, out hit, direction.magnitude)){
            if(hit.collider.CompareTag("Enemy")){
                EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                enemy.Hurt(this);
                Destroy(gameObject, 0.1f);
                //ObjPoolManager.Instance.ReleaseObj(gameObject, 0.1f);
            }
            // else if(hit.collider.CompareTag("Sphere")){
            //     Spherekk kk = hit.collider.GetComponent<Spherekk>();
            //     kk.IsAttack();
            //     Destroy(gameObject, 0.1f);
            // }
        }
    }




}
