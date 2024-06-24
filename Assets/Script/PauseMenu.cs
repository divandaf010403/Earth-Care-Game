using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;

    private void Update() 
    {
        
    }

    public void Pause()
    {
        PausePanel.transform.localPosition = new Vector3(0f, 0f, 0f);
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenu(int sceneIndex)
    {
        StartCoroutine(ChangeScene_Numerator(sceneIndex));
    }

    private IEnumerator ChangeScene_Numerator(int index)
    {

        Debug.Log("Memulai AsyncOperation untuk memuat scene: " + index);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // Ketika loading sudah mencapai 90%, kita menunggu hingga kita siap mengaktifkan scene.
            if (asyncOperation.progress >= 0.9f && !asyncOperation.allowSceneActivation)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Tunggu sejenak sebelum mereset UI
        yield return new WaitForSeconds(0.2f);
    }
}
