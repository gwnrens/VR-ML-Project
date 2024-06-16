using System.Collections.Generic;
using Unity.MLAgents.Demonstrations;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    public string demonstrationName = "drive";
    private DemonstrationRecorder demonstrationRecorder;

    void Start()
    {
        demonstrationRecorder = gameObject.AddComponent<DemonstrationRecorder>();
        demonstrationRecorder.DemonstrationName = demonstrationName;
        demonstrationRecorder.Record = true;
        Debug.Log("Recording started.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            demonstrationRecorder.Record = !demonstrationRecorder.Record;
            Debug.Log(demonstrationRecorder.Record ? "Recording resumed." : "Recording paused.");
        }
    }

    void OnDestroy()
    {
        if (demonstrationRecorder != null)
        {
            demonstrationRecorder.Record = false;
            Debug.Log("Recording stopped.");
        }
    }
}
