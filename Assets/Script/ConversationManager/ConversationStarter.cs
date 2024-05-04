using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    public LayerMask _coversationLayerMask;
    public void startConversation() 
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, _coversationLayerMask);

        foreach (Collider collider in colliders)
        {
            NPCConversation npcConversation = collider.transform.parent.GetComponent<NPCConversation>();
            ConversationManager.Instance.StartConversation(npcConversation);
        }
    }
}