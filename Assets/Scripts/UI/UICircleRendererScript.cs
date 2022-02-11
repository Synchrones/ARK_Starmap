using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICircleRendererScript : Graphic
{
    public float radius = 100;
    public float width = 10;
    public float degree = 360;
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        
        vh.Clear();

        UIVertex vertexExt = UIVertex.simpleVert;
        UIVertex vertexInt = UIVertex.simpleVert;
        vertexExt.color = color;
        vertexInt.color = color;

        for(int i = 0; i < (degree + 4) / 3; i++)
        {
            if(i % 2 == 0)
            {
                vertexExt.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * ((-i * 3) + 90)) * radius, Mathf.Sin(Mathf.Deg2Rad * ((-i * 3) + 90)) * radius);
                vh.AddVert(vertexExt);

                vertexInt.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * ((-i * 3) + 90)) * (radius - width), Mathf.Sin(Mathf.Deg2Rad * ((-i * 3) + 90)) * (radius - width));
                vh.AddVert(vertexInt);
            }
            if(i > 1)
            {
                vh.AddTriangle(i-2, i-1, i);
            }
        }
    }

    public void drawCircle(float r, float w, float d)
    {
        radius = r;
        width = w;
        degree = d;
        SetVerticesDirty();
    }
}
