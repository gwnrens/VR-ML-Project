using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class RaceCarAgent : Agent
{
    private Rigidbody rb;

    public float speed = 10.0f;        // Snelheid van de auto
    public float rotationSpeed = 100.0f; // Rotsnelheid van de auto

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody niet gevonden op de GameObject.");
            return;
        }
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        // Reset de positie en rotatie van de auto
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Voeg observaties toe
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(rb.angularVelocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Verkrijg acties van de ML-agent
        float moveInput = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        float rotateInput = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        // Beweeg de auto
        MoveCar(moveInput, rotateInput);

        // Geef een beloning voor vooruitgang
        float distanceToTarget = Vector3.Distance(transform.localPosition, Vector3.zero); // Aangepast om naar een doel te wijzen, indien gewenst
        AddReward(-distanceToTarget / 1000f); // Negatieve beloning op basis van afstand om de auto vooruit te bewegen
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");   // Versnelling vooruit en achteruit
        continuousActionsOut[1] = Input.GetAxis("Horizontal"); // Sturen links en rechts
    }

    void MoveCar(float moveInput, float rotateInput)
    {
        // Beweeg de auto voorwaarts of achterwaarts
        Vector3 movement = transform.forward * moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Draai de auto naar links of rechts
        float rotation = rotateInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}