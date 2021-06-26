using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemsScript : MonoBehaviour
{
    public TextAsset jsonFile;

    public GameObject StarSystemPrefab;
    
    public GameObject mainCamera;
    public GameObject selectedSystem;
    // Start is called before the first frame update
    void Start()
    {
        StarSystems jsonStarSystems = JsonUtility.FromJson<StarSystems>(jsonFile.text);
        foreach (StarSystem starSystem in jsonStarSystems.starSystems)
        {
            GameObject StarSystemGO = Instantiate(StarSystemPrefab, new Vector3(starSystem.posX * 7, starSystem.posZ * 7, starSystem.posY * 7), Quaternion.identity);
            StarSystemGO.name = starSystem.name;
            StarSystemGO.transform.localScale *= 20;
            StarSystemGO.AddComponent<SystemsInfosScript>();
            SystemsInfosScript starSystemInfos = StarSystemGO.GetComponent<SystemsInfosScript>();
            starSystemInfos.description = starSystem.description;
            starSystemInfos.type = starSystem.type;
            starSystemInfos.json = System.IO.File.ReadAllText(Application.dataPath + "/Jsons/Systems/" + starSystem.name + ".json");
            if(starSystem.id == 320)starSystemInfos.json = System.IO.File.ReadAllText(Application.dataPath + "/Jsons/Systems/Nul1.json"); //a file can't be named nul so... 
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 500))
            {
                if(hit.transform)
                {
                    selectedSystem = hit.transform.gameObject;
                    mainCamera.GetComponent<CameraScript>().selectSystem(selectedSystem);
                    print(hit.transform.gameObject.GetComponent<SystemsInfosScript>().description);
                    
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            this.GetComponent<StarSystemGeneration>().LoadSystem(selectedSystem);
            mainCamera.GetComponent<CameraScript>().enterSystem(selectedSystem);
        }
    }

    
    


}

[System.Serializable]
public class StarSystem
{
    
    public int id;
    public float posX;
    public float posY;
    public float posZ;
    public string name;
    public string description;
    public string type;
    
}

[System.Serializable]
public class StarSystems
{
    public StarSystem[] starSystems;
}
