using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICircleRendererScript : Graphic
{
    public float radius = 100;
    public float width = 10;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        UIVertex vertexExt = UIVertex.simpleVert;
        UIVertex vertexInt = UIVertex.simpleVert;
        vertexExt.color = color;
        vertexInt.color = color;

        for(int i = 0; i < 240; i++)
        {
            if(i % 2 == 0)
            {
                vertexExt.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (i * 3)) * radius, Mathf.Sin(Mathf.Deg2Rad * (i * 3)) * radius);
                vh.AddVert(vertexExt);

                vertexInt.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (i * 3)) * (radius - width), Mathf.Sin(Mathf.Deg2Rad * (i * 3)) * (radius - width));
                vh.AddVert(vertexInt);
            }
            if(i > 1)
            {
                vh.AddTriangle(i-2, i-1, i);
            }
        }
    }
}
