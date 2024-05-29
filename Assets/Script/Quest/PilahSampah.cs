using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PilahSampah : MonoBehaviour, IQuestHandler
{
    [SerializeField] public string quest_id = "1Q";
    [SerializeField] public Camera questCamera;
    [SerializeField] public Transform questPlayerPosition;
    [SerializeField] public Transform questCameraPosition;
    public Transform isActiveTrigger;
    public List<GameObject> colliderQuest;

    [Header("Component")]
    public GameController gc;
    public MainCharMovement mainController;
    public Interactions interactions;
    public Transform inventoryQuest;
    public TextMeshProUGUI sisaSampahTxt;

    // Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform QuestCameraPosition => questCameraPosition;
    public Transform IsActiveTrigger => isActiveTrigger;

    [Header("Spawner")]
    public List<GameObject> objectToSpawn;
    public Transform spawnerMidPosition;
    public float spawnRadius = 5f;
    public int numberOfObjectsToSpawn = 10;

    [Header("Next Action")]
    [SerializeField] private GameObject questToActive;

    private void Update() 
    {
        if (GameVariable.isQuestStarting && (GameVariable.questId == quest_id))
        {
            if (inventoryQuest.GetChild(0).GetComponent<Image>().enabled == false)
            {
                sisaSampahTxt.text = "Sisa : " + spawnerMidPosition.childCount;
            }

            if (spawnerMidPosition.childCount == 0 && inventoryQuest.GetChild(0).GetComponent<Image>().enabled == false)
            {
                Selesai_Misi();
                questToActive.SetActive(true);
            }
        }

        isActiveTrigger.gameObject.SetActive(GameVariable.isQuestStarting ? false : true);
    }

    public void Mulai_Misi()
{
    // Save the old position and rotation
    interactions.oldPosition = gc.mainCharacter.transform.position;
    interactions.oldRotation = new Vector3(0f, gc.mainCharacter.transform.eulerAngles.y, 0f);

    // Disable the character controller and set new position and rotation
    mainController.controller.enabled = false;
    gc.mainCharacter.transform.position = interactions.newPosition;
    gc.mainCharacter.transform.rotation = Quaternion.Euler(interactions.newRotation);
    gc.camera2.transform.position = interactions.cameraSetPosition;
    gc.camera2.transform.rotation = Quaternion.Euler(interactions.cameraSetRotation);
    mainController.controller.enabled = true;

    // Switch cameras
    gc.mainCamera.gameObject.SetActive(false);
    gc.camera2.gameObject.SetActive(true);

    // Update main controller's camera reference
    mainController.playerCamera = gc.camera2;

    // Set game variables
    GameVariable.isQuestStarting = true;
    GameVariable.questId = quest_id;

    // Update UI
    gc.mainUI.SetActive(false);
    gc.pilahSampahUI.SetActive(true);

    // Spawn new objects
    SpawnObjects();

    // Activate quest colliders
    foreach (GameObject obj in colliderQuest)
    {
        obj.SetActive(true);
    }
}

public void Selesai_Misi()
{
    // Reset the character's position and rotation
    mainController.controller.enabled = false;
    gc.mainCharacter.transform.position = interactions.oldPosition;
    gc.mainCharacter.transform.localRotation = Quaternion.Euler(interactions.oldRotation);
    mainController.controller.enabled = true;

    // Switch cameras back
    gc.mainCamera.gameObject.SetActive(true);
    gc.camera2.gameObject.SetActive(false);
    interactions.oldPosition = Vector3.zero;
    interactions.oldRotation = Vector3.zero;

    // Update main controller's camera reference
    mainController.playerCamera = gc.mainCamera;

    // Update UI
    gc.mainUI.SetActive(true);
    gc.pilahSampahUI.SetActive(false);

    // Reset game variables
    GameVariable.isQuestStarting = false;
    GameVariable.questId = "";

    // Deactivate quest colliders
    foreach (GameObject obj in colliderQuest)
    {
        obj.SetActive(false);
    }

    // Destroy all spawned objects
    for (int i = spawnerMidPosition.childCount - 1; i >= 0; i--)
    {
        Destroy(spawnerMidPosition.GetChild(i).gameObject);
    }
}

    public void OnQuestStart() {
        Mulai_Misi();
    }

    public void OnQuestFinish() {
        Selesai_Misi();
    }

    public int GetWaktuQuest()
    {
        return -1;
    }

    public int GetScoreQuest()
    {
        return -1;
    }

    public Sprite GetImageRequiredQuest()
    {
        return null;
    }

    public Transform GetTransform()
    {
        return transform; // Mengembalikan Transform dari GameObject ini
    }

    void SpawnObjects()
{
    // Spawn objects around the spawnerMidPosition
    for (int i = 0; i < numberOfObjectsToSpawn; i++)
    {
        Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;
        spawnPosition += spawnerMidPosition.position;
        spawnPosition.y = spawnerMidPosition.position.y;

        GameObject objectPrefab = objectToSpawn[Random.Range(0, objectToSpawn.Count)];
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        spawnedObject.transform.parent = spawnerMidPosition;
    }
}

    public void Ambil_Sampah(IInventoryItem item)
    {
        Transform childTransform = inventoryQuest.GetChild(0);
        Image image = childTransform.GetComponent<Image>();
        InventoryVariable inventoryVariable = childTransform.GetComponent<InventoryVariable>();

        if (!image.enabled)
        {
            image.enabled = true;
            image.sprite = item.image;

            inventoryVariable.itemName = item.itemName;
            inventoryVariable.jenisSampah = item.jenisSampah;
            inventoryVariable.totalSampah = item.jumlahItem;

            item.OnPickupDestroy();
        }
        else
        {
            Debug.Log("Buang Sampah Dulu");
        }
    }

    public void Buang_Sampah(TrashcanController trashcanController, MainCharMovement nPanelShow)
    {
        Transform childTransform = inventoryQuest.GetChild(0);
        Image image = childTransform.GetComponent<Image>();
        InventoryVariable inventoryVariable = childTransform.GetComponent<InventoryVariable>();

        if(trashcanController.jenisTempatSampah == inventoryVariable.jenisSampah)
        {
            if (image.enabled)
            {
                image.enabled = false;

                inventoryVariable.itemName = "";
                inventoryVariable.jenisSampah = "";
                inventoryVariable.totalSampah = 0;
            }
        }
        else
        {
            if (image.sprite == null)
            {
                Debug.Log("Gagal Buang Sampah");
            }
            else
            {
                Debug.Log("Gagal Buang Sampah");
                nPanelShow.showNotification("Tempat Sampah Tidak Sesuai");
            }
        }
    }
}
