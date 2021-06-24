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

            foreach(CelestialObject celestialObject in systemContent.celestial_objects)
            {
                GameObject celestialGO;
                if(celestialObject.type == "STAR")
                {
                    celestialGO = Instantiate(starPrefab, starSystem.transform.position, Quaternion.identity);
                    celestialGO.name = celestialObject.designation;
                    celestialGO.transform.parent = SSContentGO.transform;
                    celestialGO.transform.localScale /= 3;
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
                    float distance = celestialObject.distance * 5;
                
                    celestialGO.transform.parent = SSContentGO.transform;
                    celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), 0, distance * Mathf.Sin(longitude));
                    celestialGO.transform.localScale /= 3;

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
                    float distance = celestialObject.distance * 1000;

                    celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), 0, distance * Mathf.Sin(longitude));
                    celestialGO.transform.localScale /= 6;
                }
            
            }
        }
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
        public float distance;
        public int id;
        public int parent_id;
    }

}
