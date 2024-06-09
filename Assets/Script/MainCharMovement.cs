using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCharMovement : MonoBehaviour
{
    public static MainCharMovement Instance;

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
    public FixedJoystick joystick;
    public FixedTouchField touchField;
    public Animator anim;

    [Header("Coba Dictionary")]
    [SerializeField] public CustomDictionary customDictionary;
    [SerializeField] public Dictionary<string, Transform> newDictionary;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        newDictionary = customDictionary.ToDictionary();

        foreach (var kvp in newDictionary)
        {
            kvp.Value.gameObject.SetActive(false);
        }
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
        
        totalHorizontal += joystick.Horizontal;
        totalVertical += joystick.Vertical;

        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        Vector3 desiredMoveDirection = (cameraForward.normalized * totalVertical + cameraRight.normalized * totalHorizontal).normalized;

        // Character Movement
        MoveCharacter(desiredMoveDirection);
        ApplyGravityAndStayOnGround();
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
        ApplyGravityAndStayOnGround();
        RotateCharacter(desiredMoveDirection);
    }

    private void MoveCharacter(Vector3 direction)
    {
        direction.Normalize();
        Vector3 nextPosition = transform.position + direction * GameVariable.speed * Time.deltaTime;

        if (IsOnGround(nextPosition))
        {
            controller.Move(direction * GameVariable.speed * Time.deltaTime);
        }
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

    private void ApplyGravityAndStayOnGround()
    {
        if (!IsOnGround(transform.position))
        {
            Vector3 gravityMove = new Vector3(0f, gravityValue * Time.deltaTime, 0f);
            controller.Move(gravityMove);
        }

        AdjustToGround();
    }

    private void AdjustToGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            controller.Move(targetPosition - transform.position);
        }
    }

    private bool IsOnGround(Vector3 position)
    {
        Ray ray = new Ray(position, Vector3.down);
        return Physics.Raycast(ray, 1f, groundLayer);
    }

    public void countCoin(int addCoin) {
        playerCoin += addCoin;
        playerCoinTxt.text = playerCoin.ToString();

        SaveSystem.UpdatePlayerCoin(this);
    }

    public void showNotification(string message)
    {
        newDictionary["pesan"].gameObject.SetActive(true);
        if (newDictionary["pesan"] != null)
        {
            Animator nPanelAnimator = newDictionary["pesan"].GetComponent<Animator>();
            if (nPanelAnimator != null)
            {
                StartCoroutine(CloseNotificationAfterDelay(nPanelAnimator, 2f, newDictionary["pesan"]));
            }

            TextMeshProUGUI txtTMP = newDictionary["pesan"].GetChild(0).GetComponent<TextMeshProUGUI>();
            txtTMP.text = message;
        }
    }

    IEnumerator CloseNotificationAfterDelay(Animator animator, float delay, Transform nPanel)
    {
        animator.SetBool("isOpen", true);
        yield return new WaitForSeconds(delay);
        animator.SetBool("isOpen", false);

        // Tunggu sampai animasi selesai
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // Menonaktifkan nPanel setelah animasi selesai
        nPanel.gameObject.SetActive(false);
    }
}