using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public GameObject loaderUI;
    public GameObject playExitBtn;
    public Slider progressSlider;

    private void Start() 
    {
        ResetLoader();
    }

    void ResetLoader()
    {
        progressSlider.value = 0;
        loaderUI.SetActive(false);
        playExitBtn.SetActive(true);
    }

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(ChangeScene_Numerator(sceneIndex));

        // SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator ChangeScene_Numerator(int index)
    {
        loaderUI.SetActive(true);
        playExitBtn.SetActive(false);

        Debug.Log("Memulai AsyncOperation untuk memuat scene: " + index);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;
        float progress = 0;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress < 0.9f)
            {
                progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
            }
            else
            {
                progress = Mathf.MoveTowards(progress, 1f, Time.deltaTime);
            }

            progressSlider.value = progress;
            Debug.Log("Progres loading: " + progress);

            if (progress >= 0.9f && asyncOperation.progress >= 0.9f)
            {
                Debug.Log("Loading selesai. Mengaktifkan scene.");
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene dimuat. Mengatur ulang UI.");
        yield return new WaitForSeconds(0.2f);

        // Reset elemen UI setelah loading selesai
        ResetLoader();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
