using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BersihSungai : MonoBehaviour, IQuestHandler
{
    public static BersihSungai Instance;
    [SerializeField] public string quest_id = "3Q";
    [SerializeField] public Camera questCamera;
    [SerializeField] public Transform questPlayerPosition;
    [SerializeField] public Transform questCameraPosition;
    [SerializeField] public Transform trashSpawner;
    public Transform isActiveTrigger;
    public List<GameObject> colliderQuest;
    public List<GameObject> objectToSpawn;
    private Coroutine spawnCoroutine;

    private float spawnIntervalMin = 1f;
    private float spawnIntervalMax = 3f;

    [Header("Get Component")]
    // public MainCharMovement mainController;
    public Interactions interactions;

    //Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform QuestCameraPosition => questCameraPosition;
    public Transform IsActiveTrigger => isActiveTrigger;

    [Header("Quest Setting")]
    [SerializeField] Sprite imgRequired;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI pointsText;
    float questStartTimer = 3f; // Timer untuk memulai quest
    float questDuration = 60f; // Durasi quest dalam detik (3 menit)
    bool questStarted = false; // Untuk melacak apakah quest sudah dimulai
    float trashSpawnInterval = 5f; // Interval untuk spawn trash
    float trashSpawnTimer = 0f; // Timer untuk spawn trash
    int questPoint = 0; // Poin saat menjalankan quest

    [Header("Ketika Quest Selesai")]
    public Transform finishPanel;

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
                    finishedQuest(questPoint);
                    OnQuestFinish();
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

            pointsText.text = "Skor : " + questPoint.ToString();
        }

        isActiveTrigger.gameObject.SetActive(GameVariable.isQuestStarting ? false : true);
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

    public void Mulai_Misi()
    {
        //Variable Set
        GameVariable.isQuestStarting = true;
        GameVariable.questId = quest_id;

        GameController.Instance.mainCharacter.gameObject.SetActive(false);
        GameController.Instance.mainCharacterRiverQuest.gameObject.SetActive(true);

        MainCharMovement.Instance.controller.enabled = false;
        GameController.Instance.mainCharacterRiverQuest.transform.position = interactions.newPosition;
        GameController.Instance.mainCharacterRiverQuest.transform.rotation = Quaternion.Euler(interactions.newRotation);
        GameController.Instance.camera2.transform.position = interactions.cameraSetPosition;
        GameController.Instance.camera2.transform.rotation = Quaternion.Euler(interactions.cameraSetRotation);
        MainCharMovement.Instance.controller.enabled = true;

        GameController.Instance.mainCamera.gameObject.SetActive(false);
        GameController.Instance.camera2.gameObject.SetActive(true);
        GameVariable.isQuestStarting = true;

        GameController.Instance.mainUI.SetActive(false);
        GameController.Instance.bersihSungaiUI.SetActive(true);

        spawnCoroutine = StartCoroutine(SpawnItemsPeriodically());

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

        GameController.Instance.mainCharacter.gameObject.SetActive(true);
        GameController.Instance.mainCharacterRiverQuest.gameObject.SetActive(false);

        GameController.Instance.mainCamera.gameObject.SetActive(true);
        GameController.Instance.camera2.gameObject.SetActive(false);

        GameController.Instance.mainUI.SetActive(true);
        GameController.Instance.bersihSungaiUI.SetActive(false);

        foreach (GameObject obj in colliderQuest)
        {
            obj.SetActive(false);
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        // Hapus semua objek yang menjadi child dari spawnPoint
        foreach (Transform child in trashSpawner)
        {
            Destroy(child.gameObject);
        }
    }

    public int GetWaktuQuest()
    {
        return (int)questDuration;
    }

    public int GetScoreQuest()
    {
        return questPoint;
    }

    public Sprite GetImageRequiredQuest()
    {
        return imgRequired;
    }

    public void OnQuestStart()
    {
        Mulai_Misi();
    }

    public void OnQuestFinish()
    {
        Selesai_Misi();
    }

    public Transform GetTransform()
    {
        return transform; // Mengembalikan Transform dari GameObject ini
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

    void finishedQuest(int finishScore)
    {
        finishPanel.gameObject.SetActive(true);
        finishPanel.localPosition = new Vector3(0f, 0f, 0f);
        Transform finishPanelChild = finishPanel.GetChild(0);
        
        finishPanelChild.GetChild(0).GetComponent<TextMeshProUGUI>().text = finishScore.ToString();
        finishPanelChild.GetChild(1).GetComponent<TextMeshProUGUI>().text = questPoint >= 50 ? "YAY BERHASIL" : "GAGAL!!!";
        finishPanelChild.GetChild(1).GetComponent<TextMeshProUGUI>().color = questPoint >= 50 ? Color.green : Color.red;
    }
}
