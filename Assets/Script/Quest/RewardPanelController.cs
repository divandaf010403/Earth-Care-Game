using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanelController : MonoBehaviour
{
    public static RewardPanelController Instance;
    [SerializeField] GameObject rewardPanel;
    [SerializeField] GameObject rewardBg;

    private void Awake() 
    {
        // Pastikan hanya ada satu instance QuestManager yang ada
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void getReward(RewardSystem myReward)
    {
        Transform rewardMain = rewardPanel.transform.GetChild(0).GetChild(0);

        foreach (Transform child in rewardMain)
        {
            Destroy(child.gameObject);
        }

        foreach (var reward in myReward.reward)
        {
            GameObject rewardItem = Instantiate(rewardBg, rewardMain);

            Image rewardImage = rewardItem.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI rewardText = rewardItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // Menampilkan gambar dari InventoryExtItemData menggunakan GetImage()
            rewardImage.sprite = reward.GetImage();

            rewardText.text = reward.isCoin ? $"{reward.coinAmount} Coins" : reward.item.jumlahItem.ToString();

            // Simpan Reward
            if (reward.isCoin)
            {
                GameVariable.playerCoin += reward.coinAmount;
                SaveSystem.UpdatePlayerCoin();
            }
            else
            {
                bool itemExists = false;

                // Mengecek apakah item dengan jenisSampah yang sama sudah ada dalam inventaris
                foreach (InventoryExtItemData existingItem in InventoryExt.Instance.inventoryExtItemDataList.slotData)
                {
                    if (existingItem.jenisSampah == reward.item.jenisSampah)
                    {
                        existingItem.jumlahItem += reward.item.jumlahItem; // Menambah jumlahItem
                        itemExists = true;
                        break;
                    }
                }

                // Jika item dengan jenisSampah yang sama tidak ditemukan, tambahkan item baru ke inventaris
                if (!itemExists)
                {
                    InventoryExtItemData inventoryExtItemData = new InventoryExtItemData(
                        InventoryExt.Instance.myItemIdExt, 
                        reward.item.itemName, 
                        reward.item.itemImagePath, // Simpan path dari Sprite
                        reward.item.typeSampah, 
                        reward.item.jenisSampah, 
                        reward.item.jumlahItem
                    );
                    InventoryExt.Instance.inventoryExtItemDataList.slotData.Add(inventoryExtItemData);
                }

                SaveSystem.SaveInventoryExt(InventoryExt.Instance.inventoryExtItemDataList.slotData);

                InventoryExt.Instance.LoadInventoryItem();

                InventoryExt.Instance.IncrementAndSaveItemId();
            }
        }

        rewardPanel.SetActive(true);
        rewardPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
}
