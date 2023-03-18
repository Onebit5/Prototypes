using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 4f;
    public float chaseSpeed = 7f;
    public float chaseRange = 15f;
    public float fov = 180f;
    public float hearingRange = 30f;
    public float minWaitTime = 0.5f;
    public float maxWaitTime = 1.5f;

    private NavMeshAgent navMeshAgent;
    private Transform player;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool waiting = false;
    private float waitTimer = 0f;
    private float waitTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSeePlayer())
        {
            isChasing = true;
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.SetDestination(player.position);
            Debug.Log("CanSeePlayer called");
        }
        else if (isChasing)
        {
            isChasing = false;
            navMeshAgent.speed = patrolSpeed;
            waiting = true;
            waitTime = Random.Range(minWaitTime, maxWaitTime);
            Patrol();
            Debug.Log("isChasing called");
        }
        else if (!waiting)
        {
            Patrol();
            Debug.Log("Patrolling called");
        }
        else
        {
            Debug.Log("waiting else called");
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waiting = false;
                waitTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
                Debug.Log("waiting timer called");
            }
        }
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.1f)
        {
            int attempts = 0;
            while (attempts < patrolPoints.Length)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) > 0.1f)
                {
                    navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
                    waiting = true;
                    waitTime = Random.Range(minWaitTime, maxWaitTime);
                    Debug.Log("Patrolling to point " + (currentPatrolIndex + 1) + " of " + patrolPoints.Length);
                    break;
                }
                attempts++;
            }
            if (attempts >= patrolPoints.Length)
            {
                Debug.Log("Unable to find valid patrol point.");
            }
        }
        else
        {
            navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle <= fov / 2)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, chaseRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        if (Vector3.Distance(transform.position, player.position) <= hearingRange && player.GetComponent<AudioSource>().isPlaying)
        {
            return true;
        }

        return false;
    }
}