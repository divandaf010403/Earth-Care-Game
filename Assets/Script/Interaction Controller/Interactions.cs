using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Interactions : MonoBehaviour
{
    [SerializeField] private Transform _interactPoint;
    [SerializeField] private float _interactPointRadius;
    [SerializeField] private LayerMask _interactLayerMask;
    [SerializeField] private LayerMask _questLayerMask;
    private readonly Collider[] _colliders = new Collider[1];
    [SerializeField] private int _numFound;
    [SerializeField] public GameObject buttonInteract;
    public List<Sprite> imgAction;

    [Header("Get Component")]
    MainCharMovement mainChar;
    public Inventory inventory;
    public InventoryExt inventoryExt;
    TrashcanController trashcanController;

    [Header("Inventory")]
    Transform otherGameObject;
    Transform inventoryPanel;
    public Transform inventoryExtPanel;

    [Header("Ambil Variabel")]
    GameController gc;

    [Header("Main Character Settings")]
    [SerializeField] public Vector3 newPosition;
    [SerializeField] public Vector3 newRotation;
    [SerializeField] public Vector3 oldPosition = Vector3.zero;
    [SerializeField] public Vector3 oldRotation = Vector3.zero;
    [SerializeField] public Vector3 cameraSetPosition = Vector3.zero;
    [SerializeField] public Vector3 cameraSetRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        otherGameObject = GameObject.Find("Screen").transform.GetChild(0);
        inventoryPanel = otherGameObject.transform.Find("Inventory");
        GameObject gameController = GameObject.Find("GameController");

        mainChar = GetComponent<MainCharMovement>();
        gc = gameController.GetComponent<GameController>();
        buttonInteract.SetActive(false);

        // inventory.ItemAdded += InventoryScript_ItemAdded;
        // inventoryExt.ItemAdded += InventoryExtScript_ItemAdded;
    }

    // Update is called once per frame
    void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(
            _interactPoint.position,
            _interactPointRadius,
            _colliders,
            _interactLayerMask
        );

        if (_numFound > 0)
        {
            Image setActionImg = buttonInteract.transform.GetChild(0).GetComponent<Image>();

            switch (_colliders[0].tag)
            {
                case "Item":
                case "ItemCraft":
                case "Merchant":
                    buttonInteract.SetActive(true);
                    setActionImg.sprite = imgAction[0];
                    break;
                case "Trashcan":
                    buttonInteract.SetActive(true);
                    setActionImg.sprite = imgAction[1];
                    break;
                default:
                    buttonInteract.SetActive(false);
                    break;
            }
        }
        else
        {
            buttonInteract.SetActive(false);
        }
    }

    public void buttonCondition()
    {
        if (_numFound > 0)
        {
            switch (_colliders[0].tag)
            {
                case "Item":
                    removeItem();
                    break;
                case "ItemCraft":
                    removeItemCraft();
                    break;
                case "Trashcan":
                    TrashcanController trashcanController = _colliders[0].GetComponent<TrashcanController>();
                    if (GameVariable.isQuestStarting && (GameVariable.questId == "1Q"))
                    {
                        PilahSampah pilahSampah = FindObjectOfType<PilahSampah>();
                        pilahSampah.Buang_Sampah(trashcanController);
                    }
                    else
                    {
                        inventory.RemoveItem(trashcanController);
                    }
                    break;
                case "Merchant":
                    gc.openCloseinventoryExtMerchant(true);
                    break;
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
            if (GameVariable.isQuestStarting && GameVariable.questId == "1Q")
            {
                PilahSampah pilahSampah = FindObjectOfType<PilahSampah>();
                pilahSampah.Ambil_Sampah(item);
            }
            else
            {
                inventory.AddItem(item);
            }
        }
    }
    
    public void removeItemCraft()
    {
        var interactableItem = _colliders[0].GetComponent<Interactable>();
        IInventoryItem item = _colliders[0].GetComponent<IInventoryItem>();

        if (interactableItem != null)
        {
            interactableItem.Interact(this);
        }

        if (item != null)
        {
            inventoryExt.AddItem(item);
        }
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
