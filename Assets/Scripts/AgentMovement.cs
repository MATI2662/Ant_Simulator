using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Its attached to the agent game object
// It should move the agent to the random position on the map
// After the agent reaches the target position, it should go to the next random position
// make sure that agent is rotated towards moving direction
// agent should collect the food and store it in the inventory (max 5 food items)
// when the inventory is full, agent should go to the base and unload the food




public class AgentMovement : MonoBehaviour
{
    public GameObject fruitPrefab;
    public GameObject basePrefab;
    public int fruitCount;

    
    
    private List<Vector3> seenFruits = new List<Vector3>();
    private Vector3 target;
    NavMeshAgent agent;
    
    public float radius = 5f;
    [Range(0, 360)]public float angle = 45f;
    public LayerMask obstacleLayer;
    public LayerMask foodLayer;

    public bool canSeeFood {get; private set;}

    void Start()
    {
        StartCoroutine(FOVCheck());
        SetAgentPosition();
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        SetAgentPosition();
        UpdateRotationAgent();
    }

    private IEnumerator FOVCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOV();
        }
    }

    private void FOV()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, radius, foodLayer);

        if(rangeCheck.Length > 0)
        {
            Transform food = rangeCheck[0].transform;
            Vector2 directionToFood = (food.position - transform.position).normalized;

            if(Vector2.Angle(transform.up, directionToFood) < angle / 2)
            {
                float distanceToFood = Vector2.Distance(transform.position, food.position);

                if(!Physics2D.Raycast(transform.position, directionToFood, distanceToFood, obstacleLayer))
                {
                    canSeeFood = true;
                    Debug.Log("Can see food");

                    if(!seenFruits.Contains(food.position))
                        seenFruits.Add(food.position);
                    // target = food.position;
                }
                else
                    canSeeFood = false;
            }
            else
                canSeeFood = false;
        }
        else if (canSeeFood)
            canSeeFood = false;
    }

    void SetTargetPosition()
    {
        target = new Vector3(Random.Range(-7, 7), Random.Range(-5, 5), transform.position.z);
    }

    void SetAgentPosition()
    {
        if (fruitCount >= 5)
        {
            agent.SetDestination(basePrefab.transform.position);
            return;
        }

        if (seenFruits.Count > 0)
        {
            target = seenFruits[0];
            agent.SetDestination(target);
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            SetTargetPosition();
        }
        agent.SetDestination(target);
    }

    void UpdateRotationAgent()
    {
        Vector3 direction = agent.velocity.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * 10f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius);

        Vector3 angle01 = DirectionFromAngle(-transform.eulerAngles.z, -angle / 2);
        Vector3 angle02 = DirectionFromAngle(-transform.eulerAngles.z, angle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle01 * radius);
        Gizmos.DrawLine(transform.position, transform.position + angle02 * radius);

        if(canSeeFood)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, fruitPrefab.transform.position);
        }
    }

    private Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // handle collision with food
    // if agent collides with food, it should destroy the food and increase the score

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Food")
        {
            if(fruitCount >= 5)
                return;
            

            //if food is in seenFruits, remove it for all agents

            foreach(GameObject agent in GameObject.Find("Base").GetComponent<BaseGetPoints>().agents)
            {
                if(agent.GetComponent<AgentMovement>().seenFruits.Contains(collision.gameObject.transform.position))
                    agent.GetComponent<AgentMovement>().seenFruits.Remove(collision.gameObject.transform.position);
            }

            
            Destroy(collision.gameObject);
            fruitCount++;
            Debug.Log("Food destroyed");
        }
    }

    public int GetFoodCount()
    {
        return fruitCount;
    }

    public void ResetFoodCount()
    {
        fruitCount = 0;
    }
}
