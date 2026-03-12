using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class NPC : MonoBehaviour
{
    [HideInInspector] public int ID;
    private NavMeshAgent agent;

    [Header("Wandering")]
    [HideInInspector] public Vector3 originPosition;
    public float wanderRadius = 10f;
    public Vector2 wanderInterval;
    private float wanderTimer;
    private float currentWanderInterval;


    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        currentWanderInterval = Random.Range(wanderInterval.x, wanderInterval.y);
    }
    void Update()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= currentWanderInterval)
        {
            Vector3 newPos = GetRandomNavmeshPosition(originPosition, wanderRadius);
            agent.SetDestination(newPos);

            currentWanderInterval = Random.Range(wanderInterval.x, wanderInterval.y);
            wanderTimer = 0;
        }
    }
    Vector3 GetRandomNavmeshPosition(Vector3 origin, float distance)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * distance;
            randomDir += origin;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }
        return origin;
    }
}
