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
        ActivateQuest();

        if (questObjectiveText != null && objectiveList != null)
        {
            questObjectiveText.text = objectiveList[_questNumberActive];
        }
    }

    private void Update() 
    {
        
    }

    public void ActivateQuest()
    {
        // Menonaktifkan semua child game objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        transform.GetChild(_questNumberActive).gameObject.SetActive(true);

        // Mengaktifkan child index sebelum questNumber dan ber tag quest
        for (int i = _questNumberActive - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).CompareTag("Quest"))
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
