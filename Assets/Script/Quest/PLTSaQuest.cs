using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PLTSaQuest : MonoBehaviour
{
    public int requiredItem = 0;
    public TextMeshProUGUI alreadyRequireTxt;
    [SerializeField] public string tipePenampungan = "Organik";
    int totalRequireItem = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        alreadyRequireTxt.text = requiredItem.ToString() + " / " + totalRequireItem;
    }
}
