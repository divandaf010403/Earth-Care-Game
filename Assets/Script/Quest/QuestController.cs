using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance;
    [SerializeField] public int _questNumberActive;

    [Header("Objective")]
    [SerializeField] TextMeshProUGUI questObjectiveText;

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

    void Start()
    {
        // _questNumberActive = GameVariable.questNumber;
        // ActivateQuest();
    }

    private void Update() 
    {
        
    }

    public void ActivateQuest()
    {
        // Menonaktifkan semua child game objects
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform questWpChild = transform.GetChild(i);
            questWpChild.gameObject.SetActive(false);
            for(int j = 0; j < questWpChild.childCount; j++)
            {
                if (questWpChild.GetChild(j).CompareTag("WpQuest"))
                {
                    questWpChild.GetChild(j).gameObject.SetActive(false);
                    break;
                }
            }
        }

        // Quest Mana Yang Harus Aktif
        Transform onQuestActiveTransform = transform.GetChild(_questNumberActive);
        onQuestActiveTransform.gameObject.SetActive(true);
        QuestObjectiveText objectiveText = onQuestActiveTransform.GetComponent<QuestObjectiveText>();
        if (objectiveText != null)
        {
            questObjectiveText.text = objectiveText.objectiveText;
        }


        for(int q = 0; q < onQuestActiveTransform.childCount; q++)
        {
            if (onQuestActiveTransform.GetChild(q).CompareTag("WpQuest"))
            {
                onQuestActiveTransform.GetChild(q).gameObject.SetActive(true);
                break;
            }
        }

        // Mengaktifkan Quest Selanjutnya
        if (transform.GetChild(_questNumberActive).CompareTag("Quest"))
        {
            int nextChildIndex = _questNumberActive + 1;
            if (nextChildIndex < transform.childCount)
            {
                Transform questChild = transform.GetChild(nextChildIndex);
                if (questChild != null && questChild.CompareTag("NPC"))
                {
                    questChild.gameObject.SetActive(true);

                    for (int i = 0; i < questChild.childCount; i++)
                    {
                        if (questChild.GetChild(i).CompareTag("Conversation"))
                        {
                            questChild.GetChild(i).gameObject.SetActive(false);
                            Debug.Log("Apakah" + questChild.GetChild(i).gameObject.name);
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Next child index is out of bounds.");
            }
        }
        else
        {
            Transform questChild = transform.GetChild(_questNumberActive);
            if (questChild != null)
            {
                for (int i = 0; i < questChild.childCount; i++)
                {
                    if (questChild.GetChild(i).CompareTag("Conversation"))
                    {
                        questChild.GetChild(i).gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }

        // Mengaktifkan child index sebelum questNumber dan ber tag quest
        for (int i = _questNumberActive - 1; i >= 0; i--)
        {
            IQuestFinishHandler handler = transform.GetChild(i).GetComponent<IQuestFinishHandler>();
            if (handler != null)
            {
                handler.IsQuestFinished = true;
            }

            if (transform.GetChild(i).CompareTag("Quest"))
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void IncreaseObjectiveTutorial(int number)
    {
        Debug.Log("Memeriksa peningkatan quest: current quest " + GameVariable.questNumber + ", incoming quest " + number);

        if (number == GameVariable.questNumber + 1)
        {
            GameVariable.questNumber = number;
            _questNumberActive = GameVariable.questNumber;
            ActivateQuest();
            SaveSystem.UpdatePlayerQuest();
        }
        else
        {
            Debug.LogWarning("Quest tidak berurutan atau sudah dilewati.");
        }
    }

    public void getChildNumberNextQuest(Transform thisObj)
    {
        // Jika parentObj ada
        if (thisObj.parent != null)
        {
            Transform parentObj = thisObj.parent;
            int directChildIndex = 0;

            // Loop melalui semua anak langsung dari parentObj
            for (int i = 0; i < parentObj.childCount; i++)
            {
                Transform child = parentObj.GetChild(i);

                // Cek apakah nama anak tersebut sama dengan nama thisObj
                if (child.name == thisObj.name)
                {
                    Debug.Log(thisObj.name + " adalah anak ke-" + directChildIndex + " dalam " + parentObj.name);
                    IncreaseObjectiveTutorial(directChildIndex + 1);

                    RewardSystem reward = thisObj.GetComponent<RewardSystem>();
                    if (reward != null)
                    {
                        RewardPanelController.Instance.getReward(reward);
                    }
                    return;
                }

                // Increment directChildIndex
                directChildIndex++;
            }

            // Jika tidak ditemukan, mungkin ada kesalahan dalam struktur hierarki atau nama yang tidak cocok
            Debug.LogError("Object " + thisObj.name + " tidak ditemukan sebagai anak langsung dari " + parentObj.name);
        }
        else
        {
            Debug.LogError("Object tidak memiliki parentObj.");
        }
    }
}
