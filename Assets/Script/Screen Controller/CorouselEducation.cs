using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorouselEducation : MonoBehaviour
{
    public static CorouselEducation Instance;

    [Header("Content Vieport")]
    public Image imageParent;
    public List<Sprite> contentPanels;

    // [Header("Navigation Dots")]
    // public GameObject dotsContainer;
    // public GameObject dotPrefab;

    [Header("Pagination Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Page Settings")]
    public bool useTimer = false;
    public bool isLimitedSwipe = false;
    public float autoMoveTime = 5f;
    private float timer;
    public int currentIndex = 0;
    public float swipeThreshold = 50f;
    private Vector2 touchStartPos;

    // Reference to the RectTransform of the content area
    public RectTransform contentArea;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        nextButton.onClick.AddListener(NextContent);
        prevButton.onClick.AddListener(PreviousContent);

        

        // Initialize dots
        // InitializeDots();

        // Display initial content
        // ShowContent();

        // Start auto-move timer if enabled
        if (useTimer)
        {
            timer = autoMoveTime;
            InvokeRepeating("AutoMoveContent", 1f, 1f); // Invoke every second to update the timer
        }
    }

    // void InitializeDots()
    // {
    //     // Create dots based on the number of content panels
    //     for (int i = 0; i < contentPanels.Count; i++)
    //     {
    //         GameObject dot = Instantiate(dotPrefab, dotsContainer.transform);
    //         Image dotImage = dot.GetComponent<Image>();
    //         dotImage.color = (i == currentIndex) ? Color.white : Color.gray;
    //         dotImage.fillAmount = 0f; // Initial fill amount
    //         // You may want to customize the dot appearance and layout here
    //     }
    // }

    // void UpdateDots()
    // {
    //     // Update the appearance of dots based on the current index
    //     for (int i = 0; i < dotsContainer.transform.childCount; i++)
    //     {
    //         Image dotImage = dotsContainer.transform.GetChild(i).GetComponent<Image>();
    //         dotImage.color = (i == currentIndex) ? Color.white : Color.gray;

    //         float targetFillAmount = timer / autoMoveTime;
    //         StartCoroutine(SmoothFill(dotImage, targetFillAmount, 0.5f));
    //     }
    // }

    // IEnumerator SmoothFill(Image image, float targetFillAmount, float duration)
    // {
    //     float startFillAmount = image.fillAmount;
    //     float elapsedTime = 0f;

    //     while (elapsedTime < duration)
    //     {
    //         image.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     image.fillAmount = targetFillAmount; // Ensure it reaches the exact target
    // }

    void AutoMoveContent()
    {
        timer -= 1f; // Decrease timer every second

        if (timer <= 0)
        {
            timer = autoMoveTime;
            NextContent();
        }

        // UpdateDots(); // Update dots on every timer tick
    }

    void NextContent()
    {
        currentIndex = (currentIndex + 1) % contentPanels.Count;
        ShowContent();
        // UpdateDots();
    }

    void PreviousContent()
    {
        currentIndex = (currentIndex - 1 + contentPanels.Count) % contentPanels.Count;
        ShowContent();
        // UpdateDots();
    }

    public void ShowContent()
    {
        imageParent.sprite = contentPanels[currentIndex];
        // Activate the current panel and deactivate others
        // for (int i = 0; i < contentPanels.Count; i++)
        // {
        //     imageParent.sprite = contentPanels[i];

            // Update dot visibility and color based on the current active content
            // Image dotImage = dotsContainer.transform.GetChild(i).GetComponent<Image>();

            // if (isActive)
            // {
            //     // Reset timer and fill amount when the content is swiped
            //     timer = autoMoveTime;
            //     dotImage.fillAmount = 1f;
            // }
            // else
            // {
            //     // Set the fill amount to 0 for non-active content
            //     dotImage.fillAmount = 0f;
            // }
        // }


    }

    public void SetCurrentIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < contentPanels.Count)
        {
            currentIndex = newIndex;
            ShowContent();
            // UpdateDots();
        }
    }
}