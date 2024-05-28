using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestController : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] private int _questNumber;
=======
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

>>>>>>> Stashed changes
    // Start is called before the first frame update
    void Start()
    {
        _questNumber = GameVariable.questNumber;
        ActivateQuest();

        if (questObjectiveText != null && objectiveList != null)
        {
            questObjectiveText.text = objectiveList[_questNumberActive];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_questNumber != GameVariable.questNumber)
        {
            _questNumber = GameVariable.questNumber;
            ActivateQuest();
        }
    }

    public void ActivateQuest()
    {
        // Menonaktifkan semua child game objects
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        transform.GetChild(GameVariable.questNumber).gameObject.SetActive(true);

        // Mengaktifkan child index sebelum questNumber dan ber tag quest
        for (int i = GameVariable.questNumber - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).CompareTag("quest"))
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
