using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BersihSungai : MonoBehaviour, IQuestHandler
{
    [SerializeField] public Camera questCamera;
    [SerializeField] public Transform questPlayerPosition;
    [SerializeField] public Transform questCameraPosition;
    [SerializeField] public Transform trashSpawner;
    public Transform isActiveTrigger;
    public Transform isActiveLeftCollider;
    public Transform isActiveRightCollider;

    [Header("Get Component")]
    public GameController gc;
    public MainCharMovement mainController;
    public Interactions interactions;

    //Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform QuestCameraPosition => questCameraPosition;
    public Transform TrashSpawner => trashSpawner;
    public Transform IsActiveTrigger => isActiveTrigger;

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

    public void Mulai_Misi()
    {
        interactions.oldPosition = gc.mainCharacter.transform.position;
        interactions.oldRotation = new Vector3(0f, gc.mainCharacter.transform.eulerAngles.y, 0f);

        mainController.controller.enabled = false;
        gc.mainCharacter.transform.position = interactions.newPosition;
        gc.mainCharacter.transform.rotation = Quaternion.Euler(interactions.newRotation);
        gc.camera2.transform.position = interactions.cameraSetPosition;
        gc.camera2.transform.rotation = Quaternion.Euler(interactions.cameraSetRotation);
        mainController.controller.enabled = true;

        gc.mainCamera.gameObject.SetActive(false);
        gc.camera2.gameObject.SetActive(true);

        mainController.playerCamera = gc.camera2;
        interactions.isQuestStart = true;

        gc.mainUI.SetActive(false);
        gc.bersihSungaiUI.SetActive(true);
    }

    public void Selesai_Misi()
    {
        mainController.controller.enabled = false;
        gc.mainCharacter.transform.position = interactions.oldPosition;
        gc.mainCharacter.transform.localRotation = Quaternion.Euler(interactions.oldRotation);
        mainController.controller.enabled = true;

        gc.mainCamera.gameObject.SetActive(true);
        gc.camera2.gameObject.SetActive(false);
        interactions.oldPosition = Vector3.zero;
        interactions.oldRotation = Vector3.zero;

        mainController.playerCamera = gc.mainCamera;
        interactions.isQuestStart = false;

        gc.mainUI.SetActive(true);
        gc.bersihSungaiUI.SetActive(false);
    }

    public void OnQuestStart()
    {
        Mulai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }

    public void OnQuestFinish()
    {
        Selesai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }
}
