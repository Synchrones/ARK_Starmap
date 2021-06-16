using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemsScript : MonoBehaviour
{
    public TextAsset jsonFile;

    public GameObject StarSystemPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        StarSystems jsonStarSystems = JsonUtility.FromJson<StarSystems>(jsonFile.text);
        foreach (StarSystem starSystem in jsonStarSystems.starSystems)
        {
            GameObject StarSystemGO = Instantiate(StarSystemPrefab, new Vector3(starSystem.posX / 3, starSystem.posZ / 3, starSystem.posY / 3), Quaternion.identity);
            StarSystemGO.name = starSystem.name;
            
            //Debug.Log(starsystem.id + starsystem.code + starsystem.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    


}

[System.Serializable]
public class StarSystem
{
    
    public string id;
    public string code;
    public string name;
    public float posX;
    public float posY;
    public float posZ;

}

[System.Serializable]
public class StarSystems
{
    public StarSystem[] starSystems;
}
