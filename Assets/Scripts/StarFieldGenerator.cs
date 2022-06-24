using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldGenerator : MonoBehaviour
{
    public void generateStarField(Vector3 pos, int radius, float count, float sizeMin, float sizeMax, GameObject starPrefab, string color1, string color2)
    {
        color1 = color1.Substring(4, 11);
        color2 = color2.Substring(4, 11);
        Color starColor1 = new Color(int.Parse(color1.Split(',')[0]), int.Parse(color1.Split(',')[1]), int.Parse(color1.Split(',')[2]), 1);
        Color starColor2 = new Color(int.Parse(color2.Split(',')[0]), int.Parse(color2.Split(',')[1]), int.Parse(color2.Split(',')[2]), 1);

        for (int i = 0; i < count; i++)
        {
            GameObject star = Instantiate(starPrefab, Random.insideUnitSphere * radius, Quaternion.identity);

            if(Random.Range(0, 2) > 1)
            {
                star.GetComponent<SpriteRenderer>().color = starColor1;
            }
            else
            {
                star.GetComponent<SpriteRenderer>().color = starColor2;
            }
            star.transform.localScale *= Random.Range(sizeMin, sizeMax);
            star.AddComponent<StarDotScript>();
        }
    }
}