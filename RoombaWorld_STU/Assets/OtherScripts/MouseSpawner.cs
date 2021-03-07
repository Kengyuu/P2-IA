using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSpawner : MonoBehaviour
{
    public float spawnTime = 30;
    public float spawnTimeRandomness = 20;
    float currentSpawnTime;
    float elapsedTime;
    public GameObject mousePrefab;

    GameObject[] spawnPoints;
    GameObject[] patrolPoints;

    private void Start() {
        currentSpawnTime = spawnTime + Random.Range(-spawnTimeRandomness, spawnTimeRandomness);
        spawnPoints = GameObject.FindGameObjectsWithTag("EXIT");
        patrolPoints = GameObject.FindGameObjectsWithTag("PATROLPOINT");
    }
    
    void Update() {
        elapsedTime += Time.deltaTime; 
        if(elapsedTime >= currentSpawnTime) {
            GameObject mouse = Instantiate(mousePrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
            mouse.GetComponent<FSM.FSM_MouseBehaviour>().point = patrolPoints[Random.Range(0, patrolPoints.Length)];
            
            currentSpawnTime = spawnTime + Random.Range(-spawnTimeRandomness, spawnTimeRandomness);
            elapsedTime = 0.0f;
        }
    }
}
