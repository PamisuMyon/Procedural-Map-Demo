using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Enemy, IDamagable
{
    public MeleeWeapon leftWeapon;
    public MeleeWeapon rightWeapon;

    [Space]
    public float hpMax;
    public float[] damages;

    [Space]
    public float jumpForce;
    public float turningSpeed;
    public float activeAreaRadius = -1;
    public float attackDistance;
    public float attackAngleHalf;
    public float giveUpDistance;
    public float viewAngleHalf = 100;
    public float senseRadius = 4;

    [Space]
    public float idleTime;
    public float patrolTime;
    [Range(0, 1)]
    public float timeRandomness;

    public enum State
    {
        Idling, Patrolling, Assaulting, Hurt, Dead
    }

    [Header("View Only")]
    [SerializeField] State state;
    [SerializeField] float hp;
    [SerializeField] int attackPhase;
    [SerializeField] bool isAttacking;
    [SerializeField] float remainingDistance;
    Transform player;
    Vector3 origin;
    bool jumping;
    float timeCounter;
    Vector3 target = Vector3.negativeInfinity;

    Animator animator;
    // Rigidbody rb;
    NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        // rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        origin = transform.position;
        hp = hpMax;

        if (leftWeapon != null) leftWeapon.owner = transform;
        if (rightWeapon != null) rightWeapon.owner = transform;

        TriggerArea triggerArea = GetComponentInChildren<TriggerArea>();
        // triggerArea.TriggerEnter += OnDetectAreaTriggerEnter;
        triggerArea.TriggerStay += OnDetectAreaTriggerStay;
        triggerArea.TriggerExit += OnDetectAreaTriggerExit;

        SetState(State.Idling);
    }

    void Update()
    {
        if (state == State.Idling)
        {
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0)
                SetState(State.Patrolling);
        }
        else if (state == State.Patrolling)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                RandomPatrolDestination();
            HandleJumping();

            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0)
                SetState(State.Idling);
        }
        else if (state == State.Assaulting)
        {
            if (!isAttacking)
            {
                agent.SetDestination(player.position);
                remainingDistance = agent.remainingDistance;
                if (remainingDistance > giveUpDistance)
                {
                    SetState(State.Patrolling);
                }
                else if (remainingDistance > attackDistance)
                {
                    agent.isStopped = false;
                    agent.updateRotation = true;
                    HandleJumping();
                    attackPhase = 0;
                }
                else
                {
                    // Check if facing player
                    var dir = player.position - transform.position;
                    dir = Vector3.ProjectOnPlane(dir, transform.up);
                    var angle = Vector3.Angle(transform.forward, dir);
                    if (angle > attackAngleHalf)
                    {
                        // Turn to player
                        agent.updateRotation = false;
                        var rot = Quaternion.LookRotation(dir, transform.up);
                        rot = Quaternion.Lerp(transform.rotation, rot, turningSpeed * Time.deltaTime);
                        transform.rotation = rot;                        
                    }
                    else 
                    {
                        agent.isStopped = true;
                        isAttacking = true;
                        string animState = attackPhase == 2 ? "Attack2" : (attackPhase == 1 ? "Attack1" : "Attack0");
                        animator.Play(animState);
                    }
                }
            }
        }

        animator.SetBool("IsWalking", agent.velocity != Vector3.zero);
    }

    void SetState(State newState)
    {
        if (state == newState) return;
        ExitState(state);
        EnterState(newState);
        state = newState;
    }

    void ExitState(State oldState)
    {
        if (oldState == State.Assaulting)
        {
            leftWeapon.IsAttacking = false;
            rightWeapon.IsAttacking = false;
            attackPhase = 0;
            isAttacking = false;
        }
        else if (oldState == State.Hurt)
        {
            agent.isStopped = false;
        }
    }

    void EnterState(State newState)
    {
        if (newState == State.Idling)
        {
            timeCounter = idleTime + Utils.RandomSigned(0, idleTime * timeRandomness);
            jumping = false;
            agent.isStopped = true;
        }
        else if (newState == State.Patrolling)
        {
            timeCounter = patrolTime + Utils.RandomSigned(0, patrolTime * timeRandomness);
            agent.isStopped = false;
        }
        else if (newState == State.Dead)
        {
            agent.isStopped = true;
        }
    }

    void HandleJumping()
    {
        if (agent.isOnOffMeshLink)
        {
            if (!jumping)
            {
                jumping = true;
                // rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            jumping = false;
        }
    }

    void OnDrawGizmos() 
    {
        // if (activeAreaRadius > 0 && origin != Vector3.zero)
        //     Gizmos.DrawWireSphere(origin, activeAreaRadius);
        if (target.x != float.NegativeInfinity)
        {
            Gizmos.DrawSphere(target, .3f);
        }
        // SphereCollider detectCol = GetComponentInChildren<SphereCollider>();
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        // Gizmos.DrawFrustum(Vector3.zero, viewAngleHalf * 2, detectCol.radius, 0, 1);
    }

    void RandomPatrolDestination()
    {
        // if (wayPoints.Length <= 0) return;
        // var index = Random.Range(0, wayPoints.Length);
        // agent.SetDestination(wayPoints[index].position);
        var point = Random.insideUnitCircle * activeAreaRadius;
        target = origin + new Vector3(point.x, transform.position.y, point.y);
        agent.SetDestination(target);
    }

    // void OnDetectAreaTriggerEnter(Collider collider)
    // {
    //     if (collider.gameObject.tag == "Player")
    //     {
    //         if (state == State.Idling || state == State.Patrolling)
    //         {
    //             DetectPlayer(collider.transform);
    //         }
    //     }
    // }

    void OnDetectAreaTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (state == State.Idling || state == State.Patrolling)
            {
                DetectPlayer(collider.transform);
            }
        }
    }

    void OnDetectAreaTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player = null;
            if (state != State.Hurt && state != State.Dead)
                SetState(State.Patrolling);
        }
    }

    void DetectPlayer(Transform player)
    {
        bool detected = false;
        var dir = player.position - transform.position;
        if (dir.sqrMagnitude < senseRadius)
            detected = true;
        else
        {
            dir = Vector3.ProjectOnPlane(dir, transform.up);
            var angle = Vector3.Angle(transform.forward, dir);
            if (angle < viewAngleHalf)
                detected = true;
        }
        if (detected)
        {
            this.player = player;
            if (state != State.Hurt && state != State.Dead)
                SetState(State.Assaulting);
        }
    }

    public void TakeDamage(float value, Transform damageSource = null, Vector3 force = default(Vector3))
    {
        hp -= value;
        if (hp > 0)
        {
            animator.SetTrigger("Hurt");
            agent.isStopped = true;
            SetState(State.Hurt);
        }
        else
        {
            SetState(State.Dead);
        }
    }

    public void OnHurtAnimFinished()
    {
        if (player != null)
        {
            SetState(State.Assaulting);
        }
        else
        {
            SetState(State.Idling);
        }
    }

    public void OnAttackBegin()
    {
        leftWeapon.damage = damages[attackPhase];
        rightWeapon.damage = damages[attackPhase];
        leftWeapon.IsAttacking = true;
        rightWeapon.IsAttacking = true;
    }

    public void OnAttackEnd()
    {
        leftWeapon.IsAttacking = false;
        rightWeapon.IsAttacking = false;
    }

    public void OnAttackFinish()
    {
        attackPhase = ++attackPhase % damages.Length;
        isAttacking = false;
    }

}
