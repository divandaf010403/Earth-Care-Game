using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpateTrashIndicator : MonoBehaviour
{
    [SerializeField] Transform trashParent;
    Slider slider;
    [SerializeField] int trashCount = 0;
    [SerializeField] int MaxTrash = 50;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        int activeChildCount = 0;
        foreach (Transform child in trashParent)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }

        if (trashCount != activeChildCount)
        {
            trashCount = activeChildCount;
            slider.value = (float)trashCount / MaxTrash * 100;

            Debug.Log(slider.value);
            Debug.Log((float)trashCount / MaxTrash * 100);
        }
    }
}
