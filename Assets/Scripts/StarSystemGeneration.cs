using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
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
                GameObject celestialGO = new GameObject();

                float longitude;
                float latitude;
                float distance;

                switch(celestialObject.type)
                {
                    case "STAR":

                        celestialGO = Instantiate(starPrefab, starSystem.transform.position, Quaternion.identity);
                        celestialGO.name = celestialObject.designation;
                        celestialGO.transform.parent = SSContentGO.transform;

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
                                celestialGO = Instantiate(brownPlanetPrefab, starSystem.transform.position, Quaternion.identity);
                                break;
                        }
                        celestialGO.name = celestialObject.name;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance;
                    
                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.localScale /= 10;
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 1;

                        GameObject orbitContainer = new GameObject("orbit");
                        orbitContainer.transform.position = starSystem.transform.position;
                        orbitContainer.transform.rotation = Quaternion.Euler(0, - celestialObject.longitude, celestialObject.latitude);
                        orbitContainer.transform.parent = celestialGO.transform;
                        drawOrbit(orbitContainer, distance, 0.01f, longitude);
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
                        distance = celestialObject.distance * 1500;

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

                        foreach(KeyValuePair<GameObject, int> planet in planetList)
                        {
                            if(planet.Value == celestialObject.parent_id) celestialGO.transform.parent = planet.Key.transform;
                        }
                        foreach(KeyValuePair<GameObject, int> moon in moonList)
                        {
                            if(moon.Value == celestialObject.parent_id) celestialGO.transform.parent = moon.Key.transform;
                        }

                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 0.2f;

                        break;

                    case "ASTEROID_BELT":

                        break;
                    default:

                        print("unknown CO type" + celestialObject.type);
                        GameObject.Destroy(celestialGO);
                        
                        break;

                }

                celestialGO.AddComponent<COInfosScript>();
                COInfosScript coInfosScript = celestialGO.GetComponent<COInfosScript>();

                coInfosScript.coName = celestialObject.name;
                coInfosScript.description = celestialObject.description;
                coInfosScript.type = celestialObject.type;
                coInfosScript.lastTimeModified = celestialObject.time_modified;
                coInfosScript.habitable = celestialObject.habitable;
                coInfosScript.population = celestialObject.sensor_population;
                coInfosScript.economy = celestialObject.sensor_economy;
                coInfosScript.danger = celestialObject.sensor_danger;
                coInfosScript.fairchanceact = celestialObject.fairchanceact;
                coInfosScript.size = celestialObject.size;
                coInfosScript.subtype = celestialObject.subtype.name;
            
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

        public Subtype subtype;
    }

    [System.Serializable]
    public class Subtype
    {
        public string name;
    }



    public void drawOrbit(GameObject gameObject, float radius, float lineWidth, float longitude)
    {
        var segments = 120;
        var line = gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;
        var pointCount = segments + 1;
        var points = new Vector3[pointCount];

        
        for (int i = 0; i < pointCount; i += 1)
        {
            var rad = Mathf.Deg2Rad * (i * 360 / segments);
            points[i] = new Vector3(Mathf.Cos(rad + longitude) * radius, 0, Mathf.Sin(rad + longitude) * radius);
        }
        line.SetPositions(points);
        line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        line.startColor = Color.cyan;
        line.endColor = Color.cyan;
        
    }

}
