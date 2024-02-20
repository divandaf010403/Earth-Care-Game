//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SceneManager : MonoBehaviour
//{
//    public void SavePlayer()
//    {
//        SaveSystem.SavePlayer(this);
//    }

//    public void LoadPlayer()
//    {
//        StartCoroutine(LoadPlayerCoroutine());
//    }

//    private IEnumerator LoadPlayerCoroutine()
//    {
//        yield return null; // Delay untuk memberikan kesempatan layar loading untuk tampil

//        PlayerData data = SaveSystem.LoadPlayer();

//        if (data != null)
//        {
//            // Set position
//            Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
//            transform.position = position;

//            // Set rotation
//            Quaternion rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);
//            transform.rotation = rotation;

//            Debug.Log("Posisi" + position);
//            Debug.Log("Rotasi" + rotation);
//        }
//    }
//}
