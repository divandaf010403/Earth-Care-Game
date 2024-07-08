using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChokePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            QuestController.Instance.getChildNumberNextQuest(transform);
        }
    }
}
