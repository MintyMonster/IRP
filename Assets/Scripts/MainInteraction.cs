using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainInteraction : MonoBehaviour
{
    public bool interacting;
    private Animator animator;
    private NavMeshAgent agent;
    public float interactionDistance = 2f; // Distance within which player can interact
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        // Check for player interaction
        if (Input.GetKeyDown(KeyCode.E)) // Assuming 'E' is the interaction key
        {
            // Check if player is within interaction distance
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player && Vector3.Distance(player.transform.position, transform.position) <= interactionDistance)
            {


                this.GetComponent<PiratePointWalk>().isInteracting = !this.GetComponent<PiratePointWalk>().isInteracting;
                if (this.GetComponent<PiratePointWalk>().isInteracting)
                {
                    // Stop the agent and switch to idle animation
                    agent.isStopped = true;
                    animator.SetFloat("speed", 0f); // Use the same idle animation
                }
                else
                {
                    // Resume patrolling immediately
                    agent.isStopped = false;
                    animator.SetFloat("speed", 1f); // Switch to walking animation
                    agent.SetDestination(this.GetComponent<PiratePointWalk>().currentTarget.position); // Set the current target again
                }
            }
        }
    }
}
