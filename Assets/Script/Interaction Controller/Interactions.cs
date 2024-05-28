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
    public List<Sprite> imgAction;

    [Header("Get Component")]
    MainCharMovement mainChar;
    
    public Inventory inventory;
    public InventoryExt inventoryExt;

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

        Transform btn_i = mainChar.newDictionary["interact"];

        if (_numFound > 0)
        {
            Image setActionImg = btn_i.transform.GetChild(0).GetComponent<Image>();

            switch (_colliders[0].tag)
            {
                case "Item":
                case "ItemCraft":
                case "Merchant":
                    btn_i.gameObject.SetActive(true);
                    setActionImg.sprite = imgAction[0];
                    break;
                case "Trashcan":
                case "Quest":
                    btn_i.gameObject.SetActive(true);
                    setActionImg.sprite = imgAction[1];
                    break;
                default:
                    btn_i.gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            btn_i.gameObject.SetActive(false);
        }
        
        if(GameVariable.isQuestStarting)
        {
            mainChar.newDictionary["quest"].gameObject.SetActive(false);
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
                        pilahSampah.Buang_Sampah(trashcanController, mainChar);
                    }
                    else
                    {
                        bool isRemoved = inventory.RemoveItem(trashcanController, mainChar);
                        if (isRemoved)
                        {
                            if (_colliders[0].CompareTag("Quest"))
                            {
                                PLTSaQuest q2 = _colliders[0].GetComponent<PLTSaQuest>();
                                q2.requiredItem++;
                            }
                            Debug.Log("Item was successfully removed.");
                        }
                        else
                        {
                            // Handle failure to remove item if needed
                            Debug.Log("Failed to remove item.");
                        }
                    }
                    break;
                case "Merchant":
                    gc.openCloseinventoryExtMerchant(true);
                    break;
            }
        }
        else
        {
            mainChar.newDictionary["interact"].gameObject.SetActive(false);
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
            mainChar.newDictionary["quest"].gameObject.SetActive(true);
            IQuestHandler questHandler = other.GetComponentInParent<IQuestHandler>();
            newPosition = questHandler.QuestPlayerPosition.transform.position;
            newRotation = questHandler.QuestPlayerPosition.transform.rotation.eulerAngles;
            cameraSetPosition = questHandler.QuestCameraPosition.transform.position;
            cameraSetRotation = questHandler.QuestCameraPosition.transform.rotation.eulerAngles;
        }

        if (other.CompareTag("Conversation"))
        {
            mainChar.newDictionary["dialog"].gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Quest"))
        {
            mainChar.newDictionary["quest"].gameObject.SetActive(false);
            newPosition = Vector3.zero;
            newRotation = Vector3.zero;
        }

        if (other.CompareTag("Conversation"))
        {
            mainChar.newDictionary["dialog"].gameObject.SetActive(false);
        }
    }

    public void QuestButtonClick()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, _questLayerMask);

        foreach (Collider collider in colliders)
        {
            IQuestHandler triggerable = collider.GetComponentInParent<IQuestHandler>();
            if (triggerable != null)
            {
                GameController.Instance.questHandler = triggerable.GetTransform();
                GameController.Instance.showPanelBeforeQuestStart();
            }
        }
    }

    public void EndQuestButtonClick()
    {
        GameController.Instance.questHandler.GetComponent<IQuestHandler>().OnQuestFinish();
        GameController.Instance.questHandler = null;

        QuestController.Instance.ActivateQuest();
    }
}
