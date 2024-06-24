using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RequiredItem
{
    public string itemName;
    public int quantity;
    public int nextQuest = 8;

    public RequiredItem(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }
}

public class QuestSaluranAir : MonoBehaviour, IQuestFinishHandler
{
    [SerializeField] Button btnPerbaiki;
    [SerializeField] List<RequiredItem> requiredRepair;
    public bool isFinished;
    [SerializeField] GameObject saluranRusak;
    [SerializeField] GameObject saluranPerbaikan;

    [Header("Next Action")]
    [SerializeField] Transform nextTranformToActive;

    private void LateUpdate() 
    {
        if (isFinished) 
        {
            saluranRusak.SetActive(false);
            saluranPerbaikan.SetActive(true);
        }
        else
        {
            saluranRusak.SetActive(true);
            saluranPerbaikan.SetActive(false);
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

            // Iterate through each required item and its quantity
            foreach (RequiredItem requiredItem in requiredRepair)
            {
                int totalCount = 0;

                // Check if the current required item exists in the inventory and accumulate its quantity
                foreach (InventoryExtItemData itemData in InventoryExt.Instance.inventoryExtItemDataList.slotData)
                {
                    if (itemData.jenisSampah == requiredItem.itemName)
                    {
                        totalCount += itemData.jumlahItem;
                    }
                }

                // If the total quantity of the required item is less than needed, set allItemsFound to false
                if (totalCount < requiredItem.quantity)
                {
                    allItemsFound = false;
                    break; // Exit the loop early since we already know not all items are found
                }
            }

            if (allItemsFound)
            {
                // Add your repair logic here
                StartCoroutine(GameController.Instance.HandleWithLoadingPanelTransition(() => {
                    QuestController.Instance.getChildNumberNextQuest(nextTranformToActive);
                }, null));
            }
            else
            {
                MainCharMovement.Instance.showNotification("Item Yang Diperlukan Kurang");
            }
        }
        else
        {
            // Logic for when there are no required items
            Debug.Log("No required items specified.");
        }
    }
}
