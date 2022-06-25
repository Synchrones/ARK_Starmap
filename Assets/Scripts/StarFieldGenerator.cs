using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldGenerator : MonoBehaviour
{
    public void generateStarField(Vector3 pos, int radius, float count, string color1, string color2, Material starFieldMaterial)
    {
        GameObject starFieldContainer = new GameObject();
        for (int i = 0; i < 2; i++)
        {
            string color;
            Color32 starColor;
            if(i == 0) color = color1.Substring(4, 11);
            else color = color2.Substring(4, 11);
            starColor = new Color(int.Parse(color.Split(',')[0]), int.Parse(color.Split(',')[1]), int.Parse(color.Split(',')[2]), 1);

            GameObject starField = new GameObject();
            starField.name = "Star Field";
            starField.transform.position = pos;
            Mesh mesh = starField.AddComponent<MeshFilter>().mesh;
            MeshRenderer meshRenderer = starField.AddComponent<MeshRenderer>();

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            for (int j = 0; j < count / 2; j++)
            {
                vertices.Add(Random.insideUnitSphere * radius);
                indices.Add(j);
            }
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Points, 0);

            meshRenderer.material = starFieldMaterial;
            meshRenderer.material.SetColor("_Color", starColor);
            starField.transform.parent = starFieldContainer.transform;
        }
    }
}