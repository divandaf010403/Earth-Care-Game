using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwnNPC : MonoBehaviour
{
    public static SpwnNPC Instance;
    public GameObject[] NPC_Prefab;
    public int NPC_toSpawn;
    public Transform NpcParent;
    public Transform spawnesPos;
    [SerializeField] GameObject[] spawnedObj;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
        StartCoroutine(SpawnTrashRoutine());

        // Load previously spawned items
        LoadSpawnedItems();
    }

    void Update() {
        
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while (count < NPC_toSpawn && NPC_Prefab.Length > 0)
        {
            int npcIndex = Random.Range(0, NPC_Prefab.Length);
            GameObject obj = Instantiate(NPC_Prefab[npcIndex], NpcParent);

            Transform child = null;
            if (transform.childCount > 0)
            {
                // Lakukan pemilihan acak dua kali
                child = transform.GetChild(Random.Range(0, transform.childCount));
                child = child.GetChild(Random.Range(0, child.childCount));
            }

            if (child != null)
            {
                obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
                obj.transform.localPosition = new Vector3(child.localPosition.x, NpcParent.localPosition.y, child.localPosition.z);
            }

            yield return new WaitForEndOfFrame();
            count++;
        }
    }

    private IEnumerator SpawnTrashRoutine()
    {
        while (true)
        {
            // Tunggu antara 30 hingga 60 detik
            float waitTime = Random.Range(45f, 60f);
            yield return new WaitForSeconds(waitTime);

            // Panggil fungsi ThrowTrashTimer
            if(NpcParent.childCount > 0)
            {
                int randomIndexNPC = Random.Range(0, NpcParent.childCount - 1);
                int randomIndexTrash = Random.Range(0, spawnedObj.Length);

                // Get the sibling index for the spawned item
                int siblingIndex = spawnesPos.childCount;
                
                ThrowTrash throwTrash = NpcParent.GetChild(randomIndexNPC).GetComponent<ThrowTrash>();
                throwTrash.ThrowTrashTimer(spawnedObj[randomIndexTrash], spawnesPos, siblingIndex);
            }
        }
    }

    public void SaveSpawnedItem(GameObject spawnedObject, Transform parent, int siblingIndex)
    {
        string itemId = "Item_" + " " + parent.name + " " + siblingIndex.ToString();
        Vector3 position = spawnedObject.transform.position;

        TrashManager trashManager = spawnedObject.GetComponent<TrashManager>();
        if (trashManager != null && !string.IsNullOrEmpty(trashManager.prefabPath))
        {
            string prefabPath = trashManager.prefabPath;

            TrashItemData itemData = new TrashItemData(itemId, prefabPath, position);
            PlayerPrefs.SetString(itemId, JsonUtility.ToJson(itemData));

            string existingItems = PlayerPrefs.GetString("spawnedItems", "");
            PlayerPrefs.SetString("spawnedItems", existingItems + itemId + ";");

            trashManager.itemId = itemId;
        }
    }

    private void LoadSpawnedItems()
    {
        string[] itemIds = PlayerPrefs.GetString("spawnedItems", "").Split(';');
        foreach (string itemId in itemIds)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                string itemDataJson = PlayerPrefs.GetString(itemId);
                TrashItemData itemData = JsonUtility.FromJson<TrashItemData>(itemDataJson);
                SpawnItem(itemData);
            }
        }
    }

    private void SpawnItem(TrashItemData itemData)
    {
        GameObject itemPrefab = Resources.Load<GameObject>(itemData.prefabPath);
        if (itemPrefab != null)
        {
            GameObject newItem = Instantiate(itemPrefab, itemData.position, Quaternion.identity, spawnesPos);
            TrashManager trashManager = newItem.GetComponent<TrashManager>();
            if (trashManager != null)
            {
                trashManager.itemId = itemData.itemId;
                trashManager.prefabPath = itemData.prefabPath;
                trashManager.CheckItemStatus();
            }
        }
    }
}

[System.Serializable]
public class TrashItemData
{
    public string itemId;
    public string prefabPath;
    public Vector3 position;

    public TrashItemData(string itemId, string prefabPath, Vector3 position)
    {
        this.itemId = itemId;
        this.prefabPath = prefabPath;
        this.position = position;
    }
}