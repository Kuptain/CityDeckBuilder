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
    public float velocityThreshhold;
    public Vector2 wanderInterval;
    private float wanderTimer;
    private float currentWanderInterval;
   

    public List<GameObject> bodyVariants;
    GameObject body;
    Animator animator;
    SpriteRenderer render;


    public void Start()
    {
        currentWanderInterval = 0;

        foreach (GameObject variant in bodyVariants)
        {
            variant.SetActive(false);
        }

        if (bodyVariants.Count > 0)
        {
            int randomIndex = Random.Range(0, bodyVariants.Count);
            bodyVariants[randomIndex].SetActive(true);
            body = bodyVariants[randomIndex];
        }
        body.transform.parent = null;

        agent = GetComponent<NavMeshAgent>();
        animator = body.GetComponentInChildren<Animator>();
        render = body.GetComponentInChildren<SpriteRenderer>();

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

        if(agent.velocity.x > velocityThreshhold || agent.velocity.x < -velocityThreshhold || agent.velocity.z > velocityThreshhold || agent.velocity.z < -velocityThreshhold)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        if (agent.velocity.x > velocityThreshhold)
        {
            body.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (agent.velocity.x < -velocityThreshhold)
        {
            body.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        body.transform.position = transform.position;
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
