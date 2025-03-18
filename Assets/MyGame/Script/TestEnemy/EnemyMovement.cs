using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public EnemyLineOfSightChecker lineOfSightChecker;

    public NavMeshTriangulation Triangulation;
    public float updateRate = 0.1f;
    private NavMeshAgent agent;
    private Animator animator;
    private Coroutine followCoroutine;

    public EnemyState defautState;
    private EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            onStateChange?.Invoke(_state, value);
            _state = value;
        }
    }
    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent onStateChange;

    public float idleLocationRadius = 4f;
    public float idleMoveSpeedMultiplier = 0.5f;
    [SerializeField] private int wayPointIndex = 0;
    public Vector3[] wayPoint = new Vector3[4];

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        lineOfSightChecker.onGainEvent += HandleGainSight;
        lineOfSightChecker.onLossEvent += HandleLoseSight;

        onStateChange += HandleStateChange;
    }

    private void HandleLoseSight(PlayerController player)
    {
        
        State = defautState;
    }

    private void HandleGainSight(PlayerController player)
    {
        State = EnemyState.Chase;
    }

    private void OnDisable()
    {
        _state = defautState;
    }

    public void Spawn()
    {
        for (int i = 0; i < wayPoint.Length; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out hit, 2f, agent.areaMask))
            {
                wayPoint[i] = hit.position;
            }
            else Debug.LogError("unable to find pos for navmesh near Triangulation vertex");
        }
        onStateChange?.Invoke(EnemyState.Spawn, defautState);
    }

    private void Update()
    {
        animator.SetFloat("Speed", 1f);
    }

    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            if (followCoroutine != null)
            {
                StopCoroutine(followCoroutine);
            }
            if (oldState == EnemyState.Idle)
            {
                agent.speed /= idleMoveSpeedMultiplier;
            }

            switch (newState)
            {
                case EnemyState.Idle:
                    followCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case EnemyState.Patrol:
                    followCoroutine = StartCoroutine(DoPatrolMotion());
                    break;
                case EnemyState.Chase:
                    followCoroutine = StartCoroutine(FollowTarget());
                    break;
            }
        }
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);
        agent.speed *= idleMoveSpeedMultiplier;

        while (true)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                yield return wait;
            }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector2 point = UnityEngine.Random.insideUnitCircle * idleLocationRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, agent.areaMask))
                {
                    agent.SetDestination(hit.position);
                }

            }

            yield return wait;
        }
    }
    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        yield return new WaitUntil(() => agent.enabled && agent.isOnNavMesh);
        agent.SetDestination(wayPoint[wayPointIndex]);
        while (true)
        {
            if (agent.isOnNavMesh && agent.enabled && agent.remainingDistance <= agent.stoppingDistance)
            {
                wayPointIndex++;
                if (wayPointIndex >= wayPoint.Length)
                {
                    wayPointIndex = 0;
                }
                agent.SetDestination(wayPoint[wayPointIndex]);
            }

            yield return wait;
        }
    }
    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);
        while (gameObject.activeSelf)
        {
            if (agent.enabled)
            {
                agent.SetDestination(player.transform.position);
            }
            yield return wait;
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < wayPoint.Length; i++)
        {
            Gizmos.DrawSphere(wayPoint[i], 0.25f);
            if(i + 1 < wayPoint.Length)
            {
                Gizmos.DrawLine(wayPoint[i], wayPoint[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(wayPoint[i], wayPoint[0]);
            }
        }
    }
}
