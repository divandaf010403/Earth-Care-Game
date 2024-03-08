using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilahSampah : MonoBehaviour, IQuestHandler
{
    [SerializeField] public Camera questCamera;
    [SerializeField] public Transform questPlayerPosition;
    [SerializeField] public Transform questCameraPosition;
    [SerializeField] public Transform trashSpawner;
    public Transform isActiveTrigger;
    public List<GameObject> colliderQuest;

    [Header("Component")]
    public GameController gc;
    public MainCharMovement mainController;
    public Interactions interactions;

    // Interface
    public Camera QuestCamera => questCamera;
    public Transform QuestPlayerPosition => questPlayerPosition;
    public Transform QuestCameraPosition => questCameraPosition;
    public Transform TrashSpawner => trashSpawner;
    public Transform IsActiveTrigger => isActiveTrigger;

    IEnumerator ActivateObjectDelayed()
    {
        yield return null; // Menunggu satu frame
        if (interactions != null)
        {
            if (interactions.isQuestStart)
            {
                isActiveTrigger.gameObject.SetActive(false);
            }
            else
            {
                isActiveTrigger.gameObject.SetActive(true);
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
        gc.pilahSampahUI.SetActive(true);

        foreach (GameObject obj in colliderQuest)
        {
            obj.SetActive(true);
        }
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
        gc.pilahSampahUI.SetActive(false);

        foreach (GameObject obj in colliderQuest)
        {
            obj.SetActive(false);
        }
    }

    public void OnQuestStart() {
        Mulai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }

    public void OnQuestFinish() {
        Selesai_Misi();
        StartCoroutine(ActivateObjectDelayed());
    }
}
