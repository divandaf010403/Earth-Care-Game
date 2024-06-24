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
    public Sprite rewardImage;

    public Reward(int coinAmount, Sprite rewardImage)
    {
        this.isCoin = true;
        this.coinAmount = coinAmount;
        this.rewardImage = rewardImage;
    }

    public Reward(InventoryExtItemData item, Sprite rewardImage)
    {
        this.isCoin = false;
        this.item = item;
        this.rewardImage = rewardImage;
    }
}