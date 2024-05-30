using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public Transform mainCharacter;
    public Transform mainCharacterRiverQuest;
    public Transform mainCamera;
    public Transform camera2;

    [Header("Loading")]
    public Transform loadingPanel;
    private CanvasGroup loadingCanvasGroup;

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

        loadingCanvasGroup = loadingPanel.GetComponent<CanvasGroup>();
        if (loadingCanvasGroup == null)
        {
            loadingCanvasGroup = loadingPanel.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update() 
    {
        btnQuitQuest.gameObject.SetActive(GameVariable.isQuestStarting ? true : false);
    }

    public void LoadingPanelTransition(float duration = 1.0f, bool fadeIn = true)
    {
        if (fadeIn)
        {
            StartCoroutine(FadeIn(duration));
        }
        else
        {
            StartCoroutine(FadeOut(duration));
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            loadingCanvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);
            yield return null;
        }
        loadingCanvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut(float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            loadingCanvasGroup.alpha = Mathf.Lerp(1, 0, counter / duration);
            yield return null;
        }
        loadingCanvasGroup.alpha = 0;
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
            getQuestHandler.OnQuestStart();
            beforeQuest.panelStartQuest.gameObject.SetActive(false);
        });
    }

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
        inventoryExt.GetChild(0).localPosition += new Vector3(50f, 0f, 0f);
        inventoryExt.GetChild(2).gameObject.SetActive(true);
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
        inventoryExt.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        inventoryExt.GetChild(2).gameObject.SetActive(false);
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
}
