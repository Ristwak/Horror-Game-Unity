using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform[] waypoints;
    public float idleTime = 2f;
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;
    public float sightDistance = 10f;
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private float idleTimer = 0f;
    private Transform player;
    private AudioSource audioSource;

    private enum EnemyState { Idle, Walk, Chase }
    private EnemyState currentState = EnemyState.Idle;

    private bool isChasingAnimation = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        if (waypoints != null && waypoints.Length > 0)
            SetDestinationToWaypoint();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                idleTimer += Time.deltaTime;

                if (animator != null)
                {
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsChasing", false);
                }

                PlaySound(idleSound);

                if (waypoints != null && waypoints.Length > 0 && idleTimer >= idleTime)
                    NextWaypoint();

                CheckForPlayerDetection();
                break;

            case EnemyState.Walk:
                idleTimer = 0f;

                if (animator != null)
                {
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsChasing", false);
                }

                PlaySound(walkingSound);

                if (agent != null && agent.isOnNavMesh &&
                    !agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.Idle;
                }

                CheckForPlayerDetection();
                break;

            case EnemyState.Chase:
                idleTimer = 0f;

                if (player == null)
                {
                    ReturnToPatrol();
                    break;
                }

                if (agent != null && agent.isOnNavMesh)
                {
                    agent.speed = chaseSpeed;

                    if (!agent.hasPath ||
                        Vector3.Distance(agent.destination, player.position) > 0.25f)
                    {
                        agent.SetDestination(player.position);
                    }
                }

                isChasingAnimation = true;

                if (animator != null)
                    animator.SetBool("IsChasing", true);

                PlaySound(chasingSound);

                if (Vector3.Distance(transform.position, player.position) > sightDistance)
                    ReturnToPatrol();

                break;
        }
    }

    private void CheckForPlayerDetection()
    {
        if (player == null)
            return;

        Vector3 playerDirection = player.position - transform.position;

        if (playerDirection.sqrMagnitude > sightDistance * sightDistance)
            return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, playerDirection.normalized,
            out hit, sightDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                currentState = EnemyState.Chase;
                isChasingAnimation = true;
                Debug.Log("Player detected!");
            }
        }
    }

    private void PlaySound(AudioClip soundClip)
    {
        if (audioSource == null || soundClip == null)
            return;

        if (!audioSource.isPlaying || audioSource.clip != soundClip)
        {
            audioSource.clip = soundClip;
            audioSource.Play();
        }
    }

    private void NextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (waypoints[currentWaypointIndex] == null)
            return;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            agent.speed = walkSpeed;
        }

        currentState = EnemyState.Walk;

        if (animator != null)
            animator.enabled = true;
    }

    private void ReturnToPatrol()
    {
        isChasingAnimation = false;

        if (animator != null)
            animator.SetBool("IsChasing", false);

        if (agent != null && agent.isOnNavMesh)
            agent.speed = walkSpeed;

        if (waypoints != null && waypoints.Length > 0)
            SetDestinationToWaypoint();
        else
            currentState = EnemyState.Idle;
    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}