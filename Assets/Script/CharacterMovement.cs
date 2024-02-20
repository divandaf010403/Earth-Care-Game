using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    public float speed = 5f;
    public float rotationSpeed = 10f;
    public Transform playerCamera;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
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
        // Move the character using Rigidbody
        Vector3 movement = direction * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    private void RotateCharacter(Vector3 direction)
    {
        // Rotate the character to face the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime));
        }
    }
}
