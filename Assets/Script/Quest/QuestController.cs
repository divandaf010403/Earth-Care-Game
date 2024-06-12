using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance;
    [SerializeField] int _questNumberActive;

    [Header("Objective")]
    [SerializeField] TextMeshProUGUI questObjectiveText;

    [Header("Objective List")]
    public string[] objectiveList;

    private void Awake() 
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

    void Start()
    {
        _questNumberActive = GameVariable.questNumber;
        ActivateQuest();
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

        Transform onQuestActiveTransform = transform.GetChild(_questNumberActive);
        onQuestActiveTransform.gameObject.SetActive(true);
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
                if (questChild != null)
                {
                    questChild.gameObject.SetActive(true);

                    for (int i = 0; i < questChild.childCount; i++)
                    {
                        if (questChild.GetChild(i).CompareTag("Conversation"))
                        {
                            questChild.GetChild(i).gameObject.SetActive(false);
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
            if (transform.GetChild(i).CompareTag("Quest"))
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void IncreaseObjectiveTutorial(int number)
    {
        if (number > GameVariable.questNumber)
        {
            GameVariable.questNumber = number;
            _questNumberActive = GameVariable.questNumber;
            ActivateQuest();
            SaveSystem.UpdatePlayerQuest();

            questObjectiveText.text = objectiveList[_questNumberActive];
        }
    }
}
