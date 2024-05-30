using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using Cinemachine;

public class ConversationStarter : MonoBehaviour
{
    public LayerMask _coversationLayerMask;
    public void startConversation() 
    {
        StartCoroutine(HandleInteraction());
    }

    public void SkipConversation()
    {
        if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
        {
            ConversationManager.Instance.SkipConversation();
        }
    }

    private IEnumerator HandleInteraction()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, _coversationLayerMask);

        // Start fade in
        yield return StartCoroutine(GameController.Instance.FadeInLoadingPanel());

        // Here you can add code to reposition the camera if needed
        RepositionCamera(false, colliders[0]);
        repositionCharacter(colliders[0]);
        yield return new WaitForSeconds(0.5f);

        // Start fade out
        yield return StartCoroutine(GameController.Instance.FadeOutLoadingPanel());

        foreach (Collider collider in colliders)
        {
            NPCConversation npcConversation = collider.transform.parent.GetComponent<NPCConversation>();
            ConversationManager.Instance.StartConversation(npcConversation);
        }
    }

    private void RepositionCamera(bool isEnable, Collider collider)
    {
        // Dapatkan referensi ke CinemachineFreeLook dari kamera utama
        CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

        Debug.Log(Camera.main.name);

        if (cinemachineBrain != null)
        {
            // Nonaktifkan CinemachineFreeLook jika diperlukan
            cinemachineBrain.enabled = isEnable;

            // Hitung posisi tengah antara pemain (collider) dan target (collider.parent)
            Vector3 playerPosition = collider.transform.position;
            Vector3 targetPosition = collider.transform.parent.position;
            Vector3 midpoint = (playerPosition + targetPosition) / 2.0f;

            // Geser posisi kamera ke kiri sejauh 20 unit dan ke atas sejauh 10 unit
            Vector3 offset = new Vector3(6f, 2.7f, 4f);
            Vector3 newPosition = midpoint + offset;

            // Atur posisi dan rotasi kamera utama
            Camera.main.transform.position = newPosition;
            
            Vector3 lookAtPosition = new Vector3(midpoint.x, midpoint.y + 1f, midpoint.z);
            Camera.main.transform.LookAt(lookAtPosition);

            Debug.Log(midpoint);

            // freeLookCamera.enabled = !isEnable;
        }
    }

    private void repositionCharacter(Collider collider)
    {
        MainCharMovement.Instance.controller.enabled = false;
        MainCharMovement.Instance.transform.position = collider.transform.position;
        MainCharMovement.Instance.transform.rotation = Quaternion.Euler(collider.transform.rotation.eulerAngles.x, collider.transform.rotation.eulerAngles.y + 180, collider.transform.rotation.eulerAngles.z);
        MainCharMovement.Instance.controller.enabled = true;
    }
}