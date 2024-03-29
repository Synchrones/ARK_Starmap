using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/* TODO: (main TODO) :
    -add landing zones section to infobox
   TODO: (bug fixes) :  
    -oberon star is... small -> several scaling issues
    -unknow Co type "POI" (vega Vanduul attack)
    -systems looks bigger when on the side of the screen
    -camera seems to be vibrating when looking to really small objects
    -names too long for infobox (see planet in Pallas system)

   TODO: (graphics) :
    -rework star shader (colors, animations, "corona"...)
    -reworks planetary rings
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
    public GameObject displaySection;
    public GameObject upLeftUI;
    InputManagerScript InputManager;
    AudioManagerScript AudioManager;
    bool hasHitSoundPlayed;
    bool systemHit;
    GameObject clickedGO;
    public GameObject lastSystemHit;
    public GameObject lastHoveredCO;
    public GameObject HoverGizmo;
    bool isHoverGizmoActive;

    public Sprite UEESprite;
    public Sprite BNUSprite;
    public Sprite VNCLSprite;
    public Sprite XIANSprite;
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
    public List<GameObject> tunnelsS;
    public List<GameObject> tunnelsM;
    public List<GameObject> tunnelsL;

    public List<GameObject> systemsUNC;
    public List<GameObject> systemsDEV;
    public List<GameObject> systemsXIAN;
    public List<GameObject> systemsVNCL;
    public List<GameObject> systemsBANU;
    public List<GameObject> systemsUEE;

    void Start()
    {
        systemsJson = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/SystemsList.json");
        jumpPointsJson = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/JumpPoints.json");

        jumpPointList = new List<JumpPoint>();
        var systemList = new List<KeyValuePair<GameObject, int>>();

        StarSystems jsonStarSystems = JsonUtility.FromJson<StarSystems>(systemsJson);
        systemsUNC = new List<GameObject>();
        systemsDEV = new List<GameObject>();
        systemsXIAN = new List<GameObject>();
        systemsVNCL = new List<GameObject>();
        systemsBANU = new List<GameObject>();
        systemsUEE = new List<GameObject>();

        GameObject systemContainer = new GameObject("Systems");
        foreach (StarSystem starSystem in jsonStarSystems.starSystems)
        {
            GameObject StarSystemGO = Instantiate(StarSystemPrefab, new Vector3(starSystem.posX * 7, starSystem.posZ * 7, starSystem.posY * 7), Quaternion.identity);
            StarSystemGO.transform.parent = systemContainer.transform;
            StarSystemGO.name = starSystem.name;
            
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
                        systemsUEE.Add(StarSystemGO);
                        break;

                    case "2":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = BNUSprite;
                        systemsBANU.Add(StarSystemGO);
                        break;

                    case "3":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = VNCLSprite;
                        systemsVNCL.Add(StarSystemGO);
                        break;

                    case "4":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = XIANSprite;
                        systemsXIAN.Add(StarSystemGO);
                        break;

                    case "7":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = DEVSprite;
                        systemsDEV.Add(StarSystemGO);
                        break;

                    case "8":

                        StarSystemGO.GetComponent<SpriteRenderer>().sprite = UNCSprite;
                        systemsUNC.Add(StarSystemGO);
                        break;
                }
            }
            StarSystemGO.transform.GetChild(0).GetComponent<TextMeshPro>().text = starSystem.code;    
            StarSystemGO.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            
            StarSystemGO.AddComponent<AppaerenceAndSizeKeeper>().scaleMultiplier = 0.7f;
            

            starSystemInfos.json = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/Systems/" + starSystem.name + ".json");
            if(starSystem.id == 320)starSystemInfos.json = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/Systems/Nul1.json"); //a file can't be named nul so... 

            systemList.Add(new KeyValuePair<GameObject, int>(StarSystemGO, starSystem.id));

        }
        
        jumpPointContainer = new GameObject("Jump-Points");
        tunnelsL = new List<GameObject>();
        tunnelsM = new List<GameObject>();
        tunnelsS = new List<GameObject>();
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
            if(tunnel.size == "L")
            {
                jumpPoint.size = "LARGE";
                tunnelsL.Add(tunnelGO);
            }
            else if(tunnel.size == "M")
            {
                jumpPoint.size = "MEDIUM";
                line.material.SetColor("_MainColor", new Color32(188, 117, 83, 14));
                tunnelsM.Add(tunnelGO);
            }
            else if(tunnel.size == "S")
            {
                jumpPoint.size = "SMALL";
                line.material.SetColor("_MainColor", new Color32(248, 117, 83, 14));
                tunnelsS.Add(tunnelGO);
            }
            tunnelGO.SetActive(false);
            TunnelTraficScript tunnelTraficScript = tunnelGO.AddComponent<TunnelTraficScript>();
            jumpPointList.Add(jumpPoint);
            
        }
        StarFieldGenerator starFieldGenerator = gameObject.GetComponent<StarFieldGenerator>();
        GameObject starField = starFieldGenerator.generateStarField(new Vector3(0, 0, 0), 2000, 2000, new Color32(122, 122, 122, 255), new Color32(150, 180, 207, 255), starFieldMaterial);
        starField.transform.position = new Vector3(200, 0, 0);
        
        cameraMode = 0;
        areTunnelsActives = true;

        for (int i = 0; i < 2; i++)
        {
            Mesh mesh = HoverGizmo.transform.GetChild(i).GetComponent<MeshFilter>().mesh;
            List<int> indices = new List<int>();
            for (int j = 0; j < mesh.GetSubMesh(0).vertexCount; j++)
            {
                indices.Add(j);
            }
            mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
        }
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        InputManager = GameObject.Find("InputManager").GetComponent<InputManagerScript>();

        GameObject toggleSpacebox = GameObject.Find("GameobjectReferences").GetComponent<GameobjectReferencesScript>().disableSpacebox; 
        toggleSpacebox.GetComponent<ButtonHandler>().spacebox = GameObject.Find("spacebox"); 
        if(toggleSpacebox.GetComponent<Toggle>().isOn)
        {
            GameObject.Find("spacebox").SetActive(false);
        }
    }

    void Update()
    {
        if(cameraMode == 0)
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 2000))
            {
                if(!systemHit)
                {
                    if(!EventSystem.current.IsPointerOverGameObject(-1))
                    {
                        AudioManager.play("GOHover");
                        systemHit = true;
                        lastSystemHit = hit.transform.gameObject;
                        if(!hit.transform.gameObject.GetComponent<SystemsInfosScript>().lockOpacity)
                        {
                            StopAllCoroutines();
                            StartCoroutine(setSystemOpacity(hit.transform.gameObject, 1));
                        }
                    }
                }
                if(Input.GetMouseButtonDown(1)){
                    clickedGO = hit.transform.gameObject;
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    clickedGO = hit.transform.gameObject;
                }
                
                if(Input.GetMouseButtonUp(1))
                {
                    if(clickedGO == hit.transform.gameObject)
                    {
                        SelectSystem(hit.transform.gameObject);
                    }
                    else
                    {
                        clickedGO = null;
                    }
                }
                if(Input.GetMouseButtonUp(0))
                {
                    if(clickedGO == hit.transform.gameObject)
                    {
                        selectedSystem = hit.transform.gameObject;
                        LoadAndEnterSystem();
                    }
                }
            }
            else
            {
                if(systemHit)
                {
                    systemHit = false;
                    if(selectedSystem != null)
                    {
                        if(selectedSystem != lastSystemHit)
                        {
                            if(!lastSystemHit.GetComponent<SystemsInfosScript>().lockOpacity)
                            {
                                StopAllCoroutines();
                                StartCoroutine(setSystemOpacity(lastSystemHit, 0.5f));
                            }
                            
                        }
                    }
                    else
                    {
                        if(!lastSystemHit.GetComponent<SystemsInfosScript>().lockOpacity)
                        {
                            StopAllCoroutines();
                            StartCoroutine(setSystemOpacity(lastSystemHit, 0.5f));
                        }
                        
                    }
                }
            }

            if(Input.GetKeyDown(KeyCode.Return))
            {
                LoadAndEnterSystem();
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(selectedSystem != null)
                {
                    UIContainer.GetComponent<DiscScript>().UnloadDisc();
                    UIContainer.GetComponent<DiscScript>().UnloadInfobox();
                    if(!lastSystemHit.GetComponent<SystemsInfosScript>().lockOpacity)
                    {
                        StopAllCoroutines();
                        StartCoroutine(setSystemOpacity(lastSystemHit, 0.5f));
                    }
                    selectedSystem = null;
                } 
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
                if (!isHoverGizmoActive || lastHoveredCO != hit.transform.gameObject)
                {
                    HoverGizmo.SetActive(true);
                    HoverGizmo.transform.position = hit.transform.position;
                    HoverGizmo.transform.parent = hit.transform.parent;
                    HoverGizmo.transform.localScale = Vector3.Scale(hit.transform.localScale, new Vector3(1.2f, 1.2f, 1.2f));
                    isHoverGizmoActive = true;
                    lastHoveredCO = hit.transform.gameObject;
                }
                if(Input.GetMouseButtonDown(1)){
                    clickedGO = hit.transform.gameObject;
                }

                if (Input.GetMouseButtonUp(1))
                {
                    if(clickedGO == hit.transform.gameObject)
                    {
                        SelectCO(hit.transform.gameObject);
                        COSelected = true;
                    }
                    else
                    {
                        clickedGO = null;
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
                    UnselectCO();
                }
                else UnloadAndExitSystem();
            }
        }
        
        if(Input.GetKeyDown(InputManager.SHTunnels))
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                if(displaySection.transform.GetChild(0).GetChild(i).GetComponent<ButtonHandler>().activated)count += 1;
            }
            if(count >= 2)
            {
                showTunnels(false);
                areTunnelsActives = false;
            }
            else
            {
                showTunnels(true);
                areTunnelsActives = true;
            }
        }
    }

    public void showTunnels(bool state)
    {
        for (int i = 0; i < 3; i++)
        {
            ButtonHandler button = displaySection.transform.GetChild(0).GetChild(i).GetComponent<ButtonHandler>();

            if(state)button.currentStateColor = new Color(1, 1, 1, 0.6f);
            else button.currentStateColor = new Color(1, 1, 1, 0.1f);

            if(button.activated ^ state)
            {
                if(i == 0)
                {
                    button.showTunnelsS();
                    button.backToCurrentColor();
                }
                else if(i == 1)
                {
                    button.showTunnelsM();
                    button.backToCurrentColor();
                }
                else if(i == 2)
                {
                    button.showTunnelsL();
                    button.backToCurrentColor();
                }
            }
            if(state)button.activated = true;
            else button.activated = false;
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
        upLeftUI.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
        upLeftUI.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
        selectedObject = gameObject;
        mainCamera.GetComponent<CameraScript>().SelectCO(selectedObject);
        DiscScript discScript = UIContainer.GetComponent<DiscScript>();
        discScript.selectedObject = selectedObject;
        discScript.mode = 1;
        discScript.LoadDisc();
    }

    public void UnselectCO()
    {
        COSelected = false;
        upLeftUI.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
        upLeftUI.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
        UIContainer.GetComponent<DiscScript>().UnloadDisc();
        UIContainer.GetComponent<DiscScript>().UnloadInfobox();
    }

    public void LoadAndEnterSystem()
    {
        if(!lastSystemHit.GetComponent<SystemsInfosScript>().lockOpacity)
        {
            StopAllCoroutines();
            StartCoroutine(setSystemOpacity(lastSystemHit, 0.5f));
        }
        displaySection.SetActive(false);
        upLeftUI.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        upLeftUI.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
        upLeftUI.transform.GetChild(4).gameObject.SetActive(true);
        jumpPointContainer.SetActive(false);
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
        displaySection.SetActive(true);
        upLeftUI.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
        upLeftUI.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        upLeftUI.transform.GetChild(4).gameObject.SetActive(false);
        jumpPointContainer.SetActive(true);
        HoverGizmo.SetActive(false);
        isHoverGizmoActive = false;
        HoverGizmo.transform.parent = transform.parent;
        cameraMode = 0;
        mainCamera.GetComponent<CameraScript>().ExitSystem(selectedSystem);
        AudioManager.stop("SystemAmbient");
        systemName.SetActive(false);
        spaceBoxColor.GetComponent<MeshRenderer>().material.color = new Color(0,0,0,0);
        selectedSystem = null;
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
    public IEnumerator setSystemOpacity(GameObject system, float opacity)
    {  
        SpriteRenderer spriteRenderer = system.GetComponent<SpriteRenderer>();
        float opacityDifference = opacity - spriteRenderer.color.a;

        for (int i = 0; i < 30; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, spriteRenderer.color.a + opacityDifference / 30);
            foreach(TextMeshPro tmp in system.transform.GetComponentsInChildren<TextMeshPro>())
            {
                tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, spriteRenderer.color.a + opacityDifference / 30);
            }
            yield return null;
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

