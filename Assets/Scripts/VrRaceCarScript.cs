using UnityEngine;

public class VRCarController : MonoBehaviour
{
    public float speed = 10.0f;        // Snelheid van de auto
    public float turnSpeed = 50.0f;    // Draaisnelheid van de auto
    public SteeringWheelScript steeringWheel; // Referentie naar het stuurwiel script

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
        // Get trigger values for forward and backward driving
        float driveForward = Input.GetAxis("RightTrigger"); // Right Trigger for moving forward
        float driveBackward = Input.GetAxis("LeftTrigger"); // Left Trigger for moving backward

        // Calculate the current speed based on trigger input
        float currentSpeed = driveForward * speed - driveBackward * speed;

        // Get the joystick input for turning
        float turn = Input.GetAxis("Horizontal"); // Horizontal axis of the joystick

        // Move the car forward or backward
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Only allow the car to turn if the steering wheel is being grabbed
        if (steeringWheel.IsGrabbed())
        {
            float turnAmount = turn * turnSpeed * Time.fixedDeltaTime;
            Quaternion rotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * rotation);
        }
    }
}
