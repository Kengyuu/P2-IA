using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustSpawner : MonoBehaviour
{
    public float dustTime = 5.0f;
    private float currentDustTime = 0.0f;
    public GameObject dust;

    
    void Update()
    {
        currentDustTime += Time.deltaTime; 
        if(currentDustTime >= dustTime)
        {
            Instantiate(dust);
            dust.transform.position = RandomLocationGenerator.RandomPatrolLocation();
            dust.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
            currentDustTime = 0.0f;
        }
    }
}
