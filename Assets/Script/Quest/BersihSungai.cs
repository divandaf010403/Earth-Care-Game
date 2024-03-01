using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BersihSungai : MonoBehaviour
{
    [SerializeField] public Camera camSetChange;
    [SerializeField] public Transform playerPositionChange;
    [SerializeField] public Transform cameraPositionChange;
    [SerializeField] public Transform trashSpawner;

    public Interactions interactions;
    public Transform isActiveTrigger;
    public Transform isActiveLeftCollider;
    public Transform isActiveRightCollider;

    void Start()
    {
        if (interactions != null)
        {
            // Interaction component found, you can use it now
        }
        else
        {
            Debug.LogError("Interactions component not found on MainCharacter.");
        }

        isActiveLeftCollider.gameObject.SetActive(false);
        isActiveRightCollider.gameObject.SetActive(false);
    }

    IEnumerator ActivateObjectDelayed()
    {
        yield return null; // Menunggu satu frame
        if (interactions != null)
        {
            if (interactions.isQuestStart)
            {
                isActiveTrigger.gameObject.SetActive(false);
                isActiveLeftCollider.gameObject.SetActive(true);
                isActiveRightCollider.gameObject.SetActive(true);
            }
            else
            {
                isActiveTrigger.gameObject.SetActive(true);
                isActiveLeftCollider.gameObject.SetActive(false);
                isActiveRightCollider.gameObject.SetActive(false);
            }
        }
    }

    // Panggil coroutine ini ketika Anda ingin mengubah keadaan objek
    public void DeactivateObject()
    {
        StartCoroutine(ActivateObjectDelayed());
    }

}
