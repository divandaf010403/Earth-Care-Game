using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField] private int _questNumber;
    // Start is called before the first frame update
    void Start()
    {
        _questNumber = GameVariable.questNumber;
        ActivateQuest();
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
