using UnityEngine;

public class ClickSpawn : MonoBehaviour {

    private Camera cam;
    private GameObject pooPrefab;
    private GameObject dustPrefab;
    private GameObject mousePrefab;

    GameObject[] patrolPoints;    

	void Start () {
        cam = Camera.main;
        pooPrefab = Resources.Load<GameObject>("POO");
        dustPrefab = Resources.Load<GameObject>("DUST");
        mousePrefab = Resources.Load<GameObject>("MOUSE");
        patrolPoints = GameObject.FindGameObjectsWithTag("PATROLPOINT");
	}
	
	void Update () {
        Vector3 position;

        if (Input.GetMouseButtonDown(0)) {
            position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject dust = GameObject.Instantiate(dustPrefab);
            dust.transform.position = position;
        }

        if (Input.GetMouseButtonDown(1)) {
            position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject poo = GameObject.Instantiate(pooPrefab);
            poo.transform.position = position;
        }

        if (Input.GetMouseButtonDown(2)) {
            position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject mouse = GameObject.Instantiate(mousePrefab);
            mouse.GetComponent<FSM.FSM_MouseBehaviour>().point = patrolPoints[Random.Range(0, patrolPoints.Length)];
            mouse.transform.position = position;
        }

    }
}
