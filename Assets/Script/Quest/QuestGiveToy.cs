using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiveToy : MonoBehaviour
{
    [SerializeField] List<string> toyRequire;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoQuestAndNext(Button dialogButton, Collider other)
    {
        if (toyRequire != null && toyRequire.Count > 0)
        {
            bool allItemsFound = true;

            foreach (string requiredToy in toyRequire)
            {
                bool itemFound = false;

                // Check if the current required item exists in the inventory and accumulate its quantity
                foreach (InventoryExtItemData itemData in InventoryExt.Instance.inventoryExtItemDataList.slotData)
                {
                    if (itemData.jenisSampah == requiredToy)
                    {
                        itemFound = true;
                        break; // No need to continue checking once the item is found
                    }
                }

                if (!itemFound)
                {
                    allItemsFound = false;
                    break; // If any required item is not found, break out of the loop
                }
            }

            if (allItemsFound)
            {
                // Add your repair logic here
                StartCoroutine(GameController.Instance.HandleWithLoadingPanelTransition(() => {
                    // Menghapus listener sebelumnya untuk menghindari penambahan listener ganda
                    dialogButton.onClick.RemoveAllListeners();

                    // Menambahkan listener baru yang akan memanggil startConversation saat tombol diklik
                    dialogButton.onClick.AddListener(() => ConversationStarter.Instance.StartConversation(other));
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
