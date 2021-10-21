using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelTraficScript : MonoBehaviour
{
    Material material;
    float offset;
    void Start()
    {
        material = gameObject.GetComponent<LineRenderer>().material;
    }
    void Update()
    {
        offset += 0.0005f;
        material.SetTextureOffset("_Tex1", new Vector2(offset, 0));
        material.SetTextureOffset("_Tex2", new Vector2(-offset, 0));
    }
}
