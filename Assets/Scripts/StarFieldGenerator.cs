using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldGenerator : MonoBehaviour
{
    public GameObject generateStarField(Vector3 pos, float radius, float count, Color color1, Color color2, Material starFieldMaterial)
    {
        GameObject starFieldContainer = new GameObject();
        for (int i = 0; i < 2; i++)
        {
            Color32 starColor;
            if(i == 0) starColor = color1;
            else starColor = color2;

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
            meshRenderer.material.SetColor("_EmissionColor", new Color(starColor.r * 0.003f, starColor.g * 0.003f, starColor.b * 0.003f));
            starField.transform.parent = starFieldContainer.transform;
        }
        return starFieldContainer;
    }
}