using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidthKeeper : MonoBehaviour
{
    LineRenderer lineRenderer;
    Camera mainCamera;
    // Start is called before the first frame update
    List<LineRenderer> lines;

    void Start()
    {
        mainCamera = Camera.main;
        lines = new List<LineRenderer>();
        
        foreach(Transform child in transform)
        {
            lines.Add(child.GetComponent<LineRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(LineRenderer line in lines)
        {
            float width = Vector3.Distance(mainCamera.transform.position, line.GetPosition(0)) / 300;
            line.startWidth = width;
            line.endWidth = width;
        }
        
    }
}
