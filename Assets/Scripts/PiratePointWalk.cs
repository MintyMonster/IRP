using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PiratePointWalk : MonoBehaviour
{

    public Transform pointA;
    public Transform pointB;

    private NavMeshAgent agent;
    private Animator animator;
    public Transform currentTarget { get; private set; }
    public bool isInteracting { get; set; } = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentTarget = pointA;
        StartCoroutine(PatrolRoutine());
        agent.speed = 1.4f;
    }

    void Update()
    {
        
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!isInteracting)
            {
                // Set the destination of the agent to the current target
                agent.SetDestination(currentTarget.position);
                // Switch to walking animation
                animator.SetFloat("speed", 1f);
                Debug.Log("Walking to: " + currentTarget.name);

                // Wait until the agent reaches the target
                while (!ReachedDestination())
                {
                    yield return null;
                }

                // Switch to Idle animation
                Debug.Log("Reached: " + currentTarget.name + ". Switching to idle.");
                animator.SetFloat("speed", 0f);
                Debug.Log("Speed set to: " + animator.GetFloat("speed"));

                // Wait for 7 seconds
                yield return new WaitForSeconds(7f);

                // Switch to the next target
                currentTarget = currentTarget == pointA ? pointB : pointA;
                Debug.Log("Next target: " + currentTarget.name);

                // Brief delay to ensure the animator updates correctly before moving again
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                // If interacting, wait a short time before checking again
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    bool ReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /*
    public bool canMove { get; set; } = true;
    public float speed { get; set; }
    private float walkRadius = 40.0f;
    private NavMeshAgent agent;
    private Animator animator;

    private bool WalkingA = false;
    private bool DestinationReached = false;

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        speed = 2f;
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (WalkingA)
                agent.SetDestination(pointA.position);
            else
                agent.SetDestination(pointB.position);


            HandleDestinationReached();

            Debug.Log(WalkingA);
        }
    }

    private void HandleDestinationReached()
    {

        if(agent.remainingDistance <= agent.stoppingDistance)
            if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                DestinationReached = true;
                agent.speed = 0f;
                animator.SetFloat("speed", 1);
                if (DestinationReached)
                {
                    DestinationReached = false;
                    StartCoroutine(WaitAtDestination());
                }
            }
    }

    IEnumerator WaitAtDestination()
    {
        Debug.Log("started");

        yield return new WaitForSeconds(10);

        WalkingA = !WalkingA;
        Debug.Log("reached");
        agent.speed = 2f;
        animator.SetFloat("speed", 0);
        

    }
    */
}
