using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public string itemId;
    public string prefabPath;

    void Awake()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            itemId = "Item_Trash" + " " + transform.parent.name + " " + transform.GetSiblingIndex().ToString();
        }
    }

    void Start()
    {
        CheckItemStatus();
    }

    public void CheckItemStatus()
    {
        if (PlayerPrefs.GetInt(itemId, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeItem()
    {
        PlayerPrefs.SetInt(itemId, 1);
        gameObject.SetActive(false);
    }
}
