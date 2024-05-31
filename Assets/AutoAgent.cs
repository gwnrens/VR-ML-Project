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

    public Transform checkpointPrefab; // Voeg een prefab toe via de Inspector
    public Transform[] checkpoints; // Dynamisch toegewezen checkpoints
    private int nextCheckpoint = 0;

    public LayerMask terrainLayer; // LayerMask voor het terrein
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float timeSinceLastCheckpoint = 0f; // Timer voor het bijhouden van tijd sinds het laatste checkpoint
    private float maxTimeForCheckpoint = 10f; // Maximale tijd om een checkpoint te bereiken

    void Start()
    {
        carScript = GetComponent<SingyuRaceCarScript>();
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Instantiate the checkpoints from the prefab
        InstantiateCheckpoints();
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
        timeSinceLastCheckpoint = 0f; // Reset de timer
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observeer de richting naar het volgende checkpoint
        if (checkpoints.Length > 0)
        {
            Vector3 directionToCheckpoint = (checkpoints[nextCheckpoint].position - transform.position).normalized;
            sensor.AddObservation(directionToCheckpoint);

            // Voeg de afstand tot het checkpoint toe
            sensor.AddObservation(Vector3.Distance(transform.position, checkpoints[nextCheckpoint].position));

            // Voeg de relatieve rotatie naar het checkpoint toe
            Vector3 relativePos = checkpoints[nextCheckpoint].position - transform.position;
            float angleToCheckpoint = Vector3.SignedAngle(transform.forward, relativePos, Vector3.up);
            sensor.AddObservation(angleToCheckpoint / 180.0f); // Normaliseer tussen -1 en 1
        }

        // Voeg snelheidsobservaties toe
        sensor.AddObservation(rb.velocity.magnitude);
        sensor.AddObservation(rb.angularVelocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Verkrijg de acties vanuit de ML-agent
        float move = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        float rotate = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        // Debugging: Print de actie inputs om te zien wat er wordt doorgegeven
        Debug.Log($"Move: {move}, Rotate: {rotate}");

        // Gebruik dezelfde schaal en richting als in SingyuRaceCarScript
        carScript.SetInputs(move, rotate);

        // Beloon de agent voor het naderen van het checkpoint
        AddRewardForCheckpointProximity();

        // Beloon de agent voor het blijven op het terrein
        CheckIfOnTerrain();

        // Verhoog de tijd sinds het laatste checkpoint
        timeSinceLastCheckpoint += Time.deltaTime;

        // Straf de agent als deze te lang doet over een checkpoint
        if (timeSinceLastCheckpoint > maxTimeForCheckpoint)
        {
            AddReward(-1.0f); // Straf voor te lang doen over een checkpoint
            EndEpisode();
        }
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
                timeSinceLastCheckpoint = 0f; // Reset de timer wanneer een checkpoint wordt bereikt
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
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    private void InstantiateCheckpoints()
    {
        // Voorbeeld van het instantiëren van checkpoints vanuit een prefab
        // Dit moet aangepast worden naar jouw specifieke setup
        int checkpointCount = 3; // Aantal checkpoints instellen
        checkpoints = new Transform[checkpointCount];
        for (int i = 0; i < checkpointCount; i++)
        {
            // Instantiate the prefab at the initial position
            checkpoints[i] = Instantiate(checkpointPrefab, new Vector3(i * 10, 0, 0), Quaternion.identity);
        }
    }

    private void ResetCheckpoints()
    {
        // Reset de posities van de checkpoints als dat nodig is
        // Je kunt dit aanpassen om de checkpoints dynamisch te verplaatsen
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].position = new Vector3(i * 10, 0, 0); // Voorbeeldpositie
        }
    }

    private void AddRewardForCheckpointProximity()
    {
        if (checkpoints.Length > 0)
        {
            float distanceToCheckpoint = Vector3.Distance(transform.position, checkpoints[nextCheckpoint].position);
            float reward = Mathf.Max(0, 1 - (distanceToCheckpoint / 10.0f)); // Pas de schaal van de beloning aan indien nodig
            AddReward(reward * 0.1f);
        }
    }
}
