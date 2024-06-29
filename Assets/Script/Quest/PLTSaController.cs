using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLTSaController : MonoBehaviour
{
    [SerializeField] PLTSaQuest requiredOrganik;
    [SerializeField] PLTSaQuest requiredAnorganik;

    [Header("Next Action")]
    [SerializeField] Transform nextTranformToActive;
    // [SerializeField] private GameObject questToActive;
    // [SerializeField] private GameObject questToNonActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (requiredAnorganik.requiredItem == requiredAnorganik.totalRequireItem && requiredOrganik.requiredItem == requiredOrganik.totalRequireItem)
        {
        //     questToActive.SetActive(true);
        //     questToNonActive.SetActive(false);

            QuestController.Instance.getChildNumberNextQuest(transform);
        }
    }
}
