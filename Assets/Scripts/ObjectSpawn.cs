using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is attached to empty game object
// It should spawn the agent game object at starting position
// It should spawn the fruit game object at mouse position
// Fruit shouldnt be spawned on the obstacle

public class ObjectSpawn : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject fruitPrefab;
    public GameObject wallPrefab;
    private GameObject newAgent;

    void Start()
    {
        newAgent = Instantiate(agentPrefab, new Vector3(5, 0, -1), Quaternion.identity);
        GameObject.Find("Base").GetComponent<BaseGetPoints>().agents.Add(newAgent);
    }

    void Update()
    {
        SpawnFruit();
        SpawnWall();
    }

    void SpawnFruit()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // prevent spawning fruit on obstacle layer
            
            if(FindCollisions(mousePos) < 1)
                Instantiate(fruitPrefab, new Vector3(mousePos.x, mousePos.y, -1), Quaternion.identity);
        }
    }

    void SpawnWall()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(wallPrefab, new Vector3(mousePos.x, mousePos.y, -1), Quaternion.identity);
        }
    }

    private int FindCollisions(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.12f);
        return hits.Length;
        
    }
}
