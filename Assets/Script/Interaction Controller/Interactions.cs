using Cinemachine.Utility;
using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Interactions : MonoBehaviour
{

    [SerializeField] private Transform _interactPoint;
    [SerializeField] private float _interactPointRadius;
    [SerializeField] private LayerMask _interactLayerMask;

    private readonly Collider[] _colliders = new Collider[1];
    [SerializeField] private int _numFound;
    [SerializeField] public GameObject buttonInteract;

    [Header("Get Component")]
    MainCharMovement mainChar;
    public Inventory inventory;
    TrashcanController trashcanController;

    [Header("Inventory")]
    Transform otherGameObject;
    Transform inventoryPanel;

    [Header("Ambil Variabel")]
    GameController gc;
    [SerializeField] private Vector3 newPosition;
    [SerializeField] private Vector3 newRotation;
    [SerializeField] private Vector3 oldPosition = Vector3.zero;
    [SerializeField] private Vector3 oldRotation = Vector3.zero;

    [Header("Quest")]
    [SerializeField] public bool isQuestStart = false;

    // Start is called before the first frame update
    void Start()
    {
        otherGameObject = GameObject.Find("Screen").transform.GetChild(0);
        inventoryPanel = otherGameObject.transform.Find("Inventory");
        GameObject gameController = GameObject.Find("GameController");

        mainChar = GetComponent<MainCharMovement>();
        gc = gameController.GetComponent<GameController>();
        buttonInteract.SetActive(false);

        inventory.ItemAdded += InventoryScript_ItemAdded;
        // inventory.ItemRemoved += InventoryScript_ItemRemoved;
    }

    // Update is called once per frame
    void Update()
    {
        // Ambil Sampah
        _numFound = Physics.OverlapSphereNonAlloc(_interactPoint.position, _interactPointRadius, _colliders, _interactLayerMask);

        if (_numFound > 0)
        {
            if (_colliders[0].CompareTag("Item"))
            {
                buttonInteract.SetActive(true);
            }
            else if (_colliders[0].CompareTag("Trashcan"))
            {
                buttonInteract.SetActive(true);
            }
            else
            {
                buttonInteract.SetActive(false);
            }
        }
        else
        {
            buttonInteract.SetActive(false);
        }

        // Quest
        if (isQuestStart)
        {
            mainChar.endMisiBtn.SetActive(true);
        }
        else
        {
            mainChar.endMisiBtn.SetActive(false);
        }
    }

    public void buttonCondition()
    {
        if (_numFound > 0)
        {
            if (_colliders[0].CompareTag("Item"))
            {
                removeItem();
            }
            else if (_colliders[0].CompareTag("Trashcan"))
            {
                Interact_Trashcan();
            }
        }
        else
        {
            buttonInteract.SetActive(false);
        }
    }


    public void removeItem()
    {
        var interactableItem = _colliders[0].GetComponent<Interactable>();
        IInventoryItem item = _colliders[0].GetComponent<IInventoryItem>();

        if (interactableItem != null)
        {
            interactableItem.Interact(this);
        }

        if (item != null)
        {
            inventory.AddItem(item);
        }
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        foreach (Transform slot in inventoryPanel)
        {
            // Check if the slot has any children
            if (slot.childCount > 0)
            {
                Transform imageTransform = slot.GetChild(0).GetChild(0);
                Image image = imageTransform.GetChild(0).GetComponent<Image>();
                // ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
                InventoryVariable inventoryVariable = imageTransform.GetComponent<InventoryVariable>();

                if (!image.enabled)
                {
                    image.enabled = true;
                    image.sprite = e.Item.image;

                    inventoryVariable.jenisSampah = e.Item.jenisSampah;

                    // itemDragHandler.Item = e.Item;

                    if (mainChar != null)
                    {
                        mainChar.cubeVal++;
                    }

                    break;
                }
            }
        }
    }

    // private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    // {
    //     foreach (Transform slot in inventoryPanel)
    //     {
    //         Transform imageTransform = slot.GetChild(0).GetChild(0);
    //         Image image = imageTransform.GetChild(0).GetComponent<Image>();
    //         // ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
    //         InventoryVariable inventoryVariable = imageTransform.GetComponent<InventoryVariable>();

    //         Transform propsTransform = transform.Find("Props");

    //         if (propsTransform != null)
    //         {
    //             Transform trashcanTransform = propsTransform.Find("Trashcan");
    //             if (trashcanTransform != null)
    //             {
    //                 trashcanController = trashcanTransform.GetComponent<TrashcanController>();
    //                 if (trashcanController != null)
    //                 {
    //                     if (trashcanController.jenisTempatSampah == inventoryVariable.jenisSampah)
    //                     {

    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogError("Props GameObject not found");
    //         }

        //     if (itemDragHandler.Item.Equals(e.Item))
        //     {
        //         image.enabled = false;
        //         image.sprite = null;
        //         itemDragHandler.Item = null;
        //         break;
        //     }
        // }
    // }

    private void Interact_Trashcan()
    {
        inventory = inventoryPanel.GetComponent<Inventory>();
        Transform imageTransform = inventoryPanel.GetChild(inventory.defaultSelectedItemIndex).GetChild(0);
        Image image = imageTransform.GetChild(0).GetChild(0).GetComponent<Image>();
        InventoryVariable inventoryVariable = imageTransform.GetChild(0).GetComponent<InventoryVariable>();

        trashcanController = _colliders[0].GetComponent<TrashcanController>();
        if (trashcanController != null)
        {
            if (inventoryVariable.jenisSampah != "")
            {
                if (trashcanController.jenisTempatSampah == inventoryVariable.jenisSampah)
                {
                    image.enabled = false;
                    image.sprite = null;
                    inventoryVariable.jenisSampah = "";
                    mainChar.countCoin(5);
                    Debug.Log("Buang Sampah");
                }
                else
                {
                    StartCoroutine(time_delay(mainChar.notificationPanel, 2f, "Jenis Sampah Tidak Sesuai"));
                    Debug.Log("Gagal Buang Sampah");
                }
            }
            else 
            {
                Debug.Log("Pembuangan Sampah Tidak Berhasil");
            }
        }
    }

    IEnumerator time_delay(TextMeshProUGUI notificationPanel, float delayTime, string notificationText)
    {
        notificationPanel.gameObject.SetActive(true);
        notificationPanel.text = notificationText;
        yield return new WaitForSeconds(delayTime);
        notificationPanel.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactPoint.position, _interactPointRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BersihSampah"))
        {
            mainChar.mulaiMisiBtn.SetActive(true);
            BersihSungai bersihSungaiScript = other.transform.parent.GetComponent<BersihSungai>();
            newPosition = bersihSungaiScript.playerPositionChange.transform.position;
            newRotation = bersihSungaiScript.playerPositionChange.transform.rotation.eulerAngles;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mainChar.mulaiMisiBtn.SetActive(false);
        newPosition = Vector3.zero;
        newRotation = Vector3.zero;
    }

    public void Mulai_Misi()
    {
        try
        {
            oldPosition = gc.mainCharacter.transform.position;
            oldRotation = new Vector3(0f, gc.mainCharacter.transform.eulerAngles.y, 0f);

            mainChar.controller.enabled = false;
            gc.mainCharacter.transform.position = newPosition;
            gc.mainCharacter.transform.rotation = Quaternion.Euler(newRotation);
            mainChar.controller.enabled = true;

            gc.mainCamera.gameObject.SetActive(false);
            gc.camera2.gameObject.SetActive(true);

            mainChar.playerCamera = gc.camera2;
            isQuestStart = true;

            gc.mainUI.SetActive(false);
            gc.bersihSungaiUI.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Gagal mengubah posisi karakter: " + e.Message);
        }
    }

    public void Selesai_Misi()
    {
        try
        {
            mainChar.controller.enabled = false;
            gc.mainCharacter.transform.position = oldPosition;
            gc.mainCharacter.transform.localRotation = Quaternion.Euler(oldRotation);
            mainChar.controller.enabled = true;

            gc.mainCamera.gameObject.SetActive(true);
            gc.camera2.gameObject.SetActive(false);
            oldPosition = Vector3.zero;
            oldRotation = Vector3.zero;

            mainChar.playerCamera = gc.mainCamera;
            isQuestStart = false;

            gc.mainUI.SetActive(true);
            gc.bersihSungaiUI.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Gagal mengubah posisi karakter: " + e.Message);
        }
    }
}
