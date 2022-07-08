using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// TODO: (main TODO) : add starting screen, fix space stations generation, left click on system to enter, quick zoom with middle mouse button
/* TODO: (bug fixes) : 
    -planet hitbox overlap when to close (see Kilian) 
    -reduce performance impact of the starbox? 
    -oberon star is... small
    -unknow Co type "POI" (vega Vanduul attack)
    -landing zones don't rotate with the planets
    -systems looks bigger when on the side of the screen
*/
/* TODO: (graphics) :
    -rework star shader (colors, animations, "corona"...)
    -fix hover gizmo (see Castra)
    -fix planetary rings (too small)
    -fix asteroid rings 
*/
public class StarSystemsScript : MonoBehaviour
{
    public List<JumpPoint> jumpPointList;
    string systemsJson;
    string jumpPointsJson;
    public int cameraMode; // 0 = Galaxy view, 1 = System view
    public bool COSelected;
    public GameObject StarSystemPrefab;
    public GameObject mainCamera;
    public GameObject selectedSystem;
    public GameObject selectedObject;
    public GameObject UIContainer;
    public GameObject systemName;
    public GameObject spaceBoxColor;
    public GameObject starPrefab;
    AudioManagerScript AudioManager;
    bool hasHitSoundPlayed;
    public GameObject HoverGizmo;
    bool isHoverGizmoActive;

    public Sprite UEESprite;
    public Sprite BNUSprite;
    public Sprite VNDSprite;
    public Sprite XINSprite;
    public Sprite DEVSprite;
    public Sprite UNCSprite;

    public Material tunnelMaterial;
    public Material starFieldMaterial;

    public int layerMask;
    
    //tunnels
    private int numPoint = 51;
    private float t;
    private bool areTunnelsActives;
    private GameObject jumpPointContainer;
    // Start is called before the first frame update
    void Start()
    {
        systemsJson = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/SystemsList.json");
        jumpPointsJson = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/JumpPoints.json");

        jumpPointList = new List<JumpPoint>();
        var systemList = new List<KeyValuePair<GameObject, int>>();

        StarSystems jsonStarSystems = JsonUtility.FromJson<StarSystems>(systemsJson);
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
            starSystemInfos.code = starSystem.code;

            float systemSize = float.Parse(starSystem.aggregated_size, System.Globalization.CultureInfo.InvariantCulture);
            starSystemInfos.size = Mathf.Round(systemSize).ToString();

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
            StarSystemGO.transform.GetChild(0).GetComponent<TextMeshPro>().text = starSystem.code;

            StarSystemGO.AddComponent<AppaerenceAndSizeKeeper>().scaleMultiplier = 1;
            

            starSystemInfos.json = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/Systems/" + starSystem.name + ".json");
            if(starSystem.id == 320)starSystemInfos.json = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/Systems/Nul1.json"); //a file can't be named nul so... 

            systemList.Add(new KeyValuePair<GameObject, int>(StarSystemGO, starSystem.id));

        }
        
        jumpPointContainer = new GameObject("Jump-Points");
        Tunnels jsonTunnels = JsonUtility.FromJson<Tunnels>(jumpPointsJson);
        foreach(Tunnel tunnel in jsonTunnels.tunnels)
        {
            JumpPoint jumpPoint = new JumpPoint();
            jumpPoint.entryId = int.Parse(tunnel.entry_id); 
            jumpPoint.exitId = int.Parse(tunnel.exit_id);

            int entrySystemID = int.Parse(tunnel.entry.star_system_id);
            int exitSystemID = int.Parse(tunnel.exit.star_system_id); 
        
            Vector3[] positions = new Vector3[numPoint];

            Vector3 entrySystemPos = new Vector3();
            Vector3 exitSystemPos = new Vector3();
            foreach(KeyValuePair<GameObject, int> system in systemList)
            {
                if(system.Value == entrySystemID)
                {
                    entrySystemPos = system.Key.gameObject.transform.position;
                    jumpPoint.entryGO = system.Key.gameObject;
                }
                if(system.Value == exitSystemID)
                {
                    exitSystemPos = system.Key.gameObject.transform.position;
                    jumpPoint.exitGO = system.Key.gameObject;
                }

            }
            GameObject tunnelGO = new GameObject(tunnel.entry.designation);
            tunnelGO.transform.parent = jumpPointContainer.transform;
            LineRenderer line = tunnelGO.AddComponent<LineRenderer>();
            line.positionCount = numPoint;
            Vector3 v1 = entrySystemPos + (exitSystemPos - entrySystemPos) * 1/3 + new Vector3(7 + entrySystemID / 1000, 7 + exitSystemID / 1000, 7 + entrySystemID / 1000);
            Vector3 v2 = entrySystemPos + (exitSystemPos - entrySystemPos) * 2/3 - new Vector3(7 + entrySystemID / 1000, 7 + exitSystemID / 1000, 7 + entrySystemID / 1000);
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

            line.textureMode = LineTextureMode.RepeatPerSegment;
            line.material = tunnelMaterial;
            jumpPoint.size = "LARGE";
            if(tunnel.size == "M")
            {
                jumpPoint.size = "MEDIUM";
                line.material.SetColor("_MainColor", new Color32(188, 117, 83, 14));
            }
            if(tunnel.size == "S")
            {
                jumpPoint.size = "SMALL";
                line.material.SetColor("_MainColor", new Color32(248, 117, 83, 14));
            }
            TunnelTraficScript tunnelTraficScript = tunnelGO.AddComponent<TunnelTraficScript>();
            jumpPointList.Add(jumpPoint);
            
        }
        StarFieldGenerator starFieldGenerator = gameObject.GetComponent<StarFieldGenerator>();
        starFieldGenerator.generateStarField(new Vector3(0, 0, 0), 600, 2000, new Color32(122, 122, 122, 255), new Color32(150, 180, 207, 255), starFieldMaterial);
        
        cameraMode = 0;
        areTunnelsActives = true;

        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cameraMode == 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 2000))
            {
                if(!hasHitSoundPlayed)
                {
                    AudioManager.play("GOHover");
                    hasHitSoundPlayed = true;
                }
                
                if(hit.transform)
                {
                    if(Input.GetMouseButtonUp(1))
                    {
                        SelectSystem(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if(hasHitSoundPlayed)hasHitSoundPlayed = false;
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                LoadAndEnterSystem();
            }
        }
        else if(cameraMode == 1)
        {

            layerMask = 1 << LayerMask.NameToLayer("CO");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if(!hasHitSoundPlayed)
                {
                    AudioManager.play("GOHover");
                    hasHitSoundPlayed = true;
                }
                if (!isHoverGizmoActive)
                {
                    HoverGizmo.SetActive(true);
                    HoverGizmo.transform.position = hit.transform.position;
                    HoverGizmo.transform.parent = hit.transform.parent;
                    HoverGizmo.transform.localScale = Vector3.Scale(hit.transform.localScale, new Vector3(1.5f, 1.5f, 1.5f));
                    isHoverGizmoActive = true;
                }
                if (hit.transform)
                {

                    if (Input.GetMouseButtonUp(1))
                    {
                        SelectCO(hit.transform.gameObject);
                        COSelected = true;
                    }
                }
            }
            else
            {
                if (isHoverGizmoActive)
                {
                    HoverGizmo.SetActive(false);
                    isHoverGizmoActive = false;
                }
                if(hasHitSoundPlayed)hasHitSoundPlayed = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (COSelected == true)
                {
                    COSelected = false;
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
                areTunnelsActives = false;
            }
            else
            {
                jumpPointContainer.SetActive(true);
                areTunnelsActives = true;
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
    }

    public void SelectCO(GameObject gameObject)
    {
        selectedObject = gameObject;
        mainCamera.GetComponent<CameraScript>().SelectCO(selectedObject);
        DiscScript discScript = UIContainer.GetComponent<DiscScript>();
        discScript.selectedObject = selectedObject;
        discScript.mode = 1;
        discScript.LoadDisc();
    }

    public void LoadAndEnterSystem()
    {
        if(areTunnelsActives == true)jumpPointContainer.SetActive(false);
        UIContainer.GetComponent<DiscScript>().UnloadDisc();
        this.GetComponent<StarSystemGeneration>().LoadSystem(selectedSystem);
        mainCamera.GetComponent<CameraScript>().EnterSystem(selectedSystem);
        cameraMode = 1;
        AudioManager.play("SystemAmbient");
        UIContainer.GetComponent<DiscScript>().UnloadInfobox();
        systemName.SetActive(true);
        systemName.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedSystem.GetComponent<SystemsInfosScript>().code;

    }

    public void UnloadAndExitSystem()
    {
        if(areTunnelsActives == true)jumpPointContainer.SetActive(true);
        HoverGizmo.transform.parent = transform.parent;
        cameraMode = 0;
        mainCamera.GetComponent<CameraScript>().ExitSystem(selectedSystem);
        AudioManager.stop("SystemAmbient");
        systemName.SetActive(false);
        spaceBoxColor.GetComponent<MeshRenderer>().material.color = new Color(0,0,0,0);
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
    public string code;
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

public class JumpPoint
{
    public int id;
    public int entryId;
    public GameObject entryGO;
    public int exitId;
    public GameObject exitGO;
    public string size;
}

