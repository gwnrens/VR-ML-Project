using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RaceCarAgent : Agent
{
    private Rigidbody rb;

    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public int wallLayerIndex = 8;
    public float fallThreshold = -10.0f;
    public float maxHeightThreshold = 20.0f; // Maximum height before resetting

    private Vector3 initialPosition;
    private Quaternion initialRotation;

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
    }

    public override void OnEpisodeBegin()
    {
        ResetCar();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(rb.angularVelocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float moveInput = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        float rotateInput = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        MoveCar(moveInput, rotateInput);

        // Reward for moving forward
        AddReward(0.01f * moveInput);

        // Penalty for falling off the map
        if (transform.position.y < fallThreshold)
        {
            AddReward(-1.0f);
            EndEpisode();
            ResetCar();
        }

        // Penalty for going too high
        if (transform.position.y > maxHeightThreshold)
        {
            AddReward(-1.0f);
            EndEpisode();
            ResetCar();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    void MoveCar(float moveInput, float rotateInput)
    {
        Vector3 movement = transform.forward * moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        float rotation = rotateInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == wallLayerIndex)
        {
            AddReward(-0.5f); // Penalty for hitting a wall
            EndEpisode();
            ResetCar();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Check"))
        {
            AddReward(1.0f); // Reward for reaching the checkpoint
            EndEpisode();
            ResetCar();
        }
    }

    void ResetCar()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
