using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour, IQuestHandler
{
    [Header("Component")]
    [SerializeField] public string quest_id = "4Q";
    [SerializeField] public Camera questCamera;
    [SerializeField] public Transform questPlayerPosition;
    [SerializeField] public Transform trashSpawner;
    public Transform isActiveTrigger;
    public List<GameObject> colliderQuest;
    public List<Sprite> imageTutorialList;
    public string[] requirementItem;
    
    // Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform IsActiveTrigger => isActiveTrigger;
    public Transform QuestCameraPosition => questCamera.transform;
    public string[] requiredItemToQuest => requirementItem;

    [Header("Quest Settings")]
    public TextMeshProUGUI countdownText;
    public Transform spawnPos;
    public List<GameObject> objectToSpawn;
    public float spawnRadius = 5f;
    float questStartTimer = 3f; // Timer untuk memulai quest
    float questDuration = 180f; // Durasi quest dalam detik (3 menit)
    bool questStarted = false; // Untuk melacak apakah quest sudah dimulai

    [Header("Get Components")]
    public Interactions interactions;
    [SerializeField] GameObject[] hiddenObjUi;
    [SerializeField] GameObject showUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(ActivateObjectDelayed());

        if (GameVariable.isQuestStarting && (GameVariable.questId == quest_id))
        {

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

    public void DeactivateObject()
    {
        StartCoroutine(ActivateObjectDelayed());
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

    public List<Sprite> imgTutorialList()
    {
        return imageTutorialList;
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

    void Mulai_Misi()
    {
        interactions.oldPosition = GameController.Instance.mainCharacter.transform.position;
        interactions.oldRotation = new Vector3(0f, GameController.Instance.mainCharacter.transform.eulerAngles.y, 0f);

        MainCharMovement.Instance.controller.enabled = false;
        GameController.Instance.mainCharacter.transform.position = interactions.newPosition;
        GameController.Instance.mainCharacter.transform.rotation = Quaternion.Euler(interactions.newRotation);
        MainCharMovement.Instance.controller.enabled = true;

        //Variable Set
        GameVariable.isQuestStarting = true;
        GameVariable.questId = quest_id;

        hideUI(false);
        showUI.SetActive(true);

        SpawnObjects();
    }

    void Selesai_Misi()
    {
        MainCharMovement.Instance.controller.enabled = false;
        GameController.Instance.mainCharacter.transform.position = interactions.oldPosition;
        GameController.Instance.mainCharacter.transform.localRotation = Quaternion.Euler(interactions.oldRotation);
        MainCharMovement.Instance.controller.enabled = true;

        interactions.oldPosition = Vector3.zero;
        interactions.oldRotation = Vector3.zero;

        //Variable Set
        GameVariable.isQuestStarting = false;
        GameVariable.questId = "";

        hideUI(true);
        showUI.SetActive(false);

        for (int i = 0; i < spawnPos.childCount; i++)
        {
            Transform child = spawnPos.GetChild(i);
            for (int j = child.childCount - 1; j >= 0; j--)
            {
                Destroy(child.GetChild(j).gameObject);
            }
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < spawnPos.childCount; i++)
        {
            int randTotalItemSpawn = Random.Range(3, 7);
            for (int j = 0; j < randTotalItemSpawn; j++)
            {
                Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius;

                spawnPosition += spawnPos.GetChild(i).position;

                spawnPosition.y = spawnPos.GetChild(i).position.y;

                GameObject objectPrefab = objectToSpawn[Random.Range(0, objectToSpawn.Count)];

                GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

                spawnedObject.transform.parent = spawnPos.GetChild(i);
            }
        }
    }

    void hideUI(bool isHide)
    {
        for (int i = 0; i < hiddenObjUi.Length; i++)
        {
            hiddenObjUi[i].SetActive(isHide);
        }
    }
}
