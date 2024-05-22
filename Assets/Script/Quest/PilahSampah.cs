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
    [SerializeField] public Transform trashSpawner;
    public Transform isActiveTrigger;
    public List<GameObject> colliderQuest;

    [Header("Component")]
    public GameController gc;
    public MainCharMovement mainController;
    public Interactions interactions;
    public Transform inventoryQuest;

    // Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform QuestCameraPosition => questCameraPosition;
    public Transform TrashSpawner => trashSpawner;
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
            if (spawnerMidPosition.childCount == 0 && inventoryQuest.GetChild(0).GetComponent<Image>().enabled == false)
            {
                Selesai_Misi();
                questToActive.SetActive(true);
            }
        }
    }

    IEnumerator ActivateObjectDelayed()
    {
        yield return null; // Menunggu satu frame
        if (GameVariable.isQuestStarting)
        {
            isActiveTrigger.gameObject.SetActive(false);
        }
        else
        {
            isActiveTrigger.gameObject.SetActive(true);
        }
    }

    // Panggil coroutine ini ketika Anda ingin mengubah keadaan objek
    public void DeactivateObject()
    {
        StartCoroutine(ActivateObjectDelayed());
    }

    public void Mulai_Misi()
    {
        interactions.oldPosition = gc.mainCharacter.transform.position;
        interactions.oldRotation = new Vector3(0f, gc.mainCharacter.transform.eulerAngles.y, 0f);

        mainController.controller.enabled = false;
        gc.mainCharacter.transform.position = interactions.newPosition;
        gc.mainCharacter.transform.rotation = Quaternion.Euler(interactions.newRotation);
        gc.camera2.transform.position = interactions.cameraSetPosition;
        gc.camera2.transform.rotation = Quaternion.Euler(interactions.cameraSetRotation);
        mainController.controller.enabled = true;

        gc.mainCamera.gameObject.SetActive(false);
        gc.camera2.gameObject.SetActive(true);

        mainController.playerCamera = gc.camera2;

        //Variable Set
        GameVariable.isQuestStarting = true;
        GameVariable.questId = quest_id;

        gc.mainUI.SetActive(false);
        gc.pilahSampahUI.SetActive(true);

        SpawnObjects();

        foreach (GameObject obj in colliderQuest)
        {
            obj.SetActive(true);
        }
    }

    public void Selesai_Misi()
    {
        mainController.controller.enabled = false;
        gc.mainCharacter.transform.position = interactions.oldPosition;
        gc.mainCharacter.transform.localRotation = Quaternion.Euler(interactions.oldRotation);
        mainController.controller.enabled = true;

        gc.mainCamera.gameObject.SetActive(true);
        gc.camera2.gameObject.SetActive(false);
        interactions.oldPosition = Vector3.zero;
        interactions.oldRotation = Vector3.zero;

        mainController.playerCamera = gc.mainCamera;

        gc.mainUI.SetActive(true);
        gc.pilahSampahUI.SetActive(false);

        //Variable Set
        GameVariable.isQuestStarting = false;
        GameVariable.questId = "";

        foreach (GameObject obj in colliderQuest)
        {
            obj.SetActive(false);
        }

        if (spawnerMidPosition != null)
        {
            // Looping untuk menghapus semua child objek
            foreach (Transform child in spawnerMidPosition.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void OnQuestStart() {
        Mulai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }

    public void OnQuestFinish() {
        Selesai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }

    void SpawnObjects()
    {
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
