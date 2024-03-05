using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwnNPC : MonoBehaviour
{
    public GameObject NPC_Prefab;
    public int NPC_toSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while (count < NPC_toSpawn)
        {
            GameObject obj = Instantiate(NPC_Prefab);
            Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.localPosition = child.position;

            yield return new WaitForEndOfFrame();
            count++;
        }
    }
}
