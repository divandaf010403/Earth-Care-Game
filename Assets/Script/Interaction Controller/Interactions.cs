using System.Collections;
using Cinemachine.Utility;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Interactions : MonoBehaviour
{
    [SerializeField]
    private Transform _interactPoint;

    [SerializeField]
    private float _interactPointRadius;

    [SerializeField]
    private LayerMask _interactLayerMask;

    [SerializeField]
    private LayerMask _questLayerMask;

    private readonly Collider[] _colliders = new Collider[1];

    [SerializeField]
    private int _numFound;

    [SerializeField]
    public GameObject buttonInteract;

    [Header("Get Component")]
    MainCharMovement mainChar;
    public Inventory inventory;
    TrashcanController trashcanController;

    [Header("Inventory")]
    Transform otherGameObject;
    Transform inventoryPanel;
    public Transform inventoryExtPanel;

    [Header("Ambil Variabel")]
    GameController gc;

    [Header("Main Character Settings")]
    [SerializeField]
    public Vector3 newPosition;

    [SerializeField]
    public Vector3 newRotation;

    [SerializeField]
    public Vector3 oldPosition = Vector3.zero;

    [SerializeField]
    public Vector3 oldRotation = Vector3.zero;

    [SerializeField]
    public Vector3 cameraSetPosition = Vector3.zero;

    [SerializeField]
    public Vector3 cameraSetRotation = Vector3.zero;

    [Header("Quest")]
    public bool isQuestStart = false;

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
    }

    // Update is called once per frame
    void Update()
    {
        // Ambil Sampah
        _numFound = Physics.OverlapSphereNonAlloc(
            _interactPoint.position,
            _interactPointRadius,
            _colliders,
            _interactLayerMask
        );

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
        if (e.Item.jenisSampah == "Trash")
        {
            foreach (Transform slot in inventoryPanel)
            {
                // Check if the slot has any children
                if (slot.childCount > 0)
                {
                    Transform imageTransform = slot.GetChild(0).GetChild(0);
                    Image image = imageTransform.GetChild(0).GetComponent<Image>();
                    // ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
                    InventoryVariable inventoryVariable =
                        imageTransform.GetComponent<InventoryVariable>();

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
        else if (e.Item.typeSampah == "Collectible")
        {
            Debug.Log(inventoryExtPanel.GetChild(0).childCount);
            foreach (Transform slot in inventoryExtPanel)
            {
                if (slot.GetChild(0).childCount > 0)
                {
                    Transform imageTransform = slot.GetChild(0).GetChild(0);
                    Image image = imageTransform.GetComponent<Image>();
                    ItemDragHandler itemDragHandler =
                        imageTransform.GetComponent<ItemDragHandler>();
                    InventoryVariable inventoryVariable =
                        imageTransform.GetComponent<InventoryVariable>();

                    if (!image.enabled)
                    {
                        image.enabled = true;
                        image.sprite = e.Item.image;

                        inventoryVariable.jenisSampah = e.Item.jenisSampah;

                        //itemDragHandler.Item = e.Item;

                        if (mainChar != null)
                        {
                            mainChar.cubeVal++;
                        }

                        break;
                    }
                }
            }
        }
    }

    private void Interact_Trashcan()
    {
        inventory = inventoryPanel.GetComponent<Inventory>();
        Transform imageTransform = inventoryPanel
            .GetChild(inventory.defaultSelectedItemIndex)
            .GetChild(0);
        Image image = imageTransform.GetChild(0).GetChild(0).GetComponent<Image>();
        InventoryVariable inventoryVariable = imageTransform
            .GetChild(0)
            .GetComponent<InventoryVariable>();

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
                    StartCoroutine(
                        time_delay(mainChar.notificationPanel, 2f, "Jenis Sampah Tidak Sesuai")
                    );
                    Debug.Log("Gagal Buang Sampah");
                }
            }
            else
            {
                Debug.Log("Pembuangan Sampah Tidak Berhasil");
            }
        }
    }

    IEnumerator time_delay(
        TextMeshProUGUI notificationPanel,
        float delayTime,
        string notificationText
    )
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
        if (other.CompareTag("Quest"))
        {
            mainChar.mulaiMisiBtn.SetActive(true);
            IQuestHandler questHandler = other.GetComponentInParent<IQuestHandler>();
            newPosition = questHandler.QuestPlayerPosition.transform.position;
            newRotation = questHandler.QuestPlayerPosition.transform.rotation.eulerAngles;
            cameraSetPosition = questHandler.QuestCameraPosition.transform.position;
            cameraSetRotation = questHandler.QuestCameraPosition.transform.rotation.eulerAngles;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mainChar.mulaiMisiBtn.SetActive(false);
        newPosition = Vector3.zero;
        newRotation = Vector3.zero;
    }

    public void QuestButtonClick()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, _questLayerMask);

        foreach (Collider collider in colliders)
        {
            IQuestHandler triggerable = collider.GetComponentInParent<IQuestHandler>();
            if (triggerable != null)
            {
                triggerable.OnQuestStart();
            }
        }
    }
}
