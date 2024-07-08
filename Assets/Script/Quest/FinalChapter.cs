using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalChapter : MonoBehaviour
{
    [SerializeField] Collider finalCollider;

    // Start is called before the first frame update
    void Start()
    {
        if (GameVariable.questNumber == transform.GetSiblingIndex())
        {
            ConversationStarter.Instance.StartConversation(finalCollider);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
