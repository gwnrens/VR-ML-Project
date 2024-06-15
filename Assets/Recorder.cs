using System.IO;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    public string filePath = "Assets/DemoData/demo.csv";
    private StreamWriter writer;
    private bool isRecording = false;

    void Start()
    {
        // Start de opname bij het starten van de game
        StartRecording();
    }

    void Update()
    {
        // Start of stop de opname met de "R" toets
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isRecording)
            {
                StopRecording();
            }
            else
            {
                StartRecording();
            }
        }

        // Als er wordt opgenomen, schrijf de data naar het bestand
        if (isRecording)
        {
            float steering = Input.GetAxis("Horizontal");
            float acceleration = Input.GetAxis("Vertical");
            float brake = Input.GetKey(KeyCode.Space) ? 1 : 0;
            writer.WriteLine($"{steering},{acceleration},{brake}");
        }
    }

    void StartRecording()
    {
        // Als het bestand al bestaat, voeg data toe, anders maak een nieuw bestand
        writer = new StreamWriter(filePath, true);
        writer.WriteLine("steering,acceleration,brake");
        isRecording = true;
        Debug.Log("Recording started.");
    }

    void StopRecording()
    {
        if (writer != null)
        {
            writer.Close();
            writer = null;
            isRecording = false;
            Debug.Log("Recording stopped.");
        }
    }

    void OnDestroy()
    {
        StopRecording();
    }
}