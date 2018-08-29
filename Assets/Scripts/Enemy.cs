using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public enum EnemyType
{
    Ranged, Melee, Giant
}


[RequireComponent(typeof(HealthBarSystem))]
public class Enemy : MonoBehaviour
{
    public bool canRayCast = true;
    bool canGetDamageTick = true;
    bool canFreeze = true;
    bool isIceBlock;
    bool informedManager;
    NavMeshAgent agent;
    public EnemyType enemyType;
    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public GameObject spawn;
    public GameObject miniMapPointer;
    public float health;
    public float speed;
    public int index;
    public float freezingTime;
    public Animator animator;
    public float animatorSpeed = 1;
    public HealthBarSystem healthBarSystem;
    public float damage;
    public float range;
    public GameObject player;
    [SerializeField]
    public Road road;
    public float deathAnimationTime = 3f;
    bool dying;
    bool alive;
    public float distanceToPlayer;
    public GameObject smashParticles;
    public GameObject smashGround;
    bool attacking;
    public GameObject material;
    public Material iceMat;
    public Material ogMat;
    public ParticleSystem destroyIce;
    Material[] mats = new Material[1];
    Material[] ogmats;
    void Awake()
    {
        healthBarSystem = GetComponent<HealthBarSystem>();
        alive = true;
    }

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        ogmats = material.GetComponent<SkinnedMeshRenderer>().materials;
        player = GameManager.instance.player;
        target = GameManager.instance.towerTarget;
        agent.SetDestination(target.transform.position);
        animator.speed = animatorSpeed;
        if (enemyType != EnemyType.Giant)
        {
            animator.SetBool("walking", true);
            // animator.speed = speed;
        }


        healthBarSystem.SetMaxHelthValue(health);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsPlayerInRange() && alive && !attacking)
        {
            AttackTarget();
        }

        if (health <= 0 && !dying)
        {
            canRayCast = false;
            StartCoroutine(Die());
        }
        if(agent.enabled && !agent.isStopped){
            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <=0)
            {
                if(!informedManager){
                    GameManager.instance.spawnReady = true;
                    informedManager = true;
                    canRayCast = false;
                }
                animator.SetBool("walking", false);
            }
        }

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }



    public void GetDamage(float amount)
    {
        health -= amount;
        healthBarSystem.Hit();
        healthBarSystem.SetHelthValue(health);

        //Slow down
        if (enemyType != EnemyType.Giant && !isIceBlock)
        {
            speed *= 0.75f;
            agent.speed = speed;
            animator.speed = speed;
            animator.SetBool("hit", true);
            animator.SetBool("bodyshot", true);
        }
        //Plat slow animation
    }
    public void GetCritDamage(float amount)
    {
        //Headshot
        print("Headshot");
        if (enemyType != EnemyType.Giant)
            health -= health;
        else
            health -= amount * 2.5f;

        healthBarSystem.Hit();
        healthBarSystem.SetHelthValue(health);
        if (enemyType != EnemyType.Giant && !isIceBlock)
            animator.SetBool("headshot", true);

    }
    public void GetKneeDamage(float amount)
    {
        print("Arrow to the knee");
        if (!dying)
        {
            health -= amount;
            healthBarSystem.Hit();
            healthBarSystem.SetHelthValue(health);
            if (enemyType != EnemyType.Giant && !isIceBlock)
            {
                StartCoroutine(FreezForTime());
            }
        }
    }

    public void GetDamageTick(float damage)
    {
        if(canGetDamageTick)
        StartCoroutine(GetDamageTickRoutine(damage));
    }

    public void StepOnIce(){
        if(canFreeze && speed>0 && animator.speed >0 && enemyType != EnemyType.Giant)
        StartCoroutine(FreezeOverTime(0.1f));
    }

    IEnumerator GetDamageTickRoutine(float amnt){
        print("Get Damage "+ amnt);

        canGetDamageTick = false;
        health -= amnt;
        healthBarSystem.Hit();
        healthBarSystem.SetHelthValue(health);
        yield return new WaitForSeconds(0.2f);
        canGetDamageTick = true;
    }

    public IEnumerator FreezForTime()
    {
        speed = speed * 0.75f;
        agent.speed = 0;
        animatorSpeed *= 0.75f;
        //Stop for some time
        animator.SetBool("walking", false);
        yield return new WaitForSeconds(freezingTime);
        agent.speed = speed;
        animator.speed = animatorSpeed;
        animator.SetBool("walking", true);
    }

    public IEnumerator FreezeOverTime(float tick)
    {
        canFreeze = false;
        speed -= 0.08f;
        agent.speed -= 0.08f;
        animatorSpeed -= 0.03f;
        animator.speed = animatorSpeed;

        if (animatorSpeed < 0.2){
            isIceBlock = true;
            animatorSpeed = 0;
            animator.speed = 0;
            speed = 0;
            agent.speed = 0;
            IceBloc();
            canFreeze = false;
            ogMat = material.GetComponent<Renderer>().materials[0];

            mats[0] = iceMat;
            material.GetComponent<SkinnedMeshRenderer>().materials = mats;
            yield return new WaitForSeconds(5f);
            speed = 2.5f;
            agent.speed = speed;
            animatorSpeed = 1;
            animator.speed = animatorSpeed;
            isIceBlock = false;
            material.GetComponent<SkinnedMeshRenderer>().materials = ogmats;
        }
        yield return new WaitForSeconds(tick);
        canFreeze = true;
    }
    public void IceBloc(){
        
    }

    IEnumerator Die()
    {
        dying = true;
        alive = false;
        DestroyMiniMapPointer();
        GameManager.instance.enemiesRemain--;
        GameManager.instance.spawnReady = true;
        agent.speed = 0;
        //agent.isStopped = true;
        animator.speed = 1;
        if (!isIceBlock){
            animator.SetBool("dead", true);
            yield return new WaitForSeconds(deathAnimationTime);
            Destroy(gameObject); 
        }
        else{
            Destroy(gameObject); 
        }
    }

    public void DestroyMiniMapPointer()
    {
        miniMapPointer.SetActive(false);
    }

    public void AttackTarget()
    {
        attacking = true;
        switch (enemyType)
        {
            case EnemyType.Melee:
                //agent.isStopped = true;
                StartCoroutine(MeleeAttackSequence(1f));
                break;
            case EnemyType.Ranged:

                break;

            case EnemyType.Giant:
                agent.isStopped = true;
                StartCoroutine(SmashAttackSeaquence(3f));
                break;
        }
    }

    bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    IEnumerator RangedAttackSequence(float sequenceTime)
    {
        yield return new WaitForSeconds(sequenceTime);

    }

    IEnumerator MeleeAttackSequence(float sequenceTime)
    {
        animator.SetBool("damage", true);
        yield return new WaitForSeconds(sequenceTime);
        player.GetComponent<PlayerScript>().GetDamage(damage);
    }

    IEnumerator SmashAttackSeaquence(float sequenceTime)
    {
        animator.SetBool("smash", true);
        yield return new WaitForSeconds(sequenceTime);
        if (health > 0)
            player.GetComponent<PlayerScript>().GetDamage(damage);

    }
    public void InstantiateParticleEffect()
    {
        GameObject particles = Instantiate(smashParticles, smashGround.transform.position, Quaternion.identity);
        particles.transform.LookAt(player.transform);
    }

    public void DeathSequenceStarted()
    {
        animator.SetBool("dead", false);
    }
    public void MeleeSequenceStarted()
    {
        animator.SetBool("damage", false);
    }
    public void SmashSequenceStarted()
    {
        animator.SetBool("smash", false);
    }

    public void HsSequenceStarted()
    {
        animator.SetBool("dead", false);
        animator.SetBool("headshot", false);
    }
    public void HitSequenceStarted()
    {
        animator.SetBool("hit", false);
        animator.SetBool("bodyshot", false);
        animator.SetBool("walking", true);

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
