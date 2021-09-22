using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemsScript : MonoBehaviour
{
    public TextAsset systemsJson;
    public TextAsset jumpPointsJson;
    public int cameraMode; // 0 = Galaxy view, 1 = System view
    public bool COSelected;
    public GameObject StarSystemPrefab;
    public GameObject mainCamera;
    public GameObject selectedSystem;
    public GameObject selectedObject;
    public GameObject UIContainer;

    public Sprite UEESprite;
    public Sprite BNUSprite;
    public Sprite VNDSprite;
    public Sprite XINSprite;
    public Sprite DEVSprite;
    public Sprite UNCSprite;


    public int layerMask;

    //tunnels
    private int numPoint = 51;
    private float t;
    private GameObject jumpPointContainer;
    // Start is called before the first frame update
    void Start()
    {
        var systemList = new List<KeyValuePair<GameObject, int>>();

        StarSystems jsonStarSystems = JsonUtility.FromJson<StarSystems>(systemsJson.text);
        foreach (StarSystem starSystem in jsonStarSystems.starSystems)
        {
            GameObject StarSystemGO = Instantiate(StarSystemPrefab, new Vector3(starSystem.posX * 7, starSystem.posZ * 7, starSystem.posY * 7), Quaternion.identity);
            StarSystemGO.name = starSystem.name;
            StarSystemGO.transform.localScale *= 20;
            
            StarSystemGO.AddComponent<SystemsInfosScript>();
            SystemsInfosScript starSystemInfos = StarSystemGO.GetComponent<SystemsInfosScript>();
            starSystemInfos.description = starSystem.description;
            starSystemInfos.type = starSystem.type;
            starSystemInfos.systemName = starSystem.name;
            starSystemInfos.status = starSystem.status;
            starSystemInfos.lastTimeModified = starSystem.time_modified;
            starSystemInfos.size = starSystem.aggregated_size;
            starSystemInfos.population = starSystem.aggregated_population;
            starSystemInfos.economy = starSystem.aggregated_economy;
            starSystemInfos.danger = starSystem.aggregated_danger;


            foreach(Affiliation affiliation in starSystem.affiliation)
            {
                starSystemInfos.affiliationID = affiliation.id;
                starSystemInfos.affiliationName = affiliation.name;

                switch(affiliation.id)
                {
                    case "1":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = UEESprite;

                        break;

                    case "2":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = BNUSprite;

                        break;

                    case "3":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = VNDSprite;

                        break;

                    case "4":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = XINSprite;

                        break;

                    case "7":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = DEVSprite;

                        break;

                    case "8":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = UNCSprite;

                        break;
                }
            }
            StarSystemGO.AddComponent<SystemApparenceKeeper>();

            starSystemInfos.json = System.IO.File.ReadAllText(Application.dataPath + "/Jsons/Systems/" + starSystem.name + ".json");
            if(starSystem.id == 320)starSystemInfos.json = System.IO.File.ReadAllText(Application.dataPath + "/Jsons/Systems/Nul1.json"); //a file can't be named nul so... 

            systemList.Add(new KeyValuePair<GameObject, int>(StarSystemGO, starSystem.id));

        }
        
        jumpPointContainer = new GameObject("Jump-Points");
        Tunnels jsonTunnels = JsonUtility.FromJson<Tunnels>(jumpPointsJson.text);
        foreach(Tunnel tunnel in jsonTunnels.tunnels)
        {
            int entryID = int.Parse(tunnel.entry.star_system_id);
            int exitID = int.Parse(tunnel.exit.star_system_id); 
        
            Vector3[] positions = new Vector3[numPoint];

            Vector3 entrySystemPos = new Vector3();
            Vector3 exitSystemPos = new Vector3();
            foreach(KeyValuePair<GameObject, int> system in systemList)
            {
                if(system.Value == entryID)
                {
                    entrySystemPos = system.Key.gameObject.transform.position;
                }
                if(system.Value == exitID)
                {
                    exitSystemPos = system.Key.gameObject.transform.position;
                }
            }
            GameObject tunnelGO = new GameObject(tunnel.entry.designation);
            tunnelGO.transform.parent = jumpPointContainer.transform;
            LineRenderer line = tunnelGO.AddComponent<LineRenderer>();
            line.positionCount = numPoint;
            Vector3 v1 = entrySystemPos + (exitSystemPos - entrySystemPos) * 1/3 + new Vector3(7 + entryID / 1000, 7 + exitID / 1000, 7 + entryID / 1000);
            Vector3 v2 = entrySystemPos + (exitSystemPos - entrySystemPos) * 2/3 - new Vector3(7 + entryID / 1000, 7 + exitID / 1000, 7 + entryID / 1000);
            for(int i = 0; i < numPoint - 1; i++)
            {
                positions[i] = CubicCurve(entrySystemPos, v1, v2, exitSystemPos, t);
                t += 0.02f;
                
            }
            positions[50] = CubicCurve(entrySystemPos, v1, v2, exitSystemPos, t);
            line.SetPositions(positions);
            t = 0;

            SizeKeeper sizeKeeper = tunnelGO.AddComponent<SizeKeeper>();
            sizeKeeper.startPoint = entrySystemPos;
            sizeKeeper.endPoint = exitSystemPos;
            sizeKeeper.lineRenderer = line;
        }

        cameraMode = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(cameraMode == 0)
        {
            if(Input.GetMouseButtonUp(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, 2000))
                {
                    if(hit.transform)
                    {
                        SelectSystem(hit.transform.gameObject);
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.Return))
            {
                LoadAndEnterSystem();
                
            }
        }
        else if(cameraMode == 1)
        {
            if(Input.GetMouseButtonUp(1))
            {
                layerMask = 1 << LayerMask.NameToLayer("CO");
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, 100, layerMask))
                {
                    if(hit.transform)
                    {
                        SelectCO(hit.transform.gameObject);
                        COSelected = true;
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(COSelected == true)
                {
                    COSelected = false;
                    mainCamera.GetComponent<CameraScript>().COSelected = false;
                }
                else UnloadAndExitSystem();
                
            }
        }
        
        //temporary
        if(Input.GetKeyDown(KeyCode.J))
        {
            if(jumpPointContainer.gameObject.activeInHierarchy == true)
            {
                jumpPointContainer.SetActive(false);
            }
            else
            {
                jumpPointContainer.SetActive(true);
            }
        }
    }

    public void SelectSystem(GameObject gameObject)
    {
        selectedSystem = gameObject;
        mainCamera.GetComponent<CameraScript>().SelectSystem(selectedSystem);
        DiscScript discScript = UIContainer.GetComponent<DiscScript>();
        discScript.selectedObject = selectedSystem;
        discScript.mode = 0;
        discScript.LoadDisc();

        print(gameObject.GetComponent<SystemsInfosScript>().description);
    }

    public void SelectCO(GameObject gameObject)
    {
        selectedObject = gameObject;
        mainCamera.GetComponent<CameraScript>().MoveToCO(selectedObject);
        DiscScript discScript = UIContainer.GetComponent<DiscScript>();
        discScript.selectedObject = selectedObject;
        discScript.mode = 1;
        discScript.LoadDisc();
    }

    public void LoadAndEnterSystem()
    {
        UIContainer.GetComponent<DiscScript>().UnloadDisc();
        this.GetComponent<StarSystemGeneration>().LoadSystem(selectedSystem);
        mainCamera.GetComponent<CameraScript>().EnterSystem(selectedSystem);
        UIContainer.GetComponent<SystemNameScript>().ChangeName(selectedSystem.name);
        cameraMode = 1;
    }

    public void UnloadAndExitSystem()
    {
        this.GetComponent<StarSystemGeneration>().UnloadSystem(selectedSystem);
        cameraMode = 0;
        mainCamera.GetComponent<CameraScript>().ExitSystem(selectedSystem);
        UIContainer.GetComponent<SystemNameScript>().ChangeName("");
    }

    private Vector3 QuadradicCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    private Vector3 CubicCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = QuadradicCurve(a, b, c, t);
        Vector3 p1 = QuadradicCurve(b, c, d, t);
        return Vector3.Lerp(p0, p1, t);
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
    public string status;
    public string time_modified;
    public string aggregated_size;
    public string aggregated_population;
    public string aggregated_economy;
    public string aggregated_danger;
    public Affiliation[] affiliation;

    
}

[System.Serializable]
public class StarSystems
{
    public StarSystem[] starSystems;
}

[System.Serializable]
public class Affiliation
{
    public string id;
    public string name;
}


[System.Serializable]
public class Tunnels
{
    public Tunnel[] tunnels;
}

[System.Serializable]
public class Tunnel
{
    public string entry_id;
    public string exit_id;
    public string size;

    public Entry entry;
    public Exit exit;

}

[System.Serializable]
public class Entry
{
    public string star_system_id;
    public string designation;
}

[System.Serializable]
public class Exit
{
    public string star_system_id;
}

