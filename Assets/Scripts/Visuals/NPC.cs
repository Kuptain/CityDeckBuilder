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
    public float velocityThreshold;
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

        float animSpeedMultiplier = 0.75f;

        // Ignore vertical movement and get horizontal speed
        Vector3 horizontalVelocity = new Vector3(agent.velocity.x, 0f, agent.velocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed > velocityThreshold)
        {
            animator.SetBool("isWalking", true);

            float animSpeed = speed * animSpeedMultiplier;

            // Optional: prevent extreme animation speeds
            animSpeed = Mathf.Clamp(animSpeed, 0.6f, 2.0f);

            animator.speed = animSpeed;
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.speed = 1f; // Reset to normal speed when idle
        }

        if (agent.velocity.x > velocityThreshold)
        {
            body.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (agent.velocity.x < - velocityThreshold)
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
