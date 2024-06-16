using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AutoAgent : Agent
{
    private Rigidbody rb;

    public float speed = 100.0f;        // Speed of the car
    public float rotationSpeed = 100.0f; // Rotation speed of the car

    public Transform checkpointPrefab; // Add a prefab via the Inspector
    public Transform[] checkpoints; // Dynamically assigned checkpoints
    private int nextCheckpoint = 0;

    public LayerMask terrainLayer; // LayerMask for the terrain
    public LayerMask wallLayer; // LayerMask for the walls
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float timeSinceLastCheckpoint = 0f; // Timer for tracking time since the last checkpoint
    private float maxTimeForCheckpoint = 10f; // Maximum time to reach a checkpoint

    private float moveInput = 0f; // Input for forward/backward movement
    private float rotateInput = 0f; // Input for turning

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the GameObject.");
            return;
        }
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Check if checkpoints are assigned in the Inspector
        if (checkpoints.Length == 0)
        {
            Debug.LogError("Checkpoints not assigned in the Inspector.");
        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset the position and rotation of the car
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset checkpoints
        nextCheckpoint = 0;
        timeSinceLastCheckpoint = 0f; // Reset the timer
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the direction to the next checkpoint
        if (checkpoints.Length > 0)
        {
            Vector3 directionToCheckpoint = (checkpoints[nextCheckpoint].position - transform.position).normalized;
            sensor.AddObservation(directionToCheckpoint);

            // Add the distance to the checkpoint
            sensor.AddObservation(Vector3.Distance(transform.position, checkpoints[nextCheckpoint].position));

            // Add the relative rotation to the checkpoint
            Vector3 relativePos = checkpoints[nextCheckpoint].position - transform.position;
            float angleToCheckpoint = Vector3.SignedAngle(transform.forward, relativePos, Vector3.up);
            sensor.AddObservation(angleToCheckpoint / 180.0f); // Normalize between -1 and 1
        }

        // Add speed observations
        sensor.AddObservation(rb.velocity.magnitude);
        sensor.AddObservation(rb.angularVelocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Obtain actions from the ML-agent
        moveInput = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        rotateInput = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        // Debug logs for actions received
        Debug.Log($"Move Input: {moveInput}, Rotate Input: {rotateInput}");

        // Move the car
        MoveCar();

        // Check if the car is on the terrain
        CheckIfOnTerrain();

        // Increase the time since the last checkpoint
        timeSinceLastCheckpoint += Time.deltaTime;

        // Penalize the agent if it takes too long to reach a checkpoint
        if (timeSinceLastCheckpoint > maxTimeForCheckpoint)
        {
            AddReward(-1.0f); // Penalize for taking too long to reach a checkpoint
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical"); // Forward/Backward
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); // Turning
    }

    void FixedUpdate()
    {
        MoveCar();
    }

    void MoveCar()
    {
        // Move the car forward or backward
        Vector3 movement = transform.forward * moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate the car left or right
        float rotation = rotateInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);

        // Debug logs for movement
        Debug.Log($"Car Position: {rb.position}, Car Rotation: {rb.rotation}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            if (other.transform == checkpoints[nextCheckpoint])
            {
                // Correct checkpoint reached
                nextCheckpoint = (nextCheckpoint + 1) % checkpoints.Length;
                AddReward(1.0f);
                timeSinceLastCheckpoint = 0f; // Reset the timer when a checkpoint is reached

                // Debug log for checkpoint
                Debug.Log("Checkpoint reached, next checkpoint index: " + nextCheckpoint);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            AddReward(-0.5f);
            Debug.Log("Hit a wall. Penalty applied.");
        }
    }

    void CheckIfOnTerrain()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, 1f, terrainLayer))
        {
            // The car is not on the terrain
            AddReward(-1.0f);
            EndEpisode();
        }
    }
}
