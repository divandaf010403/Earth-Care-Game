using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [SerializeField] int _questNumberActive;

    // Start is called before the first frame update
    void Start()
    {
        // _questNumber = GameVariable.questNumber;
        // Debug.Log(GameVariable.questNumber);
        ActivateQuest();
    }

    // Update is called once per frame
    void Update()
    {
        // if (_questNumber != GameVariable.questNumber)
        // {
        //     _questNumber = GameVariable.questNumber;
        //     ActivateQuest();
        // }

        ActivateQuest();
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
        for (int i = 0; i <= _questNumberActive - 1; i++)
        {
            if (transform.GetChild(i).CompareTag("Quest"))
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
