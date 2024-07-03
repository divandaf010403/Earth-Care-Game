using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [SerializeField] public string quest_id = "5Q";
    public GameObject prefab; // Referensi ke prefab yang ingin diinstansiasi
    private GameObject prefabInstance;
    public PrefabStatus prefabStatus = new PrefabStatus();
    private Dictionary<string, GameObject> objectMap = new Dictionary<string, GameObject>();
    private int destroyedCount = 0;
    private const int updateThreshold = 10;

    void Awake()
    {
        // Instansiasi prefab
        prefabInstance = Instantiate(prefab, transform);

        for (int i = 0; i < prefabInstance.transform.childCount; i++)
        {
            prefabInstance.transform.GetChild(i).gameObject.layer = 6;
        }

        // Populate objectMap with all child objects
        foreach (Transform child in prefabInstance.GetComponentsInChildren<Transform>(true))
        {
            if (child != prefabInstance.transform) // Exclude the parent object itself
            {
                objectMap[child.gameObject.name] = child.gameObject;
            }
        }

        LoadData();
    }

    void Update()
    {

    }

    public void UpdateStatus(string objectName, bool isDestroyed)
    {
        var status = prefabStatus.objectStatuses.Find(x => x.objectName == objectName);
        if (status != null)
        {
            status.isDestroyed = isDestroyed;
        }
        else
        {
            prefabStatus.objectStatuses.Add(new ObjectStatus { objectName = objectName, isDestroyed = isDestroyed });
        }

        // Update the actual object in the scene
        if (objectMap.ContainsKey(objectName))
        {
            objectMap[objectName].SetActive(!isDestroyed);
        }

        // Update destroyed count
        if (isDestroyed)
        {
            destroyedCount++;
            if (destroyedCount >= updateThreshold)
            {
                SaveData();
                destroyedCount = 0;
                QuestController.Instance.getChildNumberNextQuest(transform);
            }
        }
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

            foreach (var status in prefabStatus.objectStatuses)
            {
                if (objectMap.ContainsKey(status.objectName))
                {
                    objectMap[status.objectName].SetActive(!status.isDestroyed);
                }
            }
        }
    }
}

[System.Serializable]
public class ObjectStatus
{
    public string objectName;
    public bool isDestroyed;
}

[System.Serializable]
public class PrefabStatus
{
    public List<ObjectStatus> objectStatuses = new List<ObjectStatus>();
}