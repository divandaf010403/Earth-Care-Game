using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using TMPro;
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
    public PrefabManager prefabManager;

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
                case "QuestPLTSa":
                case "VendingMachine":
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
            TrashcanController trashcanController = _colliders[0].GetComponent<TrashcanController>();
            TrashManager trashManager = _colliders[0].GetComponent<TrashManager>();
            PLTSaQuest q2 = _colliders[0].GetComponent<PLTSaQuest>();
            switch (_colliders[0].tag)
            {
                case "Item":
                    removeItem();
                    if(!trashManager.itemId.StartsWith("Item_Trash Beach Trash(Clone)"))
                    {
                        trashManager.TakeItem();
                    }
                    break;
                case "ItemCraft":
                    CollectibleItem collectibleItem = _colliders[0].GetComponent<CollectibleItem>();
                    collectibleItem.TakeItem();
                    
                    removeItemCraft();
                    break;
                case "Trashcan":
                    if (GameVariable.isQuestStarting && (GameVariable.questId == "1Q"))
                    {
                        PilahSampah pilahSampah = FindObjectOfType<PilahSampah>();
                        pilahSampah.Buang_Sampah(trashcanController, mainChar);
                    }
                    else
                    {
                        bool isRemoved = inventory.RemoveItem(trashcanController,  mainChar);
                        if (isRemoved)
                        {
                            Debug.Log("Item was successfully removed.");
                        }
                        else
                        {
                            // Handle failure to remove item if needed
                            Debug.Log("Failed to remove item.");
                        }
                    }
                    break;
                case "QuestPLTSa":
                    if (q2.requiredItem < q2.totalRequireItem)
                    {
                        Debug.Log(q2.requiredItem + " <=> " + q2.totalRequireItem);
                        bool isRemovedQuest = inventory.RemoveItemQuest(q2,  mainChar);
                        if (isRemovedQuest)
                        {
                            q2.requiredItem++;
                            PlayerPrefs.SetInt(q2.tipePenampungan + " PLTSa", q2.requiredItem);
                            Debug.Log("Item was successfully removed.");
                        }
                        else
                        {
                            // Handle failure to remove item if needed
                            Debug.Log("Failed to remove item.");
                        }
                    }
                    break;
                case "VendingMachine":
                    bool isRemovedonVendingMachiine = inventory.RemoveItemVendingMachine(trashcanController, mainChar);
                    if (isRemovedonVendingMachiine)
                    {
                        Debug.Log("Item was successfully removed.");
                    }
                    else
                    {
                        // Handle failure to remove item if needed
                        Debug.Log("Failed to remove item.");
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
                if (GameVariable.questId == "5Q1" && inventory.inventoryItemDataList.slotData.Count < inventory.SLOTS)
                {
                    RemoveObjectOnPrefab();
                }
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

    void RemoveObjectOnPrefab()
    {
        TrashManager trashManager = _colliders[0].GetComponent<TrashManager>();
        if (trashManager != null)
        {
            prefabManager.UpdateStatus(trashManager.itemId, false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactPoint.position, _interactPointRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
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
            Button dialogButton = mainChar.newDictionary["dialog"].GetComponent<Button>();

            QuestGiveToy questGiveToy = other.transform.parent.GetComponent<QuestGiveToy>();

            // Menghapus listener sebelumnya untuk menghindari penambahan listener ganda
            dialogButton.onClick.RemoveAllListeners();

            // Menambahkan listener baru yang akan memanggil startConversation saat tombol diklik
            dialogButton.onClick.AddListener(() => {
                if (questGiveToy != null)
                {
                    CoroutineManager.Instance.StartCoroutine(questGiveToy.DoQuestAndNext(() => ConversationStarter.Instance.StartConversation(other)));
                    Debug.Log("Mulai Dialog adalah quest");
                }
                else
                {
                    ConversationStarter.Instance.StartConversation(other);
                    Debug.Log("Mulai Dialog Bukan Quest");
                }
            });

            // dialogButton.onClick.AddListener(() => {
            //     ConversationStarter.Instance.StartConversation(other);
            // });
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
                // triggerable.OnQuestStart();

                GameController.Instance.questHandler = triggerable.GetTransform();
                CorouselTutorial.Instance.contentPanels = triggerable.imgTutorialList();
                CorouselTutorial.Instance.ShowContent();
                GameController.Instance.showPanelBeforeQuestStart(triggerable);

                Debug.Log("Mulai Quest");
            }
        }
    }
}
