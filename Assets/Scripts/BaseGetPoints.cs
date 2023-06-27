using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Its attached to the base game object
// It should get food from the agent and add it to the score when the agent is in the base
// base should be score counter


public class BaseGetPoints : MonoBehaviour
{
    public GameObject agentPrefab;

    public List<GameObject> agents = new List<GameObject>();
    private GameObject newAgent;
    public int cost = 10;
    public int score;
    public int scoreTotal = 0;

    void Update()
    {
        CheckScore();
        UpdateScoreText();
    }

    //handle collision with agent
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Agent"))
        {
            AgentMovement agent = collision.gameObject.GetComponent<AgentMovement>();
            int fruitCount = agent.GetFoodCount();
            score += fruitCount;
            scoreTotal += fruitCount;
            agent.ResetFoodCount();
        }
    }

    private void CheckScore()
    {
        if(score >= cost)
        {
            newAgent = Instantiate(agentPrefab, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
            agents.Add(newAgent);
            score -= cost;
        }
    }

    private void UpdateScoreText()
    {
        GameObject.Find("TextGUI").GetComponent<UnityEngine.UI.Text>().text = "Current score: " + score + "\nTotal score: " + scoreTotal;
    }
}
