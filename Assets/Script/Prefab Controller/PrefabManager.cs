using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [SerializeField] public string quest_id = "5Q";
    public GameObject prefab; // Referensi ke prefab yang ingin diinstansiasi
    private GameObject prefabInstance;
    public PrefabStatus prefabStatus = new PrefabStatus();
    private Dictionary<string, GameObject> objectMap = new Dictionary<string, GameObject>(); // Gunakan itemId sebagai kunci
    [SerializeField] int inactiveCount = 0;

    void Awake()
    {
        // Instansiasi prefab
        prefabInstance = Instantiate(prefab, transform);

        // Populate objectMap with all child objects and set their layer to Default (layer 0)
        foreach (Transform child in prefabInstance.GetComponentsInChildren<Transform>(true))
        {
            if (child != prefabInstance.transform) // Exclude the parent object itself
            {
                TrashManager trashManager = child.GetComponent<TrashManager>();
                if (trashManager != null)
                {
                    string itemId = trashManager.itemId;
                    objectMap[itemId] = child.gameObject;
                    child.gameObject.layer = 6; // Set layer to Default (layer 6)
                }
                else
                {
                    Debug.Log("Trash Manager tidak ada");
                }
            }
        }
    }

    private void Start() 
    {
        LoadData();
        
        if (AreAllItemsInactiveInPrefab())
        {
            PerformActionAfterThreshold();
        }
    }

    void Update()
    {
        
    }

    public void UpdateStatus(string itemId, bool isActive)
    {
        var status = prefabStatus.objectStatuses.Find(x => x.objectName == itemId);
        if (status != null)
        {
            status.isActive = isActive;
        }
        else
        {
            prefabStatus.objectStatuses.Add(new ObjectStatus { objectName = itemId, isActive = isActive });
            inactiveCount++;
        }

        // Update the actual object in the scene
        if (objectMap.ContainsKey(itemId))
        {
            objectMap[itemId].SetActive(isActive);
        }

        // Check if all items are inactive
        if (AreAllItemsInactiveInPrefab())
        {
            PerformActionAfterThreshold();
        }

        SaveData();
    }

    private bool AreAllItemsInactiveInPrefab()
    {
        Debug.Log(prefabInstance.transform.childCount + " == " + inactiveCount);
        if (prefabInstance.transform.childCount == inactiveCount)
        {
            return true;
        }
        return false;
    }

    private void PerformActionAfterThreshold()
    {
        // Perform some action after 10 items are taken (SetActive(false))
        Debug.Log("Threshold reached! Performing an action.");
        QuestController.Instance.getChildNumberNextQuest(transform);
        // Add your custom action here
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(prefabStatus);
        PlayerPrefs.SetString("PrefabStatus", json);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("PrefabStatus"))
        {
            string json = PlayerPrefs.GetString("PrefabStatus");
            prefabStatus = JsonUtility.FromJson<PrefabStatus>(json);

            inactiveCount = 0;
            foreach (var status in prefabStatus.objectStatuses)
            {
                if (objectMap.ContainsKey(status.objectName))
                {
                    objectMap[status.objectName].SetActive(status.isActive);
                    if (!status.isActive)
                    {
                        inactiveCount++;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class ObjectStatus
{
    public string objectName; // Unik ID untuk setiap objek
    public bool isActive;
}

[System.Serializable]
public class PrefabStatus
{
    public List<ObjectStatus> objectStatuses = new List<ObjectStatus>();
}