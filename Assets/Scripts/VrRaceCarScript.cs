using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VRCarController : MonoBehaviour
{
    public float speed = 10.0f;        // Speed of the car
    public float turnSpeed = 50.0f;    // Turning speed of the car

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Get trigger values
        float driveForward = Input.GetAxis("RightTrigger"); // Right Trigger for moving forward
        float driveBackward = Input.GetAxis("LeftTrigger"); // Left Trigger for moving backward

        // Calculate the current speed based on trigger input
        float currentSpeed = driveForward * speed - driveBackward * speed;

        // Get the joystick input for turning
        float turn = Input.GetAxis("Horizontal"); // Horizontal axis of the joystick (usually the left joystick)

        // Move and turn the car
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        float turnAmount = turn * turnSpeed * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * rotation);
    }
}
