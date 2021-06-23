using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemGeneration : MonoBehaviour
{
    public GameObject SSContentPrefab;




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
        
        GameObject SSContent = (GameObject)Instantiate(SSContentPrefab, starSystem.transform.position, Quaternion.identity);
        SSContent.transform.parent = starSystem.transform;
        SystemsInfosScript starSystemInfos = starSystem.GetComponent<SystemsInfosScript>();
        SSData jsonSSData = JsonUtility.FromJson<SSData>(starSystemInfos.json);
        SSContent.name = jsonSSData.data.rowcount;

        foreach (SystemContent systemContent in jsonSSData.data.resultset)
        {
            SSContent.name = systemContent.name;

            foreach(CelestialObject celestialObject in systemContent.celestial_objects)
            {
                print(celestialObject.type);
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
        public string rowcount;
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

    }

}
