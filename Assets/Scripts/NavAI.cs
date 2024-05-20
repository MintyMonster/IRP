using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAI : MonoBehaviour
{

    public bool canMove { get; set; } = true;
    [SerializeField] public float speed { get; set; }
    private float walkRadius = 40.0f;
    private NavMeshAgent agent;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        speed = 1f;
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        {
            agent.speed = speed;
            HandleDestinationReached();
        }
    }

    private void HandleRandomRoam()
        => agent.SetDestination(GetRandomPosition());

    private void HandleDestinationReached()
    {
        if (!agent.pathPending)
            if (agent.remainingDistance <= agent.stoppingDistance)
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    HandleRandomRoam();
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPos = Vector3.zero;

        if(NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
            finalPos = hit.position;

        return finalPos;
    }
}
