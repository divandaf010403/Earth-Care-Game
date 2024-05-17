using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwnNPC : MonoBehaviour
{
    public GameObject[] NPC_Prefab;
    public int NPC_toSpawn;
    public Transform NpcParent;
    public Transform spawnesPos;
    [SerializeField] GameObject[] spawnedObj;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
        StartCoroutine(SpawnTrashRoutine());
    }

    void Update() {
        
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while (count < NPC_toSpawn)
        {
            int npcIndex = Random.Range(0, NPC_Prefab.Length);
            GameObject obj = Instantiate(NPC_Prefab[npcIndex]);
            Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.SetParent(NpcParent); // Set the parent
            obj.transform.localPosition = new Vector3(child.localPosition.x, NpcParent.localPosition.y, child.localPosition.z);

            yield return new WaitForEndOfFrame();
            count++;
        }
    }

    private IEnumerator SpawnTrashRoutine()
    {
        while (true)
        {
            // Tunggu antara 30 hingga 60 detik
            float waitTime = Random.Range(30f, 60f);
            yield return new WaitForSeconds(waitTime);

            // Panggil fungsi ThrowTrashTimer
            if(NpcParent.childCount > 0)
            {
                int randomIndexNPC = Random.Range(0, NpcParent.childCount - 1);
                int randomIndexTrash = Random.Range(0, spawnedObj.Length);
                
                ThrowTrash throwTrash = NpcParent.GetChild(randomIndexNPC).GetComponent<ThrowTrash>();
                throwTrash.ThrowTrashTimer(spawnedObj[randomIndexTrash], spawnesPos);
            }
        }
    }
}
