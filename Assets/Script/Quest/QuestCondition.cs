using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCondition : MonoBehaviour
{
    [SerializeField] Transform questPosition;
    [SerializeField] GameObject saluranRusak;
    [SerializeField] GameObject saluranPerbaikan;

    private void Update() 
    {
        if (questPosition != null)
        {
            Debug.Log(questPosition.GetSiblingIndex() + " = " + "quest sekarang " + GameVariable.questNumber);
            if (questPosition.GetSiblingIndex() < GameVariable.questNumber)
            {
                saluranRusak.SetActive(false);
                saluranPerbaikan.SetActive(true);
            }
            else
            {
                saluranRusak.SetActive(true);
                saluranPerbaikan.SetActive(false);
            }
        }
    }
}
