using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
	public GameObject loaderUI;
	public GameObject playExitBtn;
	public Slider progressSlider;

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(ChangeScene_Numerator(sceneIndex));
    }

    private IEnumerator ChangeScene_Numerator(int index)
    {
        loaderUI.SetActive(true);
        playExitBtn.SetActive(false);

        Debug.Log("Memulai AsyncOperation untuk memuat scene: " + index);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        float progress = 0;

        while (!asyncOperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncOperation.progress < 0.9f ? asyncOperation.progress : 1f, Time.deltaTime);
            progressSlider.value = progress;
            Debug.Log("Progres loading: " + progress);

            if (asyncOperation.progress >= 0.9f && progress >= 0.9f)
            {
                Debug.Log("Loading selesai. Mengaktifkan scene.");
                progressSlider.value = 1;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        // Reset elemen UI setelah loading selesai
        loaderUI.SetActive(false);
        playExitBtn.SetActive(true);
    }
	
	public void Exit()
	{
		Application.Quit();
	}
}
