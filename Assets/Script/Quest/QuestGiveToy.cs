using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiveToy : MonoBehaviour
{
    [SerializeField] List<string> toyRequire;
    public Transform inventoryExtMain;
    [SerializeField] GameObject hideCanvas;
    [SerializeField] GameObject[] bocilActivate;

    // Start is called before the first frame update
    void Start()
    {
        bocilActivate[0].SetActive(true);
        bocilActivate[1].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DoQuestAndNext(System.Action startConversation)
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
                foreach (string requiredToy in toyRequire)
                {
                    // Iterate through each item in the inventory
                    for (int i = InventoryExt.Instance.inventoryExtItemDataList.slotData.Count - 1; i >= 0; i--)
                    {
                        InventoryExtItemData existingItem = InventoryExt.Instance.inventoryExtItemDataList.slotData[i];
                        
                        // Check if the item name matches
                        if (existingItem.jenisSampah == requiredToy)
                        {
                            InventoryExt.Instance.inventoryExtItemDataList.slotData.RemoveAt(i);
                        }
                    }
                }

                for(int j = 0; j < inventoryExtMain.childCount; j++)
                {
                    inventoryExtMain.GetChild(j).GetChild(0).GetComponent<Image>().enabled = false;
                    inventoryExtMain.GetChild(j).GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                }

                // Hide Canvas
                hideCanvas.SetActive(false);

                //Change Bocil
                bocilActivate[0].SetActive(false);
                bocilActivate[1].SetActive(true);

                // Add your repair logic here
                startConversation?.Invoke();

                // Save the updated inventory to the JSON file
                SaveSystem.SaveInventoryExt(InventoryExt.Instance.inventoryExtItemDataList.slotData);
            }
            else
            {
                MainCharMovement.Instance.showNotification("Mainan Perahu Belum Dibuat");
            }
        }
        else
        {
            // Logic for when there are no required items
            Debug.Log("No required items specified.");
        }

        yield return null;
    }
}
