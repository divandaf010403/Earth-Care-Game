using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // ID unik untuk item ini
    public string itemId;

    void Start()
    {
        // Cek apakah item ini sudah diambil sebelumnya
        if (PlayerPrefs.GetInt(itemId, 0) == 1)
        {
            // Jika sudah diambil, sembunyikan item ini
            gameObject.SetActive(false);
        }
    }

    // Fungsi ini akan dipanggil dari Interaction.cs
    public void TakeItem()
    {
        // Menyimpan status bahwa item ini telah diambil
        PlayerPrefs.SetInt(itemId, 1);

        // Menyembunyikan atau menghancurkan item ini
        gameObject.SetActive(false);
    }
}
