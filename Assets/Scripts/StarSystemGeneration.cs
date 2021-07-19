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
                GameObject celestialGO;
                if(celestialObject.type == "STAR")
                {
                    celestialGO = Instantiate(starPrefab, starSystem.transform.position, Quaternion.identity);
                    celestialGO.name = celestialObject.designation;
                    celestialGO.transform.parent = SSContentGO.transform;
                    
                }
                if(celestialObject.type == "PLANET")
                {
                    
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

                    float longitude = celestialObject.longitude * Mathf.Deg2Rad;
                    float distance = celestialObject.distance;
                
                    celestialGO.transform.parent = SSContentGO.transform;
                    celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), 0, distance * Mathf.Sin(longitude));
                    celestialGO.transform.localScale /= 10;
                    celestialGO.layer = 6;
                    celestialGO.AddComponent<SphereCollider>().radius = 1;

                    GameObject orbitContainer = new GameObject("orbit");
                    orbitContainer.transform.position = starSystem.transform.position;
                    orbitContainer.transform.parent = celestialGO.transform;
                    drawOrbit(orbitContainer, distance, 0.01f);
                    orbitContainer.GetComponent<LineRenderer>().GetPosition(1);
                    orbitContainer.AddComponent<WidthKeeper>();
                    

                    planetList.Add(new KeyValuePair<GameObject, int>(celestialGO, celestialObject.id));
                }
                if(celestialObject.type == "SATELLITE")
                {
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

                    float longitude = celestialObject.longitude * Mathf.Deg2Rad;
                    float latitude = celestialObject.latitude * Mathf.Deg2Rad;
                    float distance = celestialObject.distance * 1500;

                    celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                    celestialGO.transform.localScale /= 50;
                    celestialGO.layer = 6;
                    celestialGO.AddComponent<SphereCollider>().radius = 0.2f;

                    moonList.Add(new KeyValuePair<GameObject, int>(celestialGO, celestialObject.id));
                }
                if(celestialObject.type == "JUMPPOINT")
                {
                    celestialGO = Instantiate(jumpPointPrefab, starSystem.transform.position, Quaternion.identity);

                    celestialGO.name = celestialObject.name;

                    float longitude = celestialObject.longitude * Mathf.Deg2Rad;
                    float latitude = celestialObject.latitude * Mathf.Deg2Rad;
                    float distance = celestialObject.distance;

                    celestialGO.transform.parent = SSContentGO.transform;
                    celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Tan(latitude), distance * Mathf.Sin(longitude));
                    celestialGO.layer = 6;

                }
                if(celestialObject.type == ("MANMADE"))
                {
                    celestialGO = Instantiate(spaceStationPrefab, starSystem.transform.position, Quaternion.identity);

                    celestialGO.name = celestialObject.designation;

                    float longitude = celestialObject.longitude * Mathf.Deg2Rad;
                    /*if(celestialObject.latitude != null){*/float latitude = celestialObject.latitude * Mathf.Deg2Rad;//}
                    float distance = celestialObject.distance;

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
        public string appearance;
        public float longitude;
        public float latitude;
        public float distance;
        public int id;
        public int parent_id;
    }



    public void drawOrbit(GameObject gameObject, float radius, float lineWidth)
    {
        var segments = 120;
        var line = gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;
        var pointCount = segments + 1;
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i += 1)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius) + gameObject.transform.parent.parent.position;
        }
        line.SetPositions(points);
        line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        line.startColor = Color.cyan;
        line.endColor = Color.cyan;
        
    }

}
