using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState{
    Idle,
    Move,
    Attack,
    Dead
}


public abstract class EnemyBase : MonoBehaviour, IstateMachineOwner
{
    [HideInInspector]
    public Animator animator;
    public StateMachine stateMachine;

    #region 寻路相关
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    public float rotationSpeed = 300f; //转向速度
    public float attackMinDistance = 0.5f; //最小攻击距离
    public float attackMaxRange = 10f; //最大追击范围
    public PlayerModel attackTarget; //攻击目标

    #endregion

    #region 流血相关
    public GameObject bloodSplashPrefab;
    public GameObject bloodDrippingPrefab;
    #endregion

    #region 受击相关
    private int HitHash;
    private int MoveSpeedHash;
    private float normalMoveSpeed = 1f;
    private float slowMoveSpeed = 0.5f;
    private Coroutine recoverMoveSpeedCoroutine;

    #endregion

    #region 血量相关
    public float health = 100f;
    private float currentHealth;
    private bool IsDead = false;
    [Tooltip("血条预制体")]
    public GameObject healthBarPrefab;
    [Tooltip("血条生成位置")]
    public Transform healthBarTransform;
    [HideInInspector]
    public GameObject healthBar;

    public float healthBarShowTime = 6f;
    private float healthBarShowTimer;

    #endregion


    protected virtual void Awake(){
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.angularSpeed = rotationSpeed;
        navMeshAgent.stoppingDistance = attackMinDistance;
        HitHash = Animator.StringToHash("Hit");
        MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        currentHealth = health;
        healthBarShowTimer = healthBarShowTime;
        stateMachine = new StateMachine(this);
    }
    

    protected virtual void Start(){
        SwitchState(EnemyState.Idle);
        FindAttackTarget();
        healthBar = Instantiate(healthBarPrefab, healthBarTransform.position, Quaternion.identity);
        healthBar.transform.SetParent(UIManager.Instance.WorldSpaceCanvas.transform);
    }

    protected virtual void Update(){
        if(IsDead) return;

        if(healthBarShowTimer < healthBarShowTime){
            healthBar.SetActive(true);
            healthBar.transform.position = healthBarTransform.position;
            healthBarShowTimer += Time.deltaTime;
        }
        else{
            healthBar.SetActive(false);
        }
    }


    public virtual void SlowMoveSpeed(){
        animator.SetFloat(MoveSpeedHash, slowMoveSpeed);
        if(recoverMoveSpeedCoroutine != null){
            StopCoroutine(recoverMoveSpeedCoroutine);
        }
        recoverMoveSpeedCoroutine = StartCoroutine(RecoverMoveSpeed(0.5f));
    }

    public IEnumerator RecoverMoveSpeed(float delay){
        yield return new WaitForSeconds(delay);
        animator.SetFloat(MoveSpeedHash, normalMoveSpeed);
        recoverMoveSpeedCoroutine = null;
    }

    public virtual void Hurt(PlayerWeaponBullet bullet, float damgeMultiplier = 1f){
        animator.SetTrigger(HitHash);
        SlowMoveSpeed();

        Vector3 dir = bullet.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(-dir);

        Destroy(Instantiate(bloodSplashPrefab, transform.position, rotation), 1f);
        Destroy(Instantiate(bloodDrippingPrefab, transform.position+Vector3.up*0.1f, Quaternion.identity), 3f);

        currentHealth -= bullet.damage * damgeMultiplier;
        if(currentHealth > 0){
            healthBar.GetComponent<EnemyHealthBarUI>().UpdateHealthBar(currentHealth / health);
            healthBarShowTimer = 0f;
        }
        else{
            SwitchState(EnemyState.Dead);
            Destroy(healthBar);
            navMeshAgent.enabled = false;
            GetComponent<Collider>().enabled = false;
            currentHealth = 0;
            IsDead = true;
        }

    }


    public void FindAttackTarget(){
        PlayerModel[] playerModels = GameManager.Instance.playerModels;
        if(playerModels != null && playerModels.Length > 0){
            PlayerModel closestPlayer = null;
            float minDistance = float.MaxValue;
            foreach(PlayerModel player in playerModels){
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if(distance < minDistance){
                    minDistance = distance;
                    closestPlayer = player;
                }
            }
            attackTarget = closestPlayer;
        }
    }

    public virtual bool HasAttackTarget(){
        return attackTarget != null;
    }

    public virtual bool IsAttackTargetInAttackRange(){
        if(HasAttackTarget()){
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= attackMinDistance;
        }
        return false;
    }

    public virtual void ChaseAttackTarget(){
        navMeshAgent.SetDestination(attackTarget.transform.position);
    }

    //状态切换
    public abstract void SwitchState(EnemyState state);


    /// <summary>
    /// 播放状态动画
    /// </summary>
    /// <param name="AnimationName">动画名称</param>
    /// <param name="transition">过渡时间</param>
    /// <param name="layer">层级</param>
    public void PlayStateAnimation(string AnimationName, float transition = 0.2f, int layer = 0){
        animator.CrossFadeInFixedTime(AnimationName, transition, layer);
    }

    public void Clear(){
        stateMachine.Stop();
        Destroy(gameObject);
    }

}
