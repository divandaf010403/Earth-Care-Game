using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BersihSungai : MonoBehaviour, IQuestHandler
{
    [SerializeField] public Camera questCamera;
    [SerializeField] public Transform questPlayerPosition;
    [SerializeField] public Transform questCameraPosition;
    [SerializeField] public Transform trashSpawner;
    public Transform isActiveTrigger;
    public List<GameObject> colliderQuest;
    public List<GameObject> objectToSpawn;
    private float spawnIntervalMin = 1f;
    private float spawnIntervalMax = 3f;

    [Header("Get Component")]
    public GameController gc;
    public MainCharMovement mainController;
    public Interactions interactions;

    //Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform QuestCameraPosition => questCameraPosition;
    public Transform TrashSpawner => trashSpawner;
    public Transform IsActiveTrigger => isActiveTrigger;

    void Start()
    {
        if (interactions != null)
        {
            // Interaction component found, you can use it now
        }
        else
        {
            Debug.LogError("Interactions component not found on MainCharacter.");
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
        GameVariable.isQuestStarting = true;

        gc.mainUI.SetActive(false);
        gc.bersihSungaiUI.SetActive(true);

        StartCoroutine(SpawnItemsPeriodically());

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
        GameVariable.isQuestStarting = false;

        gc.mainUI.SetActive(true);
        gc.bersihSungaiUI.SetActive(false);

        foreach (GameObject obj in colliderQuest)
        {
            obj.SetActive(false);
        }

        // Hentikan coroutine spawn jika sedang berjalan
        if (SpawnItemsPeriodically() != null)
        {
            StopCoroutine(SpawnItemsPeriodically());
        }

        // Hapus semua objek yang menjadi child dari spawnPoint
        foreach (Transform child in trashSpawner)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnQuestStart()
    {
        Mulai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }

    public void OnQuestFinish()
    {
        Selesai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }

    void RandomSpawnTrash()
    {
        float randomZ = Random.Range(trashSpawner.position.z -3f, trashSpawner.position.z + 3f);
        int randomIndex = Random.Range(0, objectToSpawn.Count);

        Vector3 spawnPosition = new Vector3(trashSpawner.position.x, trashSpawner.position.y, randomZ);

        GameObject newObject = Instantiate(objectToSpawn[randomIndex], spawnPosition, Quaternion.identity);

        newObject.transform.parent = trashSpawner;

        if (newObject.GetComponent<Rigidbody>() != null)
        {
            newObject.GetComponent<Rigidbody>().useGravity = false;
        }

        // Tentukan kecepatan gerakan
        float moveSpeed = 3f; // Sesuaikan dengan kebutuhan Anda

        // Gerakkan objek ke arah x positif setiap frame
        if (newObject.GetComponent<Rigidbody>() != null)
        {
            newObject.GetComponent<Rigidbody>().velocity = Vector3.right * moveSpeed;
        }
    }

    IEnumerator SpawnItemsPeriodically()
    {
        while (true)
        {
            // Spawn item secara acak
            RandomSpawnTrash();

            // Tunggu interval antara spawn sebelum melanjutkan
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
        }
    }
}
