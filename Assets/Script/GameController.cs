using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Transform mainCharacter;
    public Transform mainCharacterRiverQuest;
    public Transform mainCamera;
    public Transform camera2;

    [Header("Loading")]
    public Image loadingPanel;
    public float fadeDuration = 1f;
    public float fadeSpeedMultiplier = 2f;

    [Header("UI Controller")]
    public GameObject mainUI;
    public GameObject pilahSampahUI;
    public GameObject bersihSungaiUI;

    [Header("Inventory")]
    public Transform inventory;
    public Transform inventoryExt;

    [Header("Shop Panel")]
    public Transform shopPanel;
    public ShopController shopController;

    [Header("Quest Controller")]
    [SerializeField] Transform btnQuitQuest;
    [SerializeField] public Transform questHandler;
    [System.Serializable] public class PanelBeforeStartQuest
    {
        public Transform panelStartQuest;
        public TextMeshProUGUI waktuQuest;
        public TextMeshProUGUI scoreQuest;
        public Image imgRequiredQuest;
        public Button btnStartQuest;
    }
    [SerializeField] public PanelBeforeStartQuest beforeQuest;
    
    [Header("Ketika Quest Selesai")]
    public Transform finishPanel;

    [Header("Ended Demo Panel")]
    public Transform endedDemoPanel;
    CanvasGroup endDemoCanvas;

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

        loadingPanel.gameObject.SetActive(false);

        // LoadPlayer();
    }

    private void Start() {
        endDemoCanvas = endedDemoPanel.GetComponent<CanvasGroup>();
        if (endDemoCanvas == null)
        {
            endDemoCanvas = endedDemoPanel.gameObject.AddComponent<CanvasGroup>();
        }
        endDemoCanvas.alpha = 0f;

        LoadPlayer();

        // Start the coroutine to update transform every 5 seconds
        StartCoroutine(UpdateTransformPeriodically());
    }

    private void Update() 
    {
        btnQuitQuest.gameObject.SetActive(GameVariable.isQuestStarting ? true : false);
    }

    // Save System
    public void SavePlayerData()
    {
        PlayerData data = SaveSystem.LoadPlayerData();
        if (data == null || string.IsNullOrWhiteSpace(JsonUtility.ToJson(data)) || JsonUtility.ToJson(data) == "{}")
        {
            SaveSystem.SavePlayerData(MainCharMovement.Instance);
        }
        else
        {
            SaveSystem.UpdatePlayerTransform(MainCharMovement.Instance);
        }
    }

    // Coroutine to update player transform periodically
    private IEnumerator UpdateTransformPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (!GameVariable.isQuestStarting)
            {
                SavePlayerData();
            }
        }
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayerData();
        if (data != null)
        {
            MainCharMovement.Instance.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            MainCharMovement.Instance.transform.rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);
            MainCharMovement.Instance.playerCoin = data.playerCoin;

            QuestController.Instance.IncreaseObjectiveTutorial(data.questNumber);
        }
        else
        {
            Debug.LogWarning("No saved data found. Initializing to default values.");
            // Initialize default values if needed
        }
    }

    public IEnumerator FadeInLoadingPanel()
    {
        loadingPanel.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color color = loadingPanel.color;

        while (elapsedTime < fadeDuration / fadeSpeedMultiplier)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / (fadeDuration / fadeSpeedMultiplier));
            loadingPanel.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeOutLoadingPanel()
    {
        float elapsedTime = 0f;
        Color color = loadingPanel.color;

        while (elapsedTime < fadeDuration / fadeSpeedMultiplier)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsedTime / (fadeDuration / fadeSpeedMultiplier)));
            loadingPanel.color = color;
            yield return null;
        }
        
        if (loadingPanel.color == new Color(0,0,0,0))
        {
            loadingPanel.gameObject.SetActive(false);
        }
    }

    public void showPanelBeforeQuestStart(IQuestHandler getQuestHandler)
    {
        beforeQuest.panelStartQuest.gameObject.SetActive(true);
        beforeQuest.panelStartQuest.localPosition = new Vector3(0f, 0f, 0f);

        beforeQuest.waktuQuest.text = getQuestHandler.GetWaktuQuest() >= 0 ? getQuestHandler.GetWaktuQuest().ToString() : "-";
        beforeQuest.scoreQuest.text = getQuestHandler.GetScoreQuest() >= 0 ? getQuestHandler.GetScoreQuest().ToString() : "-";

        if (getQuestHandler.GetImageRequiredQuest() != null)
        {
            beforeQuest.imgRequiredQuest.transform.parent.parent.gameObject.SetActive(true);
            beforeQuest.imgRequiredQuest.sprite = getQuestHandler.GetImageRequiredQuest();
        }
        else
        {
            beforeQuest.imgRequiredQuest.transform.parent.parent.gameObject.SetActive(false);
        }

        // Menghapus semua listener sebelumnya sebelum menambahkan yang baru
        beforeQuest.btnStartQuest.onClick.RemoveAllListeners();
        beforeQuest.btnStartQuest.onClick.AddListener(() =>
        {
            StartCoroutine(HandleWithLoadingPanelTransition(() => 
            {
                getQuestHandler.OnQuestStart();
                beforeQuest.panelStartQuest.gameObject.SetActive(false);
            }, null));
        });
    }

    public IEnumerator HandleWithLoadingPanelTransition(System.Action mainOperation, System.Action secondOperation)
    {
        yield return StartCoroutine(FadeInLoadingPanel());

        mainOperation?.Invoke();

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeOutLoadingPanel());

        secondOperation?.Invoke();
    }

    // Akhiri Quest Sebelum Tanpa Menyelesaikan Tantangan
    public void EndQuestButtonClick()
    {
        StartCoroutine(HandleWithLoadingPanelTransition(() =>
        {
            questHandler.GetComponent<IQuestHandler>().OnQuestFinish();

            // QuestController.Instance.ActivateQuest();
        }, () => questHandler = null));
    }

    // Munculkan Panel Selesai Misi ketika Sudah Selesai Tantangan
    public void finishedQuest(int finishScore)
    {
        finishPanel.gameObject.SetActive(true);
        finishPanel.localPosition = new Vector3(0f, 0f, 0f);
        Transform finishPanelChild = finishPanel.GetChild(0);
        
        finishPanelChild.GetChild(0).GetComponent<TextMeshProUGUI>().text = finishScore.ToString();
        finishPanelChild.GetChild(1).GetComponent<TextMeshProUGUI>().text = finishScore >= 50 ? "BERHASIL" : "GAGAL!!!";
        finishPanelChild.GetChild(1).GetComponent<TextMeshProUGUI>().color = finishScore >= 50 ? Color.green : Color.red;
        
        questHandler.GetComponent<IQuestHandler>().OnQuestFinish();

        finishPanel.GetChild(1).GetComponent<Button>().onClick.AddListener(() => 
        {
            finishPanel.gameObject.SetActive(false);
        });
    }

    public void openCloseinventoryExtMerchant(bool isActive) 
    {
        inventoryExt.gameObject.SetActive(isActive);
        inventoryExt.localPosition = new Vector3(0f, 0f, 0f);
        inventoryExt.GetChild(0).GetChild(0).localPosition += new Vector3(50f, 0f, 0f);
        inventoryExt.GetChild(0).GetChild(2).gameObject.SetActive(true);
        if (isActive) 
        {
            inventory.gameObject.SetActive(false);
        }
        else
        {
            inventory.gameObject.SetActive(true);
            inventoryExt.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    public void openCloseInventoryBag(bool isActive) 
    {
        inventoryExt.gameObject.SetActive(isActive);
        inventoryExt.localPosition = new Vector3(0f, 0f, 0f);
        inventoryExt.GetChild(0).GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        inventoryExt.GetChild(0).GetChild(2).gameObject.SetActive(false);
        inventory.gameObject.SetActive(true);
    }

    public void ShowShopPanel()
    {
        shopPanel.gameObject.SetActive(true);
        shopPanel.localPosition = new Vector3(0f, 0f, 0f);
        shopController.LoadShopData();
    }

    public void CloseShopPanel()
    {
        shopPanel.gameObject.SetActive(false);
        if (shopController != null && shopController.ShopScrollView != null)
        {
            foreach (Transform child in shopController.ShopScrollView)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogError("Invalid shop or ShopScrollView reference.");
        }
    }

    public void ShowEndedDemoPanel()
    {
        StartCoroutine(FadeInPanelEndDemo());
    }

    private IEnumerator FadeInPanelEndDemo()
    {
        float duration = 1f;  // Durasi fade in
        float currentTime = 0f;

        endedDemoPanel.localPosition = new Vector3(0f, 0f, 0f);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            endDemoCanvas.alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            yield return null;
        }

        endDemoCanvas.alpha = 1f;

        // Tunggu selama 5 detik sebelum pindah ke scene "Main Menu"
        yield return new WaitForSeconds(5f);

        // Pindah ke scene "Main Menu"
        SceneManager.LoadScene("MainMenu");
    }
}
