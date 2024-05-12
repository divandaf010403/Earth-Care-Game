using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BersihSungai : MonoBehaviour, IQuestHandler
{
    public static BersihSungai Instance;
    [SerializeField] public string quest_id = "2Q";
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

    [Header("Quest Setting")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI pointsText;
    float questStartTimer = 3f; // Timer untuk memulai quest
    float questDuration = 60f; // Durasi quest dalam detik (3 menit)
    bool questStarted = false; // Untuk melacak apakah quest sudah dimulai
    float trashSpawnInterval = 5f; // Interval untuk spawn trash
    float trashSpawnTimer = 0f; // Timer untuk spawn trash
    int questPoint = 0; // Poin saat menjalankan quest

    void Awake()
    {
        // Pastikan hanya ada satu instance QuestManager yang ada
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update() 
    {
        if (GameVariable.isQuestStarting && (GameVariable.questId == quest_id))
        {
            // Jika quest belum dimulai, mulai countdown
            if (!questStarted)
            {
                questStartTimer -= Time.deltaTime;
                GameVariable.speed = 0f; // Karakter tidak bisa bergerak

                if (questStartTimer <= 0f)
                {
                    questStarted = true;
                    GameVariable.speed = 5f; // Karakter bisa bergerak
                    trashSpawnTimer = trashSpawnInterval; // Mulai spawn trash
                }
            }
            else
            {
                // Quest sudah dimulai, mulai countdown 3 menit
                questDuration -= Time.deltaTime;

                if (questDuration <= 0f)
                {
                    // Quest selesai, lakukan penanganan sesuai kebutuhan
                    // Misalnya: Menampilkan pesan quest selesai, mereset quest, dll.
                    // Reset variabel dan kondisi yang diperlukan
                    questStartTimer = 3f;
                    questStarted = false;
                    GameVariable.speed = 0f; // Karakter tidak bisa bergerak lagi
                    questDuration = 180f; // Reset durasi quest
                }
                else
                {
                    // Quest masih berlangsung, jalankan fungsi RandomSpawnTrash setiap interval
                    trashSpawnTimer -= Time.deltaTime;
                    if (trashSpawnTimer <= 0f)
                    {
                        RandomSpawnTrash();
                        trashSpawnTimer = trashSpawnInterval; // Reset timer spawn trash
                    }
                }
            }

            // Update countdown text
            UpdateCountdownText();

            pointsText.text = questPoint.ToString();
        }    
    }

    void UpdateCountdownText()
    {
        // Pastikan countdownText tidak null
        if (countdownText != null)
        {
            // Ubah durasi quest menjadi format menit:detik
            int minutes = Mathf.FloorToInt(questDuration / 60f);
            int seconds = Mathf.FloorToInt(questDuration % 60f);
            
            // Tampilkan pada TextMeshPro Text UI
            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void AddPoints(int pointsToAdd)
    {
        questPoint += pointsToAdd;
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
        //Variable Set
        GameVariable.isQuestStarting = true;
        GameVariable.questId = quest_id;

        gc.mainCharacter.gameObject.SetActive(false);
        gc.mainCharacterRiverQuest.gameObject.SetActive(true);

        mainController.controller.enabled = false;
        gc.mainCharacterRiverQuest.transform.position = interactions.newPosition;
        gc.mainCharacterRiverQuest.transform.rotation = Quaternion.Euler(interactions.newRotation);
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
        //Variable Set
        GameVariable.isQuestStarting = false;
        GameVariable.questId = "";

        gc.mainCharacter.gameObject.SetActive(true);
        gc.mainCharacterRiverQuest.gameObject.SetActive(false);

        gc.mainCamera.gameObject.SetActive(true);
        gc.camera2.gameObject.SetActive(false);

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
