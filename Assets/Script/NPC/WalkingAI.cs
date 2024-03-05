using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAI : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float rotationSpeed = 120f;
    public float stopDistance = 2.5f;

    public Vector3 destination;
    public bool reachedDestination = false;
    private Vector3 lastPosition;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!reachedDestination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                reachedDestination = true;
            }

            velocity = (transform.position - lastPosition) / Time.deltaTime;
            velocity.y = 0;
            var velocityMagnitude = velocity.magnitude;
            velocity = velocity.normalized; // Menggunakan metode Normalize untuk menghasilkan vektor dengan panjang 1.
            var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
            var rightDotProduct = Vector3.Dot(transform.right, velocity);
        }

        lastPosition = transform.position;
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }
}
