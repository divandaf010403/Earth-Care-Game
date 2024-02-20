using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCharMovement : MonoBehaviour
{
    [Header("Character Utils")]
    public Rigidbody rb;
    public float speed, rotationSpeed;
    public Transform playerCamera;
    public CinemachineFreeLook camFreeLook;
    public float rotationSpeedX = 2f;
    public float rotationSpeedY = 0.5f;

    [Header("Interaction Utils")]
    public float interactRadius = 2f;
    public TextMeshProUGUI cube, sphere, totalSampah;
    public int cubeVal = 0, sphereVal = 0, totalVal = 0;
    public GameObject[] TrashBagObj;
    public TextMeshProUGUI notificationPanel;
    public bool isCanInteractTrash = true;

    [Header("Joystick Utils")]
    public bool enableMobileInput = false;
    public FixedJoystick joystick;
    public FixedTouchField touchField;

    [Header("Screen Service")]
    GameController gc;
    public GameObject loadingPanel;
    public GameObject shopPanel;
    public GameObject mulaiMisiBtn;
    public GameObject endMisiBtn;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameController = GameObject.Find("GameController");

        rb = GetComponent<Rigidbody>();
        gc = gameController.GetComponent<GameController>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        notificationPanel.gameObject.SetActive(false);
        loadingPanel.SetActive(false);
        shopPanel.SetActive(false);
        mulaiMisiBtn.SetActive(false);
        endMisiBtn.SetActive(false);
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (enableMobileInput)
        {
            analogInput();
        }
        else
        {
            keyboardInput();
        }
    }

    private void analogInput()
    {
        float rotationX = touchField.TouchDist.x * rotationSpeedX * Time.deltaTime;
        float rotationY = touchField.TouchDist.y * rotationSpeedY * Time.deltaTime;

        camFreeLook.m_XAxis.Value += rotationX;
        camFreeLook.m_YAxis.Value -= rotationY;

        // IMPLEMENTASI JOYSTICK
        float x = joystick.Horizontal;
        float z = joystick.Vertical;

        // Calculate movement direction relative to the camera
        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;

        // Flatten the vectors so the character doesn't move up and down
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        Vector3 desiredMoveDirection = (cameraForward.normalized * z + cameraRight.normalized * x).normalized;

        // Move and rotate the character
        MoveCharacter(desiredMoveDirection);
        RotateCharacter(desiredMoveDirection);
    }

    private void keyboardInput()
    {
        float rotationX = Input.GetAxis("Mouse X") * rotationSpeedX;
        float rotationY = Input.GetAxis("Mouse Y") * rotationSpeedY;

        camFreeLook.m_XAxis.Value += rotationX;
        camFreeLook.m_YAxis.Value -= rotationY;

        // Get input from arrow keys or WASD
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction relative to the camera
        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;

        // Flatten the vectors so the character doesn't move up and down
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        Vector3 desiredMoveDirection = (cameraForward.normalized * verticalInput + cameraRight.normalized * horizontalInput).normalized;

        // Move and rotate the character
        MoveCharacter(desiredMoveDirection);
        RotateCharacter(desiredMoveDirection);
    }

    private void MoveCharacter(Vector3 direction)
    {
        direction.Normalize();
        Vector3 targetPosition = transform.position + direction * speed * Time.deltaTime;
        rb.MovePosition(targetPosition);
    }

    private void RotateCharacter(Vector3 direction)
    {
        direction.Normalize();

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            Quaternion currentRotation = rb.rotation;
            Quaternion newRotation = Quaternion.Slerp(currentRotation, toRotation, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(newRotation);
        }
    }

    public void MoveLeft()
    {
        MoveCharacterRightAndLeft(-1f);
    }

    public void MoveRight()
    {
        MoveCharacterRightAndLeft(1f);
    }

    public void StopMoving()
    {
        MoveCharacterRightAndLeft(0f);
    }

    private void MoveCharacterRightAndLeft(float direction)
    {
        Vector3 cameraRight = playerCamera.right;
        cameraRight.y = 0f;
        Vector3 desiredMoveDirection = (cameraRight.normalized * direction).normalized;

        desiredMoveDirection.Normalize();
        Vector3 velocity = desiredMoveDirection * speed;

        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    public void LoadPlayer()
    {
        StartCoroutine(LoadPlayerCoroutine());
    }

    private IEnumerator LoadPlayerCoroutine()
    {
        // Tampilkan layar loading di sini
        loadingPanel.SetActive(true);
        loadingPanel.transform.localPosition = new Vector3(0f, 0f, 0f);

        yield return null; // Delay untuk memberikan kesempatan layar loading untuk tampil

        PlayerData data = SaveSystem.LoadPlayer();

        if (data != null)
        {
            Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
            Quaternion rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);

            if (position != null && rotation != null)
            {
                gc.mainCharacter.transform.position = position;
                gc.mainCharacter.transform.rotation = rotation;
            }

            Debug.Log("Posisi" + position);
            Debug.Log("Rotasi" + rotation);
        }

        // Sembunyikan layar loading di sini
        loadingPanel.SetActive(false);
    }

    public void ShowShopPanel()
    {
        shopPanel.SetActive(true);
        shopPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
    }
}