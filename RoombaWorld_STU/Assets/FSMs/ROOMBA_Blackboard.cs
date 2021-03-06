
using System.Collections.Generic;
using UnityEngine;

public class ROOMBA_Blackboard : MonoBehaviour {

    public float closeDustDetectionRadius = 30;  // radius for dust detection   
    public float farDustDetectionRadius = 100;   // radius for random dust detection
    public float closePooDetectionRadius = 75;   // radius for poo detection    
    public float farPooDetectionRadius = 150;    // radius for random poo detection

    public float dustReachedRadius = 5; // reachability radius
    public float pooReachedRadius = 5;  // reachability radius
    public float chargingStationReachedRadius = 2;  // reachability radius

    public float energyConsumptionPerSecond = 10;    
    public float energyRechargePerSecond = 15;
    public float minCharge = 15;    // min threshold. If currentCharge is below this figure go to recharging station    
    public float maxCharge = 99;    // max threshold. Leave charging station if currentCharge reaches this level

    public float currentCharge = 100;

    private TextMesh energyLine;
    public List<GameObject> energyStations = new List<GameObject>();
    
    public List<GameObject> memory; // list of detected dust units not picked due to presence of poo
    private GameObject[] wanderPoints; 
	
	void Start () {
        memory = new List<GameObject>();
        energyLine = GameObject.Find("EnergyLine").GetComponent<TextMesh>();
        wanderPoints = GameObject.FindGameObjectsWithTag("PATROLPOINT"); 
	}
	
	void Update () {
        Discharge(Time.deltaTime);
        energyLine.text = "Charge: " + Mathf.RoundToInt(currentCharge);
    }

    public GameObject GetRandomWanderPoint()
    {
        return wanderPoints[Random.Range(0, wanderPoints.Length)]; 
    }
    // invoke this method while in charging station
    public void Recharge (float deltaTime)
    {
        currentCharge = currentCharge + deltaTime * energyRechargePerSecond;
        if (currentCharge > 100) currentCharge = 100;
    }

    // invoked by Update to subtract energy. 
    private void Discharge (float deltaTime)
    {
        currentCharge = currentCharge - deltaTime * energyConsumptionPerSecond;
        if (currentCharge < 0) currentCharge = 0;
    }

    // invoke to memorize detected but unattended dust particles
    public void AddToMemory (GameObject gm)
    {
        if (!memory.Contains(gm)) memory.Add(gm);
    }

    // get a dust particle from memory (and remove it). Retrieval follows a FIFO policy
    public GameObject RetrieveFromMemory ()
    {
        if (memory.Count == 0) return null;
        else
        {
            GameObject result = memory[0];
            memory.RemoveAt(0);
            return result;
        }
    }

    //Get closest energy station
    public GameObject GetClosestEnergyStation(GameObject me){
        GameObject closest = energyStations[0];
        foreach(GameObject g in energyStations){
            if(SensingUtils.DistanceToTarget(me, g) > SensingUtils.DistanceToTarget(me, closest))
                closest = g;
        }
        return closest;
    }
}
