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
    public List<Sprite> imageTutorialList;

    private float spawnIntervalMin = 0.5f;
    private float spawnIntervalMax = 2f;

    [Header("Get Component")]
    // public MainCharMovement mainController;
    public Interactions interactions;

    //Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform QuestCameraPosition => questCameraPosition;
    public Transform IsActiveTrigger => isActiveTrigger;

    [Header("Quest Setting")]
    public Sprite imgRequireItem;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI pointsText;
    [SerializeField] float questStartTimer = 3f; // Timer untuk memulai quest
    [SerializeField] float questDuration = 60f; // Durasi quest dalam detik (3 menit)
    float questDurationCountdown;
    private bool questStarted = false;
    private bool questFinished = false;
    float trashSpawnInterval = 5f; // Interval untuk spawn trash
    float trashSpawnTimer = 0f; // Timer untuk spawn trash
    int questPoint = 0; // Poin saat menjalankan quest
    int questPointRequire = 100;

    [Header("Ketika Quest Selesai")]
    [SerializeField] TextMeshProUGUI scoreTxt;
    [SerializeField] TextMeshProUGUI scoreResult;
    public Transform finishPanel;

    [Header("Next Action")]
    [SerializeField] private GameObject questToActive;
    [SerializeField] private GameObject questToNonActive;

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
                if (questDurationCountdown > 0f)
                {
                    questDurationCountdown -= Time.deltaTime;
                    if (questDurationCountdown <= 0f)
                    {
                        questDurationCountdown = 0f; // Ensure questDuration does not go below 0
                    }
                }

                if (questDurationCountdown == 0f && !questFinished)
                {
                    questFinished = true; // Ensure this block only runs once
                    StartCoroutine(GameController.Instance.HandleWithLoadingPanelTransition(() =>
                    {
                        finishedQuest(questPoint);
                        OnQuestFinish();

                        if (questPoint >= questPointRequire)
                        {
                            questToActive.SetActive(true);
                            questToNonActive.SetActive(false);

                            QuestController.Instance.IncreaseObjectiveTutorial(8);
                        }
                    }, null));
                }
                else if (questDurationCountdown > 0f)
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

        isActiveTrigger.gameObject.SetActive(!GameVariable.isQuestStarting);
    }

    void UpdateCountdownText()
    {
        // Pastikan countdownText tidak null
        if (countdownText != null)
        {
            // Ubah durasi quest menjadi format menit:detik
            int minutes = Mathf.FloorToInt(questDurationCountdown / 60f);
            int seconds = Mathf.FloorToInt(questDurationCountdown % 60f);
            
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
        // Reset variabel quest
        questStarted = false;
        questFinished = false;
        questStartTimer = 3f; // Set initial timer value
        questDurationCountdown = questDuration; // Set initial quest duration
        questPoint = 0; // Reset quest points

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
        GameVariable.speed = 5;

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

    public void OnQuestStart()
    {
        Mulai_Misi();
    }

    public void OnQuestFinish()
    {
        Selesai_Misi();
    }

    public List<Sprite> imgTutorialList()
    {
        return imageTutorialList;
    }

    public int GetWaktuQuest()
    {
        return (int)questDuration;
    }

    public int GetScoreQuest()
    {
        return questPointRequire;
    }

    public Sprite GetImageRequiredQuest()
    {
        return imgRequireItem;
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
        
        scoreTxt.text = finishScore.ToString();
        scoreResult.text = questPoint >= questPointRequire ? "BERHASIL" : "GAGAL!!!";
    }
}
