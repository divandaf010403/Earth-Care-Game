using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestPantai1 : MonoBehaviour
{
    [SerializeField] GameObject sampahPantaiPrefab;
    private GameObject prefabInstance;

    // Start is called before the first frame update
    void Awake()
    {
        prefabInstance = Instantiate(sampahPantaiPrefab, transform);

        for (int i = 0; i < prefabInstance.transform.childCount; i++)
        {
            prefabInstance.transform.GetChild(i).gameObject.layer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
