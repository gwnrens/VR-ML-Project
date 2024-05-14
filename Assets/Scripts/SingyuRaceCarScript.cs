using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingyuRaceCarScript : MonoBehaviour
{
    public float speed = 100.0f;        // Snelheid van de auto
    public float rotationSpeed = 100.0f; // Rotatiesnelheid van de auto

    private Rigidbody rb; // Referentie naar de Rigidbody component

    private float moveInput = 0f; // Input voor vooruit/achteruit bewegen
    private float rotateInput = 0f; // Input voor draaien

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Krijg de Rigidbody component van de auto
    }

    void FixedUpdate()
    {
        MoveCar();
    }

    void Update()
    {
        // Verwerkt input van het toetsenbord elke frame
        ProcessInput();
    }

    void MoveCar()
    {
        // Beweeg de auto vooruit of achteruit
        Vector3 movement = transform.right * moveInput * speed * Time.deltaTime; // Pas de juiste as aan
        rb.MovePosition(rb.position + movement);

        // Roteer de auto naar links of rechts
        float rotation = rotateInput * rotationSpeed * Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }

    void ProcessInput()
    {
        // Leest de toetsenbordinvoer voor beweging en rotatie
        float moveVertical = Input.GetAxis("Vertical"); // Vooruit/achteruit
        float moveHorizontal = Input.GetAxis("Horizontal"); // Draaien

        SetInputs(moveVertical, moveHorizontal);
    }

    public void SetInputs(float move, float rotate)
    {
        moveInput = move;
        rotateInput = rotate;
    }
}