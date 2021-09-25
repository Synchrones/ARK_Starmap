using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidthKeeper : MonoBehaviour
{
    LineRenderer lineRenderer;
    Camera mainCamera;
    // Start is called before the first frame update
    List<LineRenderer> lines;
    private Vector3 prevPos;

    void Start()
    {
        mainCamera = Camera.main;
        prevPos = mainCamera.transform.position;
        lines = new List<LineRenderer>();
        
        
        foreach(Transform child in transform)
        {
            lines.Add(child.GetComponent<LineRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(prevPos != mainCamera.transform.position)
        {
            foreach(LineRenderer line in lines)
            {
                float width = Vector3.Distance(mainCamera.transform.position, transform.TransformPoint(line.GetPosition(0))) / 350;
                line.startWidth = width;
                line.endWidth = width;
            }
        }
        prevPos = mainCamera.transform.position;
    }
}
