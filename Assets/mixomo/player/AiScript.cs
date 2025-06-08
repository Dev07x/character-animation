using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AiScript : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float stopDistance = 1.5f;
    private bool shouldDoJumpAttack = false;
    private bool playerIsDead = false;

    private float walkPointTimer;
    private bool isWaittingForWalkPoint;

    // Performance optimization variables
    [Header("Performance Settings")]
    public float detectionCheckInterval = 0.15f;
    public float behaviorUpdateInterval = 0.1f;

    // Cached components and values
    private enemyHealth healthComponent;
    private float lastBehaviorUpdate = 0f;
    private AIState currentState = AIState.Patrolling;
    private float sqrStopDistance; // Cached squared distance
    private float sqrSightRange;   // Cached squared distance
    private float sqrAttackRange;  // Cached squared distance

    // Cached vectors to reduce allocations
    private Vector3 cachedPlayerPos;
    private Vector3 cachedMyPos;

    private enum AIState
    {
        Patrolling,
        Chasing,
        Attacking,
        Dead
    }

    private void Start()
    {
        GameObject playerObject = GameObject.Find("Defeated 1");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("GameObject 'player' not found!");
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<enemyHealth>(); // Cache the component
        agent.stoppingDistance = stopDistance;

        // Cache squared distances (much faster than using magnitude)
        sqrStopDistance = stopDistance * stopDistance;
        sqrSightRange = sightRange * sightRange;
        sqrAttackRange = attackRange * attackRange;

        playerStutus.OnPlayerDeath += OnPlayerDied;

        // Start optimized detection
        StartCoroutine(OptimizedDetectionCoroutine());
    }

    private void OnDestroy()
    {
        playerStutus.OnPlayerDeath -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        playerIsDead = true;
        currentState = AIState.Dead;
        animator.SetBool("run", false);
        animator.SetTrigger("win");
        agent.isStopped = true;
        agent.ResetPath();
    }

    // Optimized detection with early exits
    private IEnumerator OptimizedDetectionCoroutine()
    {
        while (true)
        {
            // Early exit if dead
            if (currentState == AIState.Dead || healthComponent.currentHealth <= 0)
            {
                yield return new WaitForSeconds(1f); // Check less frequently when dead
                continue;
            }

            // Cache positions once
            cachedMyPos = transform.position;
            cachedPlayerPos = player.position;

            // Use squared distance instead of Physics.CheckSphere for better performance
            float sqrDistanceToPlayer = (cachedPlayerPos - cachedMyPos).sqrMagnitude;

            // Update detection flags based on squared distances
            playerInSightRange = sqrDistanceToPlayer <= sqrSightRange;
            playerInAttackRange = sqrDistanceToPlayer <= sqrAttackRange;

            yield return new WaitForSeconds(detectionCheckInterval);
        }
    }

    void Update()
    {
        // Early exit if dead - most important optimization
        if (currentState == AIState.Dead || healthComponent.currentHealth <= 0)
        {
            if (currentState != AIState.Dead)
            {
                currentState = AIState.Dead;
                OnPlayerDied();
            }
            return;
        }

        // Update behavior less frequently
        if (Time.time - lastBehaviorUpdate >= behaviorUpdateInterval)
        {
            UpdateAIBehavior();
            lastBehaviorUpdate = Time.time;
        }

        // Handle walk point timeout (this needs to run more frequently)
        if (isWaittingForWalkPoint && Time.time - walkPointTimer > 5f)
        {
            isWaittingForWalkPoint = false;
            walkPointSet = false;
        }
    }

    private void UpdateAIBehavior()
    {
        // Determine state based on detection
        AIState newState = AIState.Patrolling;

        if (playerInSightRange && playerInAttackRange)
            newState = AIState.Attacking;
        else if (playerInSightRange)
            newState = AIState.Chasing;

        // Only change behavior if state actually changed
        if (newState != currentState)
        {
            currentState = newState;

            switch (currentState)
            {
                case AIState.Patrolling:
                    StartPatroling();
                    break;
                case AIState.Chasing:
                    StartChasing();
                    break;
                case AIState.Attacking:
                    StartAttacking();
                    break;
            }
        }

        // Continue current behavior
        switch (currentState)
        {
            case AIState.Patrolling:
                ContinuePatroling();
                break;
            case AIState.Chasing:
                ContinueChasing();
                break;
            case AIState.Attacking:
                ContinueAttacking();
                break;
        }
    }

    private void StartPatroling()
    {
        if (!walkPointSet) SearchWalkPoint();
    }

    private void ContinuePatroling()
    {
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            // Use squared distance comparison
            if ((transform.position - walkPoint).sqrMagnitude < 1f)
            {
                walkPointSet = false;
            }
        }
        else
        {
            SearchWalkPoint();
        }
    }

    private void StartChasing()
    {
        animator.SetBool("run", true);
    }

    private void ContinueChasing()
    {
        Vector3 directionToPlayer = cachedPlayerPos - transform.position;
        directionToPlayer.y = 0;
        Vector3 targetPosition = cachedPlayerPos - directionToPlayer.normalized * stopDistance;
        agent.SetDestination(targetPosition);
    }

    private void StartAttacking()
    {
        agent.SetDestination(transform.position);
        animator.SetBool("run", false);
    }

    private void ContinueAttacking()
    {
        // Look at player (only update rotation during attack)
        Vector3 lookAtPosition = new Vector3(cachedPlayerPos.x, transform.position.y, cachedPlayerPos.z);
        transform.LookAt(lookAtPosition);

        // Push away if too close - use squared distance
        float sqrCurrentDistance = (cachedPlayerPos - transform.position).sqrMagnitude;
        if (sqrCurrentDistance < sqrStopDistance * 4f) // stopDistance * 2 squared
        {
            Vector3 pushDirection = (transform.position - cachedPlayerPos).normalized;
            transform.position += pushDirection * Time.deltaTime;
        }

        // Attack logic
        if (!alreadyAttacked)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (!shouldDoJumpAttack)
        {
            animator.SetTrigger("jumpAttack");
            shouldDoJumpAttack = true;
        }
        else
        {
            animator.SetTrigger("fffAttack");
        }

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void SearchWalkPoint()
    {
        animator.SetBool("run", true);

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
            walkPointTimer = Time.time;
            isWaittingForWalkPoint = true;
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void OnReachedWalkPoint()
    {
        isWaittingForWalkPoint = false;
    }
}