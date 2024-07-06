using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RequiredItem
{
    public Sprite imgRequire;
    public string itemName;
    public int quantity;

    public RequiredItem(Sprite ImgRequire, string itemName, int quantity)
    {
        this.imgRequire = ImgRequire;
        this.itemName = itemName;
        this.quantity = quantity;
    }
}

public class QuestSaluranAir : MonoBehaviour, IQuestFinishHandler
{
    [SerializeField] Button btnPerbaiki;
    [SerializeField] List<RequiredItem> requiredRepair;
    [SerializeField] List<GameObject> canvasToSetItem;
    public bool isFinished;

    private void Start() 
    {
        for (int i = 0; i < canvasToSetItem.Count; i++)
        {
            canvasToSetItem[i].transform.GetChild(0).GetComponent<Image>().sprite = requiredRepair[i].imgRequire;
            canvasToSetItem[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = requiredRepair[i].quantity.ToString();
        }
    }

    public bool IsQuestFinished
    {
        set{ isFinished = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            btnPerbaiki.gameObject.SetActive(true);
            btnPerbaiki.onClick.RemoveAllListeners();
            btnPerbaiki.onClick.AddListener(CheckRequiredItems);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        btnPerbaiki.gameObject.SetActive(false);
    }

    private void CheckRequiredItems()
    {
        if (requiredRepair != null && requiredRepair.Count > 0)
        {
            bool allItemsFound = true;

            foreach (RequiredItem requiredItem in requiredRepair)
            {
                int totalCount = 0;

                foreach (InventoryExtItemData itemData in InventoryExt.Instance.inventoryExtItemDataList.slotData)
                {
                    if (itemData.jenisSampah == requiredItem.itemName)
                    {
                        totalCount += itemData.jumlahItem;
                    }
                }

                if (totalCount < requiredItem.quantity)
                {
                    allItemsFound = false;
                    break;
                }
            }

            if (allItemsFound)
            {
                // Panggil coroutine dari CoroutineManager
                CoroutineManager.Instance.StartCoroutine(GameController.Instance.HandleWithLoadingPanelTransition(() =>
                {
                    QuestController.Instance.getChildNumberNextQuest(transform);
                    Debug.Log("getChildNumberNextQuest completed");
                }, null));
            }
            else
            {
                MainCharMovement.Instance.showNotification("Item Yang Diperlukan Kurang");
            }
        }
        else
        {
            Debug.Log("No required items specified.");
        }
    }
}
