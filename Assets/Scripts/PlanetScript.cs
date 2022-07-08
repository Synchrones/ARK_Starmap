using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public List<GameObject> toRotate = new List<GameObject>();
    void Update()
    {
        foreach(GameObject gameObject in toRotate)
        {
            gameObject.transform.Rotate(Vector3.up, -0.01f);
        }
    }
}
