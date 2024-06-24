using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterQuestMovement : MonoBehaviour
{
    public static CharacterQuestMovement Instance;
    CharacterController controller;
    public Transform playerCamera;
    public float gravityValue = -9.8f;
    public bool isMoveLeft = false;
    public bool isMoveRIght = false;
    Animator anim;

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
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();

        if (isMoveLeft)
        {
            MoveCharacterRightAndLeft(-1f);
            if (GameVariable.speed > 0)
            {
                anim.Play("JalanMundur");
            }
        }
        else if (isMoveRIght)
        {
            MoveCharacterRightAndLeft(1f);
            if (GameVariable.speed > 0)
            {
                anim.Play("JalanMaju");
            }
        }
        else
        {
            MoveCharacterRightAndLeft(0f);
            if(GameVariable.speed > 0)
            {
                anim.Play("DoNothing");
            }
        }
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

    private void MoveCharacterRightAndLeft(float direction)
    {
        Vector3 cameraRight = playerCamera.right;
        cameraRight.y = 0f;
        Vector3 desiredMoveDirection = (cameraRight.normalized * direction).normalized;

        desiredMoveDirection.Normalize();
        controller.Move(desiredMoveDirection * GameVariable.speed * Time.deltaTime);
    }
}
