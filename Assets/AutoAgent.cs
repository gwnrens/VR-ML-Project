using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AutoAgent : Agent
{
    private SingyuRaceCarScript carScript;
    private Rigidbody rb;

    public Transform[] checkpoints; // Voeg checkpoints toe via de Inspector
    private int nextCheckpoint = 0;

    public LayerMask terrainLayer; // LayerMask voor het terrein
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        carScript = GetComponent<SingyuRaceCarScript>();
        rb = GetComponent<Rigidbody>();
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

        // Reset checkpoints
        nextCheckpoint = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observeer de richting naar het volgende checkpoint
        if (checkpoints.Length > 0)
        {
            Vector3 directionToCheckpoint = (checkpoints[nextCheckpoint].position - transform.position).normalized;
            sensor.AddObservation(directionToCheckpoint);
        }

        // Voeg snelheidsobservaties toe
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(rb.angularVelocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float move = actionBuffers.ContinuousActions[0];
        float rotate = actionBuffers.ContinuousActions[1];

        // Gebruik de waarden om de auto te besturen
        carScript.SetInputs(move, rotate);

        // Beloon de agent voor het blijven op het terrein
        CheckIfOnTerrain();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            if (other.transform == checkpoints[nextCheckpoint])
            {
                nextCheckpoint = (nextCheckpoint + 1) % checkpoints.Length;
                AddReward(1.0f);
            }
        }
    }

    void CheckIfOnTerrain()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, terrainLayer))
        {
            // De auto is op het terrein
            AddReward(0.1f);
        }
        else
        {
            // De auto is niet op het terrein
            AddReward(-0.1f);
            EndEpisode();
        }
    }
}
