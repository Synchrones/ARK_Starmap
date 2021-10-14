using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StarSystemGeneration : MonoBehaviour
{
    public GameObject SSContentPrefab;
    public GameObject starPrefab;
    public GameObject bluePlanetPrefab;
    public GameObject brownPlanetPrefab;
    public GameObject greenPlanetPrefab;
    public GameObject gazPlanetPrefab;
    public GameObject jumpPointPrefab;
    public GameObject spaceStationPrefab;
    public GameObject asteroidFieldPrefab;
    public GameObject planetRingPrefab;
    public GameObject asteroidFrontPrefab;
    public GameObject asteroidMiddlePrefab;
    public GameObject asteroidFarPrefab;

    public Material orbitMaterial;

    //planet textures
    public Texture carbonPlanetTexture;
    public Texture chthonianPlanetTexture;
    public Texture earthLikePlanetTexture;
    public Texture icePlanetTexture;
    public Texture lavaPlanetTexture;
    public Texture oceanPlanetTexture;
    public Texture rockyPlanetTexture;
    public Texture smogPlanetTexture;
        //Sol
    public Texture mercuryTexture;
    public Texture venusTexture;
    public Texture earthTexture;
    public Texture marsTexture;
    public Texture jupiterTexture;
    public Texture saturnTexture;
    public Texture uranusTexture;
    public Texture neptuneTexture;
    public Texture plutoTexture;
    public Texture moonTexture;
        //Stanton
    public Texture hurstonTexture;
    public Texture crusaderTexture;
    public Texture arccorpTexture;
    public Texture microtechTexture;

    //star textures
    public Texture2D starTexture0;
    public Texture2D starTexture1; 
    public Texture2D starTexture2; 
    public Texture2D starTexture3; 
    public Texture2D starTexture4; 
    public Texture2D starTexture5; 
    public Texture2D starTexture6; 
    public Texture2D starTexture7; 
    public Texture2D starTexture8; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSystem(GameObject starSystem)
    {
        List<JumpPoint> jumpPointList = GetComponent<StarSystemsScript>().jumpPointList;

        GameObject SSContentGO = Instantiate(SSContentPrefab, starSystem.transform.position, Quaternion.identity);
        SSContentGO.transform.parent = starSystem.transform;
        SystemsInfosScript starSystemInfos = starSystem.GetComponent<SystemsInfosScript>();
        SSData jsonSSData = JsonUtility.FromJson<SSData>(starSystemInfos.json);

        foreach (SystemContent systemContent in jsonSSData.data.resultset)
        {
            SSContentGO.name = systemContent.name;
            var planetList = new List<KeyValuePair<GameObject, int>>();
            var moonList = new List<KeyValuePair<GameObject, int>>();

            foreach(CelestialObject celestialObject in systemContent.celestial_objects)
            {
                GameObject celestialGO;

                float longitude;
                float latitude;
                float distance;

                switch(celestialObject.type)
                {
                    case "STAR":
                        /*
                        stars are formed by two superposed spheres
                        map : texture to use (number 0 to 8)
                        rotations 1 & 2 : rotations of the two parts (<0.5 = rotation to the left, >0.5 = to the right -> still need to verify this)
                        radius : size of the star
                        color : colors to use
                        flares : ?
                        texture : ?
                        corona(funny) : ? something to do with the wavy effect around the star
                        sphere : ?
                        scale min/scale max : min and max scale of the star (some of them have a pulsating effect -> see nul)

                        */
                        // TODO: add glow, min and max size, corona effect, fix glow on darker stars
                        StarDatas starDatas = celestialObject.shader_data.sun;

                        celestialGO = Instantiate(starPrefab, starSystem.transform.position, Quaternion.identity);
                        Material starMaterialOuter = celestialGO.transform.GetChild(0).GetComponent<Renderer>().material;
                        Material starMaterialInner = celestialGO.transform.GetChild(1).GetComponent<Renderer>().material;

                        starMaterialInner.mainTexture = (Texture)this.GetType().GetField("starTexture" + (string)starDatas.map).GetValue(this);
                        starMaterialOuter.mainTexture = (Texture)this.GetType().GetField("starTexture" + (string)starDatas.map).GetValue(this);
                        
                        if(starDatas.color1.Length > 7)
                        {
                            string parsedColor1 = starDatas.color1.Split('(', ')')[1];
                            string parsedColor2 = starDatas.color2.Split('(', ')')[1];

                            starMaterialOuter.SetColor("_Color", new Color32(byte.Parse(parsedColor1.Split(',')[0]), byte.Parse(parsedColor1.Split(',')[1]), byte.Parse(parsedColor1.Split(',')[2]), 255));
                            starMaterialInner.SetColor("_Color", new Color32(byte.Parse(parsedColor2.Split(',')[0]), byte.Parse(parsedColor2.Split(',')[1]), byte.Parse(parsedColor2.Split(',')[2]), 255));
                        }
                        else
                        {
                            Color color1;
                            Color color2;

                            ColorUtility.TryParseHtmlString(starDatas.color1, out color1);
                            ColorUtility.TryParseHtmlString(starDatas.color2, out color2);

                            starMaterialOuter.color = color1;
                            starMaterialInner.color = color2;
                        }
                        
                        
                        
                        
                        StarScript starScript = celestialGO.AddComponent<StarScript>();
                        starScript.starTexture = (Texture2D)starMaterialOuter.mainTexture;
                        starScript.rotation1 = starDatas.rotation1;
                        starScript.rotation1 = starDatas.rotation2;

                        celestialGO.name = celestialObject.designation;
                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localScale *=  celestialObject.shader_data.radius * 2;

                        if(systemContent.type == "BINARY")
                        {
                            longitude = celestialObject.longitude * Mathf.Deg2Rad;
                            distance = celestialObject.distance;
            
                            celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), 0, distance * Mathf.Sin(longitude));
                        }


                        break;

                    case "PLANET":

                        switch(celestialObject.appearance)
                        {
                            case "PLANET_BLUE":
                                celestialGO = Instantiate(bluePlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                            case "PLANET_BROWN":
                                celestialGO = Instantiate(brownPlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                            case "PLANET_GREEN":
                                celestialGO = Instantiate(greenPlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                            case "PLANET_GAS":
                                celestialGO = Instantiate(gazPlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                            default:
                                celestialGO = Instantiate(bluePlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                        }
                        Renderer sphereRenderer = celestialGO.transform.GetChild(1).GetChild(1).GetComponent<Renderer>();
                        
                        //Common types
                        switch(celestialObject.texture.slug)
                        {
                            case "3s8vnafjwn09p":
                                sphereRenderer.material.mainTexture = chthonianPlanetTexture;
                                break;

                            case "duxhoei3b7nnm":
                                sphereRenderer.material.mainTexture = earthLikePlanetTexture;
                                break;
                                
                            case "j52x7qoth74yi":
                                sphereRenderer.material.mainTexture = icePlanetTexture;
                                break;

                            case "dgaoam8owcym1":
                                sphereRenderer.material.mainTexture = lavaPlanetTexture;
                                break;

                            case "8c99m3v6sfl24":
                                sphereRenderer.material.mainTexture = oceanPlanetTexture;
                                break;

                            case "jnd5qqer9z13g":
                                sphereRenderer.material.mainTexture = rockyPlanetTexture;
                                break;

                            case "8nd3j4zcsqfmz":
                                sphereRenderer.material.mainTexture = smogPlanetTexture;
                                break;
                            case "p47vvencfylfl":
                                sphereRenderer.material.mainTexture = carbonPlanetTexture;
                                break;
                        }

                        //Stanton
                        if(systemContent.name == "Stanton")
                        {
                            switch(celestialObject.texture.slug)
                            {
                                case "frurip2hsngx8":
                                    sphereRenderer.material.mainTexture = hurstonTexture;
                                    break;

                                case "qzf7kii1vu7k7":
                                    sphereRenderer.material.mainTexture = crusaderTexture;
                                    break;
                                    
                                case "2wkohq7v67kco":
                                    sphereRenderer.material.mainTexture = arccorpTexture;
                                    break;

                                case "l0arnhgmoajuy":
                                    sphereRenderer.material.mainTexture = microtechTexture;
                                    break;
                            }
                        }
                        
                        //Sol
                        if(systemContent.name == "Sol")
                        {
                            switch(celestialObject.texture.slug)
                            {
                                case "vx5nehwypz25d":
                                    sphereRenderer.material.mainTexture = mercuryTexture;
                                    break;

                                case "j8qfxa304mrco":
                                    sphereRenderer.material.mainTexture = venusTexture;
                                    break;
                                    
                                case "5dvd8cognsxbg":
                                    sphereRenderer.material.mainTexture = earthTexture;
                                    break;

                                case "7wes0jfwwl1bc":
                                    sphereRenderer.material.mainTexture = marsTexture;
                                    break;

                                case "yyxagrxfet5z8":
                                    sphereRenderer.material.mainTexture = jupiterTexture;
                                    break;

                                case "ps9wli4v169nb":
                                    sphereRenderer.material.mainTexture = saturnTexture;
                                    break;

                                case "0jumx7t0c3ytz":
                                    sphereRenderer.material.mainTexture = uranusTexture;
                                    break;

                                case "94je2ho8w8fbe":
                                    sphereRenderer.material.mainTexture = neptuneTexture;
                                    break;

                                case "pmfkvj1mwmq2z":
                                    sphereRenderer.material.mainTexture = plutoTexture;
                                    break;

                                case "9g2xvstat60bi":
                                    sphereRenderer.material.mainTexture = moonTexture;
                                    break;
                            }
                        }
                        
                        celestialGO.name = celestialObject.name;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance;
                    
                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.localScale /= 15;
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 1;

                        GameObject orbitContainer = new GameObject("orbit");
                        orbitContainer.transform.position = starSystem.transform.position;
                        orbitContainer.transform.parent = celestialGO.transform;

                        for(int i = 0; i < 60; i++)
                        {
                            GameObject orbit = new GameObject("orbit" + i);
                            orbit.transform.parent = orbitContainer.transform;
                            orbit.transform.position = orbitContainer.transform.position;
                            LineRenderer line = orbit.AddComponent<LineRenderer>();
                            line.useWorldSpace = false;
                            line.material = orbitMaterial;

                            Vector3 start = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (i * 360 / 60)) * distance, 0, Mathf.Sin(Mathf.Deg2Rad * (i * 360 / 60)) * distance);
                            Vector3 end = new Vector3(Mathf.Cos(Mathf.Deg2Rad * ((i + 1) * 360 / 60)) * distance, 0, Mathf.Sin(Mathf.Deg2Rad * ((i + 1) * 360 / 60)) * distance);
                            
                            
                            line.SetPosition(0, start);
                            line.SetPosition(1, end);

                            line.startWidth = 0.01f;
                            line.endWidth = 0.01f;

                            

                        }
                        orbitContainer.transform.rotation = Quaternion.Euler(0, - celestialObject.longitude, celestialObject.latitude);
                        orbitContainer.AddComponent<WidthKeeper>();
                        

                        planetList.Add(new KeyValuePair<GameObject, int>(celestialGO, celestialObject.id));

                        break;

                    case "SATELLITE":

                        switch(celestialObject.appearance)
                        {
                            case "PLANET_BLUE":
                                celestialGO = Instantiate(bluePlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                            case "PLANET_BROWN":
                                celestialGO = Instantiate(brownPlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                            case "PLANET_GREEN":
                                celestialGO = Instantiate(greenPlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                            default:
                                celestialGO = Instantiate(brownPlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                        }
                        
                        foreach(KeyValuePair<GameObject, int> planet in planetList)
                        {
                            if(planet.Value == celestialObject.parent_id) celestialGO.transform.parent = planet.Key.transform;
                        }
                        celestialGO.name = celestialObject.name;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance * 500;

                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.localScale /= 50;
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 0.2f;

                        moonList.Add(new KeyValuePair<GameObject, int>(celestialGO, celestialObject.id));

                        break;

                    case "JUMPPOINT":

                        celestialGO = Instantiate(jumpPointPrefab, starSystem.transform.position, Quaternion.identity);

                        celestialGO.name = celestialObject.name;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance;

                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.layer = 6;

                        celestialGO.transform.GetChild(0).gameObject.AddComponent<JumpTailScript>();

                        break;

                    case "MANMADE":

                        celestialGO = Instantiate(spaceStationPrefab, starSystem.transform.position, Quaternion.identity);

                        celestialGO.name = celestialObject.designation;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance;

                        bool flag = false;
                        foreach(KeyValuePair<GameObject, int> planet in planetList)
                        {
                            if(planet.Value == celestialObject.parent_id)
                            {
                                celestialGO.transform.parent = planet.Key.transform;
                                flag = true;
                            } 
                        }

                        if(!flag)
                        {
                            foreach(KeyValuePair<GameObject, int> moon in moonList)
                            {
                                if(moon.Value == celestialObject.parent_id)
                                {
                                    celestialGO.transform.parent = moon.Key.transform;
                                    flag = true;
                                } 
                            }
                        }
                        if(!flag) celestialGO.transform.parent = SSContentGO.transform;

                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.localScale /= 50;
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 0.2f;

                        break;

                    case "ASTEROID_BELT":
                    
                        switch(celestialObject.subtype_id)
                        {
                            case "50":

                                GameObject ringContainer = new GameObject("Ring Container");

                                int pieceCount = 120;
                                float angle = 360 / pieceCount;
                                for(int i = 0; i < pieceCount; i++)
                                {
                                    Quaternion rotation = Quaternion.AngleAxis(i * angle, Vector3.up);
                                    Vector3 direction = rotation * Vector3.left;
                                    Vector3 position = SSContentGO.transform.position + (direction * celestialObject.distance);
                                    celestialGO = Instantiate(asteroidFrontPrefab, position, rotation);
                                    celestialGO.transform.localScale /= 15; 
                                    celestialGO.transform.parent = ringContainer.transform;
                                }
                                celestialGO = ringContainer;
                                celestialGO.transform.parent = SSContentGO.transform;
                                break;
                            
                            case "51":
                                celestialGO = Instantiate(planetRingPrefab, SSContentGO.transform.position, Quaternion.identity);

                                foreach(KeyValuePair<GameObject, int> planet in planetList)
                                {
                                    if(planet.Value == celestialObject.parent_id)
                                    {
                                        celestialGO.transform.parent = planet.Key.transform;
                                        celestialGO.transform.position = planet.Key.transform.position;
                                    } 
                                }
                                foreach(KeyValuePair<GameObject, int> moon in moonList) //Yela has strange rings...
                                {
                                    if(moon.Value == celestialObject.parent_id)
                                    {
                                        GameObject.Destroy(celestialGO);
                                        celestialGO = Instantiate(asteroidFrontPrefab, moon.Key.transform.position, Quaternion.identity);
                                        celestialGO.transform.parent = moon.Key.transform;
                                    
                                    } 
                                }

                                celestialGO.transform.localScale /= 15;
                                break;

                            default:

                                celestialGO = new GameObject();

                                break;


                        }
                        

                        break;

                    case "ASTEROID_FIELD":

                        celestialGO = Instantiate(asteroidFieldPrefab, starSystem.transform.position, Quaternion.identity);
                        celestialGO.name = celestialObject.name;

                        celestialGO.transform.parent = SSContentGO.transform;
                        

                        distance = celestialObject.distance;
                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(0), 0, 0);
                        celestialGO.transform.localScale /= 50; 
                        celestialGO.layer = 6;
                        

                        break;


                    default:

                        print("unknown CO type" + celestialObject.type);
                        celestialGO = new GameObject();

                        break;

                }

                celestialGO.AddComponent<COInfosScript>();
                COInfosScript coInfosScript = celestialGO.GetComponent<COInfosScript>();

                coInfosScript.coName = celestialObject.name;
                coInfosScript.description = celestialObject.description;
                coInfosScript.appearance = celestialObject.appearance;
                coInfosScript.type = celestialObject.type;
                coInfosScript.lastTimeModified = celestialObject.time_modified;
                coInfosScript.habitable = celestialObject.habitable;
                coInfosScript.population = celestialObject.sensor_population;
                coInfosScript.economy = celestialObject.sensor_economy;
                coInfosScript.danger = celestialObject.sensor_danger;
                coInfosScript.fairchanceact = celestialObject.fairchanceact;
                coInfosScript.size = celestialObject.size;
                coInfosScript.subtype = celestialObject.subtype.name;
                foreach (Affiliation affiliation in celestialObject.affiliation)
                {
                    coInfosScript.affiliationID = affiliation.id;
                    coInfosScript.affiliationName = affiliation.name;
                }

                GameObject targetSystem = gameObject;;
                if(celestialObject.type == "JUMPPOINT")
                {
                    foreach(JumpPoint jumpPoint in jumpPointList)
                    {
                        if(jumpPoint.entryId == celestialObject.id)
                        {
                            targetSystem = jumpPoint.exitGO;
                        }
                        if(jumpPoint.exitId == celestialObject.id)
                        {
                            targetSystem = jumpPoint.entryGO;
                        }
                    }

                    celestialGO.transform.rotation = Quaternion.LookRotation(targetSystem.transform.position - transform.position);
                    
                }
            }
        }
    }

    public void UnloadSystem(GameObject gameObject)
    {
        
    }



    [System.Serializable]
    public class SSData
    {
        public Data data;
        
    }

    [System.Serializable]
    public class Data
    {
        public SystemContent[] resultset;
    }



    [System.Serializable]
    public class SystemContent
    {
        public string name;
        public string type;

        public CelestialObject[] celestial_objects;

    }

    
    
    [System.Serializable]
    public class CelestialObject
    {
        public string type;
        public string designation;
        public string name;
        public string description;
        public string appearance;
        public float longitude;
        public float latitude;
        public float distance;
        public int id;
        public int parent_id;
        public string habitable;
        public string sensor_danger;
        public string sensor_economy;
        public string sensor_population;
        public string fairchanceact;
        public string size;
        public string time_modified;
        public string subtype_id;

        public Subtype subtype;
        public Affiliation[] affiliation;
        public COTexture texture;
        public ShaderData shader_data;
    }

    [System.Serializable]
    public class Subtype
    {
        public string name;
    }

    [System.Serializable]
    public class Affiliation
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class COTexture
    {
        public string slug;
    }

    [System.Serializable]
    public class ShaderData
    {
        public StarDatas sun;
        public float radius;
    }

    [System.Serializable]
    public class StarDatas
    {
        public string map;
        public string color1;
        public string color2;
        public float rotation1;
        public float rotation2;
        

    }

}
