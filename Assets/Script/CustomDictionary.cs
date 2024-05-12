using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomDictionary
{
    [SerializeField] CustomDictionaryItem[] customDictionaryItems;

    public Dictionary<string, Transform> ToDictionary()
    {
        Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();

        foreach (var item in customDictionaryItems)
        {
            dictionary.Add(item.name, item.objTransform);
        }

        return dictionary;
    }
}

[Serializable]
public class CustomDictionaryItem
{
    [SerializeField] public string name;
    [SerializeField] public Transform objTransform;
}
