using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

        agent.stoppingDistance = stopDistance;

        playerStutus.OnPlayerDeath += OnPlayerDied;

    }
    private void OnDestroy()
    {
        playerStutus.OnPlayerDeath -= OnPlayerDied;
    }
    private void OnPlayerDied()
    {
        playerIsDead = true;
        animator.SetBool("run", false);
        animator.SetTrigger("win");
        agent.isStopped = true;
        agent.ResetPath();
    }
    // Update is called once per frame
    void Update()
    {
     
            // Don't execute AI if health is zero or below
            if (GetComponent<enemyHealth>().currentHealth <= 0 || playerIsDead) return;

            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        
            if (isWaittingForWalkPoint)
        {
            if (Time.time - walkPointTimer > 5f)
            {
                isWaittingForWalkPoint = false;
                walkPointSet = false;
                SearchWalkPoint();
            }
        }
    }

    private void Patroling()
    {
        if (gameObject.CompareTag("wall"))
        {
            Debug.Log("Rotated object after collision with wall!");
            // Rotate the current object by 90 degrees on the Y-axis
            transform.Rotate(0, 90, 0);

        }
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
      
            // Check if the collided object has the wall tag
        
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
    private void ChasePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        directionToPlayer.y = 0;

        Vector3 targetPositon = player.position - directionToPlayer.normalized * stopDistance;

        agent.SetDestination(targetPositon);
        animator.SetBool("run", true);
    }
    private void AttackPlayer()

    {

        agent.SetDestination(transform.position);

        Vector3 lookATtPosition = new Vector3(player.position.x, transform.position.y, player.position.z);

        transform.LookAt(lookATtPosition);

        float currentDistance = Vector3.Distance(transform.position, player.position);

        if (currentDistance < stopDistance * 2f)
        {
            Vector3 pushDirection = (transform.position - player.position).normalized;
            transform.position += pushDirection * Time.deltaTime;
        }

        if (!alreadyAttacked)
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

        animator.SetBool("run", false);


      


    //agent.SetDestination(transform.position);

    //transform.LookAt(player);


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
