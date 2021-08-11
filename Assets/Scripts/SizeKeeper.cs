using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeKeeper : MonoBehaviour
{

    Camera mainCamera;
    Vector3 cameraPos;
    public Vector3 startPoint;
    public Vector3 endPoint;

    public LineRenderer lineRenderer;
    float startDistance;
    float endDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        cameraPos = mainCamera.transform.position;
        startDistance = Vector3.Distance(cameraPos, startPoint);
        endDistance = Vector3.Distance(cameraPos, endPoint) / 150;
        lineRenderer.startWidth = startDistance / 150;
        lineRenderer.endWidth = endDistance;
    }
}