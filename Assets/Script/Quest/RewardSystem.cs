using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    [SerializeField] public List<Reward> reward;
}

[System.Serializable]
public class Reward
{
    public bool isCoin;
    public int coinAmount;
    public InventoryExtItemData item;
    public string rewardImagePath; // Menyimpan path dari sprite
    [System.NonSerialized] public Sprite rewardImage; // Sprite yang diinstansiasi dari path

    public Reward(int coinAmount, string rewardImagePath)
    {
        this.isCoin = true;
        this.coinAmount = coinAmount;
        this.rewardImagePath = rewardImagePath;
    }

    public Reward(InventoryExtItemData item, string rewardImagePath)
    {
        this.isCoin = false;
        this.item = item;
        this.rewardImagePath = rewardImagePath;
    }

    // Method untuk memuat sprite dari path
    public Sprite GetImage()
    {
        if (rewardImage == null)
        {
            rewardImage = Resources.Load<Sprite>(rewardImagePath);
        }
        return rewardImage;
    }
}