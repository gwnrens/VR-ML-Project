using UnityEngine;
using UnityEngine.InputSystem;

public class VrRaceCarScript : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of the car
    public float rotationSpeed = 100f; // Rotation speed of the car

    private Rigidbody rb;
    private VRInputActions inputActions;
    private float driveInput;
    private float steerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new VRInputActions();
        Debug.Log("Awake: Rigidbody and VRInputActions initialized.");
    }

    void OnEnable()
    {
        inputActions.VehicleControls.Enable();
        inputActions.VehicleControls.DriveForward.performed += ctx => DriveForward(ctx);
        inputActions.VehicleControls.DriveBackward.performed += ctx => DriveBackward(ctx);
        inputActions.VehicleControls.Steer.performed += ctx => steerInput = ctx.ReadValue<Vector2>().x;
        inputActions.VehicleControls.Steer.canceled += ctx => steerInput = 0;
        Debug.Log("OnEnable: Input actions enabled.");
    }

    void OnDisable()
    {
        inputActions.VehicleControls.Disable();
        Debug.Log("OnDisable: Input actions disabled.");
    }

    void FixedUpdate()
    {
        MoveCar();
        SteerCar();
        Debug.Log("FixedUpdate: Called MoveCar and SteerCar.");
    }

    private void DriveForward(InputAction.CallbackContext context)
    {
        driveInput = context.ReadValue<float>();
        Debug.Log("DriveForward: Drive input = " + driveInput);
    }

    private void DriveBackward(InputAction.CallbackContext context)
    {
        driveInput = -context.ReadValue<float>();
        Debug.Log("DriveBackward: Drive input = " + driveInput);
    }

    private void MoveCar()
    {
        Vector3 movement = transform.forward * driveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        Debug.Log("MoveCar: Moving with input = " + driveInput + ", speed = " + moveSpeed);
    }

    private void SteerCar()
    {
        float rotation = steerInput * rotationSpeed * Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);
        Debug.Log("SteerCar: Steering with input = " + steerInput + ", rotation = " + rotation);
    }
}