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
    public Texture CarbonPlanetTexture;
    public Texture ChthonianPlanetTexture;
    public Texture EarthLikePlanetTexture;
    public Texture IcePlanetTexture;
    public Texture LavaPlanetTexture;
    public Texture OceanPlanetTexture;
    public Texture RockyPlanetTexture;
    public Texture SmogPlanetTexture;
        //Sol
    public Texture MercuryTexture;
    public Texture VenusTexture;
    public Texture EarthTexture;
    public Texture MarsTexture;
    public Texture JupiterTexture;
    public Texture SaturnTexture;
    public Texture UranusTexture;
    public Texture NeptuneTexture;
    public Texture PlutoTexture;
    public Texture MoonTexture;
        //Stanton
    public Texture HurstonTexture;
    public Texture CrusaderTexture;
    public Texture ArccorpTexture;
    public Texture MicrotechTexture;


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
                        star is formed by two superposed textures
                        map : texture to use (number 0 to 8)
                        rotations 1 & 2 : rotations of the two parts (<5 = rotation to the left, >5 = to the right -> still need to verify this)
                        radius : size of the star
                        color : colors to use
                        flare : ?
                        texture : ?
                        corona(funny) : ? something to do with the wavy effect around the star
                        sphere : ?
                        scale min/scale max : min and max scale of the star (some of them have a pulsating effect -> see nul)
                        
                        */
                        celestialGO = Instantiate(starPrefab, starSystem.transform.position, Quaternion.identity);
                        celestialGO.name = celestialObject.designation;
                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localScale /= 5;

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
                                sphereRenderer.material.mainTexture = ChthonianPlanetTexture;
                                break;

                            case "duxhoei3b7nnm":
                                sphereRenderer.material.mainTexture = EarthLikePlanetTexture;
                                break;
                                
                            case "j52x7qoth74yi":
                                sphereRenderer.material.mainTexture = IcePlanetTexture;
                                break;

                            case "dgaoam8owcym1":
                                sphereRenderer.material.mainTexture = LavaPlanetTexture;
                                break;

                            case "8c99m3v6sfl24":
                                sphereRenderer.material.mainTexture = OceanPlanetTexture;
                                break;

                            case "jnd5qqer9z13g":
                                sphereRenderer.material.mainTexture = RockyPlanetTexture;
                                break;

                            case "8nd3j4zcsqfmz":
                                sphereRenderer.material.mainTexture = SmogPlanetTexture;
                                break;
                            case "p47vvencfylfl":
                                sphereRenderer.material.mainTexture = CarbonPlanetTexture;
                                break;
                        }

                        //Stanton
                        if(systemContent.name == "Stanton")
                        {
                            switch(celestialObject.texture.slug)
                            {
                                case "frurip2hsngx8":
                                    sphereRenderer.material.mainTexture = HurstonTexture;
                                    break;

                                case "qzf7kii1vu7k7":
                                    sphereRenderer.material.mainTexture = CrusaderTexture;
                                    break;
                                    
                                case "2wkohq7v67kco":
                                    sphereRenderer.material.mainTexture = ArccorpTexture;
                                    break;

                                case "l0arnhgmoajuy":
                                    sphereRenderer.material.mainTexture = MicrotechTexture;
                                    break;
                            }
                        }
                        
                        //Sol
                        if(systemContent.name == "Sol")
                        {
                            switch(celestialObject.texture.slug)
                            {
                                case "vx5nehwypz25d":
                                    sphereRenderer.material.mainTexture = MercuryTexture;
                                    break;

                                case "j8qfxa304mrco":
                                    sphereRenderer.material.mainTexture = VenusTexture;
                                    break;
                                    
                                case "5dvd8cognsxbg":
                                    sphereRenderer.material.mainTexture = EarthTexture;
                                    break;

                                case "7wes0jfwwl1bc":
                                    sphereRenderer.material.mainTexture = MarsTexture;
                                    break;

                                case "yyxagrxfet5z8":
                                    sphereRenderer.material.mainTexture = JupiterTexture;
                                    break;

                                case "ps9wli4v169nb":
                                    sphereRenderer.material.mainTexture = SaturnTexture;
                                    break;

                                case "0jumx7t0c3ytz":
                                    sphereRenderer.material.mainTexture = UranusTexture;
                                    break;

                                case "94je2ho8w8fbe":
                                    sphereRenderer.material.mainTexture = NeptuneTexture;
                                    break;

                                case "pmfkvj1mwmq2z":
                                    sphereRenderer.material.mainTexture = PlutoTexture;
                                    break;

                                case "9g2xvstat60bi":
                                    sphereRenderer.material.mainTexture = MoonTexture;
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

}
