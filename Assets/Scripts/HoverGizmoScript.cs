using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverGizmoScript : MonoBehaviour
{
    // Start is called before the first frame update
    Transform hoverGizmo;
    Transform horizontalLine;
    Transform verticalLine;
    Vector3 prevPos;
    void Start()
    {
        horizontalLine = gameObject.transform.GetChild(0).transform;
        verticalLine = gameObject.transform.GetChild(1).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Camera.main.transform.position != prevPos)
        {
            horizontalLine.transform.localScale = new Vector3(1, Mathf.Clamp(Vector3.Distance(transform.position, Camera.main.transform.position) * 50, 20, 300), 1);
            verticalLine.transform.localScale = new Vector3(1, 1, Mathf.Clamp(Vector3.Distance(transform.position, Camera.main.transform.position) * 50, 20, 300));

            prevPos = Camera.main.transform.position;
        }
    }
}
