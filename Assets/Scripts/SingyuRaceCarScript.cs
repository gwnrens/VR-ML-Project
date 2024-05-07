using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingyuRaceCarScript : MonoBehaviour
{
    public float speed = 100.0f;        // Snelheid van de auto
    public float rotationSpeed = 100.0f; // Rotatiesnelheid van de auto

    private Rigidbody rb; // Referentie naar de Rigidbody component

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Krijg de Rigidbody component van de auto
    }

    void FixedUpdate()
    {
        MoveCar();
    }

    void MoveCar()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // Leest horizontale input

        float moveVertical = Input.GetAxis("Vertical"); // Leest verticale input

        // Gebruik transform.right in plaats van transform.forward als forward niet correct is
        Vector3 movement = transform.right * moveVertical * speed * Time.deltaTime; // Past de juiste as aan
        rb.MovePosition(rb.position + movement);

        // Roteer op basis van horizontale input
        float rotation = moveHorizontal * rotationSpeed * Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }

}

