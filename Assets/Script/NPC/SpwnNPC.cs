using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwnNPC : MonoBehaviour
{
    public GameObject NPC_Prefab;
    public int NPC_toSpawn;
    public Transform NpcParent;

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
            obj.transform.SetParent(NpcParent); // Set the parent
            obj.transform.localPosition = new Vector3(child.localPosition.x, NpcParent.localPosition.y, child.localPosition.z);
            Debug.Log(obj.transform.localPosition);

            yield return new WaitForEndOfFrame();
            count++;
        }
    }
}
