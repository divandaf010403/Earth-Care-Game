using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCharMovement : MonoBehaviour
{
    [Header("Character Utils")]
    public CharacterController controller;
    public Transform playerCamera;
    public CinemachineFreeLook camFreeLook;
    public float raycastDistance = 1f;
    public LayerMask groundLayer;
    public float rotationSpeedX = 2f;
    public float rotationSpeedY = 0.5f;
    public float gravityValue = -9.8f;
    public int playerCoin = 0;
    public TextMeshProUGUI playerCoinTxt;

    [Header("Interaction Utils")]
    public float interactRadius = 2f;
    // public Transform notificationPanel;

    [Header("Joystick Utils")]
    public bool enableMobileInput = false;
    public FixedJoystick[] joysticks;
    public FixedTouchField touchField;
    GameController gc;
    public Animator anim;
    // public GameObject loadingPanel;
    // public GameObject shopPanel;
    // public GameObject mulaiMisiBtn;

    [Header("Movement Condition")]
    public bool isMoveLeft = false;
    public bool isMoveRIght = false;

    [Header("Shop Controller")]
    public ShopController shop;

    [Header("Coba Dictionary")]
    [SerializeField] public CustomDictionary customDictionary;
    [SerializeField] public Dictionary<string, Transform> newDictionary;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameController = GameObject.Find("GameController");

        controller = GetComponent<CharacterController>();
        gc = gameController.GetComponent<GameController>();
        anim = GetComponent<Animator>();

        // notificationPanel.gameObject.SetActive(false);
        // loadingPanel.SetActive(false);
        // shopPanel.SetActive(false);
        // mulaiMisiBtn.SetActive(false);

        newDictionary = customDictionary.ToDictionary();

        foreach (var kvp in newDictionary)
        {
            kvp.Value.gameObject.SetActive(false);
        }

        LoadPlayer();

        InvokeRepeating("SavePlayerData", 0f, 5f);
    }

    private void Update()
    {
        if (enableMobileInput)
        {
            analogInput();
        }
        else
        {
            keyboardInput();
        }

        if (isMoveLeft)
        {
            MoveCharacterRightAndLeft(-1f);
        }
        else if (isMoveRIght)
        {
            MoveCharacterRightAndLeft(1f);
        }
        else
        {
            MoveCharacterRightAndLeft(0f);
        }

        playerCoinTxt.text = playerCoin.ToString();
    }

    private void analogInput()
    {
        // Camera Rotation
        float rotationX = touchField.TouchDist.x * rotationSpeedX * Time.deltaTime;
        float rotationY = touchField.TouchDist.y * rotationSpeedY * Time.deltaTime;

        camFreeLook.m_XAxis.Value += rotationX;
        camFreeLook.m_YAxis.Value -= rotationY;

        // Joystick Input
        float totalHorizontal = 0f;
        float totalVertical = 0f;
        foreach (FixedJoystick joystick in joysticks)
        {
            totalHorizontal += joystick.Horizontal;
            totalVertical += joystick.Vertical;
        }

        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        Vector3 desiredMoveDirection = (cameraForward.normalized * totalVertical + cameraRight.normalized * totalHorizontal).normalized;

        // Character Movement
        MoveCharacter(desiredMoveDirection);
        ApplyGravity();
        RotateCharacter(desiredMoveDirection);

        // Animation
        if (desiredMoveDirection.magnitude > 0)
        {
            anim.SetBool("running", true);
            anim.SetBool("idle", false);
        }
        else
        {
            anim.SetBool("running", false);
            anim.SetBool("idle", true);
        }
    }

    private void keyboardInput()
    {
        float rotationX = Input.GetAxis("Mouse X") * rotationSpeedX;
        float rotationY = Input.GetAxis("Mouse Y") * rotationSpeedY;

        camFreeLook.m_XAxis.Value += rotationX;
        camFreeLook.m_YAxis.Value -= rotationY;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        Vector3 desiredMoveDirection = (cameraForward.normalized * verticalInput + cameraRight.normalized * horizontalInput).normalized;

        if (horizontalInput != 0 || verticalInput != 0)
        {
            anim.SetTrigger("running");
            // anim.ResetTrigger("idle");
        }
        else
        {
            anim.SetTrigger("idle");
            // anim.ResetTrigger("running");
        }

        MoveCharacter(desiredMoveDirection);
        ApplyGravity();
        RotateCharacter(desiredMoveDirection);
    }

    private void MoveCharacter(Vector3 direction)
    {
        direction.Normalize();
        controller.Move(direction * GameVariable.speed * Time.deltaTime);
    }

    private void RotateCharacter(Vector3 direction)
    {
        direction.Normalize();

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion currentRotation = transform.rotation;
            Quaternion newRotation = Quaternion.Slerp(currentRotation, toRotation, GameVariable.rotationSpeed * Time.deltaTime);
            // Use transform.rotation to rotate the character
            transform.rotation = newRotation;
        }
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            gravityValue += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            gravityValue = 0f;
        }

        controller.Move(new Vector3(0f, gravityValue, 0f) * Time.deltaTime);
    }

    public void MoveLeft()
    {
        isMoveLeft = true;
    }

    public void MoveRight()
    {
        isMoveRIght = true;
    }

    public void StopMoving()
    {
        isMoveLeft = false;
        isMoveRIght = false;
    }

    private void MoveCharacterRightAndLeft(float direction)
    {
        Vector3 cameraRight = playerCamera.right;
        cameraRight.y = 0f;
        Vector3 desiredMoveDirection = (cameraRight.normalized * direction).normalized;

        desiredMoveDirection.Normalize();
        controller.Move(desiredMoveDirection * GameVariable.speed * Time.deltaTime);
    }

    public void SavePlayerData()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if (data != null)
        {
            Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
            Quaternion rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);
            playerCoin = data.playerCoin;

            if (position != null && rotation != null)
            {
                gc.mainCharacter.transform.position = position;
                gc.mainCharacter.transform.rotation = rotation;
            }

            data.questNumber = GameVariable.questNumber;
        }
    }

    public void ShowShopPanel()
    {
        newDictionary["shop"].gameObject.SetActive(true);
        newDictionary["shop"].localPosition = new Vector3(0f, 0f, 0f);
        shop.LoadShopData();
    }

    public void CloseShopPanel()
    {
        newDictionary["shop"].gameObject.SetActive(false);
        if (shop != null && shop.ShopScrollView != null)
        {
            foreach (Transform child in shop.ShopScrollView)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogError("Invalid shop or ShopScrollView reference.");
        }
    }

    public void countCoin(int addCoin) {
        playerCoin += addCoin;
        playerCoinTxt.text = playerCoin.ToString();
    }

    public void showNotification(string message)
    {
        newDictionary["pesan"].gameObject.SetActive(true);
        if (newDictionary["pesan"] != null)
        {
            Animator nPanelAnimator = newDictionary["pesan"].GetComponent<Animator>();
            if (nPanelAnimator != null)
            {
                bool isOpen = nPanelAnimator.GetBool("isOpen");
                nPanelAnimator.SetBool("isOpen", !isOpen);

                StartCoroutine(CloseNotificationAfterDelay(nPanelAnimator, 2f, newDictionary["pesan"]));
            }

            TextMeshProUGUI txtTMP = newDictionary["pesan"].GetChild(0).GetComponent<TextMeshProUGUI>();
            txtTMP.text = message;
        }
    }

    IEnumerator CloseNotificationAfterDelay(Animator animator, float delay, Transform nPanel)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isOpen", false);
        nPanel.gameObject.SetActive(false);
    }
}