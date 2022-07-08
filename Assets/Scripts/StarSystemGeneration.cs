using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StarSystemGeneration : MonoBehaviour
{
    public GameObject SSContentPrefab;
    public GameObject starPrefab;
    public GameObject spaceBoxColorsGO;
    public GameObject bandPrefab;

    //Planets
    public GameObject bluePlanetPrefab;
    public GameObject brownPlanetPrefab;
    public GameObject greenPlanetPrefab;
    public GameObject gazPlanetPrefab;

    public GameObject jumpPointPrefab;

    //Space Stations
    public GameObject defaultSpaceStationPrefab;
    public GameObject POSpaceStationPrefab;
    public GameObject kareahSpaceStationPrefab;
    public GameObject covalexSpaceStationPrefab;
    public GameObject scanHubSpaceStationPrefab;
    public GameObject commArraySpaceStationPrefab;
    public GameObject ARKSpaceStationPrefab;


    //Asteroide Rings
    public GameObject asteroidFieldPrefab;
    public GameObject planetRingPrefab;
    public GameObject asteroidFrontPrefab;
    public GameObject asteroidMiddlePrefab;
    public GameObject asteroidFarPrefab;

    public GameObject landingZonePrefab;
    public GameObject namePrefab;

    public Material orbitMaterial;
    public Material starFieldMaterial;

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
    public Texture crusaderTextureNew;
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

    public List<GameObject> celestialObjectList;

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
                        
                        starMaterialOuter.SetColor("_Color", parseColor(starDatas.color1));
                        starMaterialInner.SetColor("_Color", parseColor(starDatas.color2));
                        celestialGO.transform.GetChild(2).GetComponent<Light>().color = parseColor(systemContent.shader_data.lightColor);

                        StarScript starScript = celestialGO.AddComponent<StarScript>();
                        starScript.starTexture = (Texture2D)starMaterialOuter.mainTexture;
                        starScript.rotation1 = starDatas.rotation1;
                        starScript.rotation1 = starDatas.rotation2;

                        celestialGO.name = celestialObject.designation;
                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localScale *=  celestialObject.shader_data.radius;
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 2;

                        if(systemContent.type == "BINARY")
                        {
                            longitude = celestialObject.longitude * Mathf.Deg2Rad;
                            distance = celestialObject.distance;
            
                            celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), 0, distance * Mathf.Sin(longitude));
                        }

                        generateInfos(celestialGO, celestialObject, celestialObject.designation);
                        

                        break;

                    case "PLANET":

                        celestialGO = setCOAppaerenceAndTexture(celestialObject.appearance, celestialObject.texture.slug, systemContent.name, starSystem);
                        
                        celestialGO.name = celestialObject.name;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance;
                    
                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Sin(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.GetChild(0).transform.Rotate(Vector3.forward, celestialObject.axial_tilt);
                        celestialGO.transform.GetChild(1).transform.Rotate(Vector3.forward, celestialObject.axial_tilt);


                        if(celestialObject.shader_data.highlight.color1 != null)
                        {
                            Color planetColor = parseColor(celestialObject.shader_data.highlight.color1);
                            celestialGO.transform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(planetColor.r * 3, planetColor.g * 3, planetColor.b * 3, 1);
                        } 
                        
                        float planetSize = systemContent.shader_data.planetsSize.min + float.Parse(celestialObject.size, System.Globalization.CultureInfo.InvariantCulture) * 0.000001f;
                        celestialGO.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 1.5f;

                        drawOrbit(celestialGO, starSystem, celestialObject, distance);

                        Mesh planetLineMesh = celestialGO.transform.GetChild(0).GetChild(1).GetComponent<MeshFilter>().mesh;
                        planetLineMesh.SetSubMesh(0, new UnityEngine.Rendering.SubMeshDescriptor(indexStart:0, indexCount:planetLineMesh.GetSubMesh(0).indexCount, topology:MeshTopology.Lines));
                        
                        PlanetScript planetScript = celestialGO.AddComponent<PlanetScript>();
                        planetScript.toRotate.Add(celestialGO.transform.GetChild(1).gameObject);

                        GameObject landingZonesContainer = new GameObject("LandingZonesContainer");
                        landingZonesContainer.transform.position = celestialGO.transform.position;
                        landingZonesContainer.transform.parent = celestialGO.transform;
                        planetScript.toRotate.Add(landingZonesContainer.gameObject);
                        if(celestialObject.id == 2026 || celestialObject.id == 2027 || celestialObject.id == 1693 || celestialObject.id == 1694 || celestialObject.id == 1695 || celestialObject.id == 1692 || celestialObject.id == 1723)
                        {
                            SSData planetDatas = JsonUtility.FromJson<SSData>(System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Jsons/Planets/" + celestialObject.name + ".json"));
                            foreach(SystemContent planetContent in planetDatas.data.resultset)
                            {
                                foreach(Children children in planetContent.children)
                                {
                                    if(children.type == "LZ")
                                    {
                                        GameObject LZGO = Instantiate(landingZonePrefab, celestialGO.transform.position, Quaternion.identity);
                                        LZGO.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
                                        LZGO.transform.Rotate(-children.latitude, -children.longitude - 90, 0);
                                        LZGO.transform.parent = landingZonesContainer.transform;
                                    }
                                }
                            }
                        }
                        if(celestialObject.name == "")
                        {
                            generateInfos(celestialGO, celestialObject, celestialObject.designation);
                        }
                        else generateInfos(celestialGO, celestialObject, celestialObject.name);
                        
                        

                        planetList.Add(new KeyValuePair<GameObject, int>(celestialGO, celestialObject.id));

                        break;

                    case "SATELLITE":

                        celestialGO = setCOAppaerenceAndTexture(celestialObject.appearance, celestialObject.texture.slug, systemContent.name, starSystem);
                        
                        foreach(KeyValuePair<GameObject, int> planet in planetList)
                        {
                            if(planet.Value == celestialObject.parent_id) celestialGO.transform.parent = planet.Key.transform;
                        }
                        celestialGO.name = celestialObject.name;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = 3 + celestialObject.distance;

                        if(celestialObject.shader_data.highlight.color1 != null)
                        {
                            Color planetColor = parseColor(celestialObject.shader_data.highlight.color1);
                            celestialGO.transform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(planetColor.r * 3, planetColor.g * 3, planetColor.b * 3, 1);
                        }
                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Sin(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.localScale /= 150 / float.Parse(celestialObject.size, System.Globalization.CultureInfo.InvariantCulture); //the 2nd argument has something to do with the decimal separator ("." instead of ",")...
                        celestialGO.transform.Rotate(Vector3.forward, celestialObject.axial_tilt);
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 2;

                        celestialGO.AddComponent<PlanetScript>();

                        Mesh moonLineMesh = celestialGO.transform.GetChild(0).GetChild(1).GetComponent<MeshFilter>().mesh;
                        moonLineMesh.SetSubMesh(0, new UnityEngine.Rendering.SubMeshDescriptor(indexStart:0, indexCount:moonLineMesh.GetSubMesh(0).indexCount, topology:MeshTopology.Lines));

                        moonList.Add(new KeyValuePair<GameObject, int>(celestialGO, celestialObject.id));

                        break;

                    case "JUMPPOINT":

                        celestialGO = Instantiate(jumpPointPrefab, starSystem.transform.position, Quaternion.identity);

                        celestialGO.name = celestialObject.name;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance;

                        celestialGO.transform.parent = SSContentGO.transform;
                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Sin(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.localScale /= 30;
                        celestialGO.layer = 6;

                        celestialGO.AddComponent<SphereCollider>().radius = 2;

                        generateInfos(celestialGO, celestialObject, celestialObject.designation);

                        celestialGO.AddComponent<JumpPointScript>();

                        break;

                    case "MANMADE":
                        
                        if(celestialObject.appearance == "DEFAULT")
                        {
                            celestialGO = Instantiate(defaultSpaceStationPrefab, starSystem.transform.position, Quaternion.identity);
                        }
                        else
                        {
                            switch(celestialObject.model.slug)
                            {
                                case "hq0yb5nb8kfgo": //PO
                                    celestialGO = Instantiate(POSpaceStationPrefab, starSystem.transform.position, Quaternion.identity);
                                    break;

                                case "5bxoelq8c8uju": //Kareah
                                    celestialGO = Instantiate(kareahSpaceStationPrefab, starSystem.transform.position, Quaternion.identity);
                                    break;

                                case "et0gvwd7ihhug": //Covalex Gundo
                                    celestialGO = Instantiate(covalexSpaceStationPrefab, starSystem.transform.position, Quaternion.identity);
                                    break;

                                case "2c7l2nene7ipk": //Cry-Astro
                                    celestialGO = Instantiate(defaultSpaceStationPrefab, starSystem.transform.position, Quaternion.identity); //for some reason this is default prefab
                                    break;

                                case "afrlkpdjwl5nm": //ScanHub
                                    celestialGO = Instantiate(scanHubSpaceStationPrefab, starSystem.transform.position, Quaternion.identity);
                                    break;
                                
                                case "pkuskri0s4twx": //CommArray
                                    celestialGO = Instantiate(commArraySpaceStationPrefab, starSystem.transform.position, Quaternion.identity);
                                    break;
                                
                                case "sxj9reqlg4ktj": //The ARK
                                    celestialGO = Instantiate(ARKSpaceStationPrefab, starSystem.transform.position, Quaternion.identity);
                                    break;
                                default:
                                    celestialGO = new GameObject();
                                    break;
                            }
                        }
                        
                        //TODO fix space station generation distance (as well as moons)
                        celestialGO.name = celestialObject.designation;

                        longitude = celestialObject.longitude * Mathf.Deg2Rad;
                        latitude = celestialObject.latitude * Mathf.Deg2Rad;
                        distance = celestialObject.distance;
                        if(distance < 0.05f)distance *= 2500; //without this, space station are generated to close for some reason

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

                        celestialGO.transform.localPosition = new Vector3(distance * Mathf.Cos(longitude), distance * Mathf.Sin(latitude), distance * Mathf.Sin(longitude));
                        celestialGO.transform.localScale /= 150;
                        celestialGO.layer = 6;
                        celestialGO.AddComponent<SphereCollider>().radius = 2;

                        if(flag && starSystem.name != "Stanton")drawOrbit(celestialGO, celestialGO.transform.parent.gameObject, celestialObject, distance);
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
                            
                            case "51": //planet ring
                                celestialGO = Instantiate(planetRingPrefab, SSContentGO.transform.position, Quaternion.identity);

                                foreach(KeyValuePair<GameObject, int> planet in planetList)
                                {
                                    if(planet.Value == celestialObject.parent_id)
                                    {
                                        celestialGO.transform.parent = planet.Key.transform;
                                        celestialGO.transform.position = planet.Key.transform.position;
                                        celestialGO.transform.Rotate(Vector3.forward, float.Parse(planet.Key.GetComponent<COInfosScript>().axial_tilt));
                                    } 
                                }
                                foreach(KeyValuePair<GameObject, int> moon in moonList) //Yela has strange rings...
                                {
                                    if(moon.Value == celestialObject.parent_id)
                                    {
                                        GameObject.Destroy(celestialGO);
                                        celestialGO = Instantiate(asteroidFrontPrefab, moon.Key.transform.position, Quaternion.identity);
                                        celestialGO.transform.parent = moon.Key.transform;
                                        celestialGO.transform.Rotate(Vector3.right, celestialObject.axial_tilt);
                                    } 
                                }

                                celestialGO.transform.localScale /= 40;
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
                coInfosScript.designation = celestialObject.designation;
                coInfosScript.appearance = celestialObject.appearance;
                coInfosScript.type = celestialObject.type;
                coInfosScript.lastTimeModified = celestialObject.time_modified;
                coInfosScript.habitable = celestialObject.habitable;
                coInfosScript.population = celestialObject.sensor_population;
                coInfosScript.economy = celestialObject.sensor_economy;
                coInfosScript.danger = celestialObject.sensor_danger;
                coInfosScript.fairchanceact = celestialObject.fairchanceact;
                coInfosScript.size = celestialObject.size;
                coInfosScript.axial_tilt = celestialObject.axial_tilt.ToString();
                coInfosScript.subtype = celestialObject.subtype.name;

                if (celestialObject.affiliation.Length == 0)
                { 
                    SystemsInfosScript systemAffiliation = SSContentGO.gameObject.GetComponentInParent<SystemsInfosScript>();
                    coInfosScript.affiliationName = systemAffiliation.affiliationName;
                    coInfosScript.affiliationID = systemAffiliation.affiliationID;

                }
                else
                {
                    foreach (Affiliation affiliation in celestialObject.affiliation)
                    {
                        coInfosScript.affiliationID = affiliation.id;
                        coInfosScript.affiliationName = affiliation.name;
                    }
                }
                
                


                GameObject targetSystem = gameObject;;
                if(celestialObject.type == "JUMPPOINT")
                {
                    foreach(JumpPoint jumpPoint in jumpPointList)
                    {
                        if(jumpPoint.entryId == celestialObject.id)
                        {
                            targetSystem = jumpPoint.exitGO;
                            coInfosScript.size = jumpPoint.size;
                        }
                        if(jumpPoint.exitId == celestialObject.id)
                        {
                            targetSystem = jumpPoint.entryGO;
                            coInfosScript.size = jumpPoint.size;
                        }
                    }
                    celestialGO.transform.rotation = Quaternion.LookRotation(targetSystem.transform.position - transform.position);
                }

                celestialObjectList.Add(celestialGO);
                
            }
            Color spaceColor;
            ColorUtility.TryParseHtmlString(systemContent.shader_data.lightColor, out spaceColor);
            spaceColor.a = 0.05f;
            spaceBoxColorsGO.GetComponent<MeshRenderer>().material.color = spaceColor;

            GameObject greenFrostBands = Instantiate(bandPrefab, starSystem.transform.position, Quaternion.identity);
            greenFrostBands.transform.parent = SSContentGO.transform;
            greenFrostBands.transform.Rotate(Vector3.right, 90);
            greenFrostBands.transform.localScale *= systemContent.frost_line + 5;
            Material greenFrostBandsDatas = greenFrostBands.GetComponent<MeshRenderer>().material;
            greenFrostBandsDatas.SetFloat("_GreenBandStart", systemContent.habitable_zone_inner);
            greenFrostBandsDatas.SetFloat("_GreenBandEnd", systemContent.habitable_zone_outer);
            greenFrostBandsDatas.SetFloat("_FrostBandPos", systemContent.frost_line);


            StarFieldGenerator starFieldGenerator = gameObject.GetComponent<StarFieldGenerator>();
            StarFieldData starFieldData = systemContent.shader_data.starfield;
            GameObject starField = starFieldGenerator.generateStarField(SSContentGO.transform.position, starFieldData.radius, starFieldData.count, parseColor(starFieldData.color1), parseColor(starFieldData.color2), starFieldMaterial);
            starField.transform.parent = SSContentGO.transform;
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
        public string type;
        public float frost_line;
        public float habitable_zone_inner;
        public float habitable_zone_outer;
        public ShaderData shader_data;
        public Children[] children;
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
        public float axial_tilt;
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
        public COModel model;
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
    public class COModel
    {
        public string slug;
    }
    [System.Serializable]
    public class ShaderData
    {
        public StarDatas sun;
        public StarFieldData starfield;
        public PlanetsSize planetsSize;
        public Highlight highlight;
        public float radius;
        public string lightColor;
    }

    [System.Serializable]
    public class StarFieldData
    {
        public float radius;
        public float count;
        public string color1;
        public string color2;
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

    [System.Serializable]
    public class PlanetsSize
    {
        public float min;
    }

    [System.Serializable]
    public class Highlight
    {
        public string color1;
    }

    [System.Serializable]
    public class Children
    {
        public string type;
        public string designation;
        public float longitude;
        public float latitude;

    }


    private GameObject setCOAppaerenceAndTexture(string appaerence, string texture, string systemName, GameObject system)
    {
        GameObject celestialGO;
        switch (appaerence)
        {
            case "PLANET_BLUE":
                celestialGO = Instantiate(bluePlanetPrefab, system.transform.position, Quaternion.identity);
                break;
            case "PLANET_BROWN":
                celestialGO = Instantiate(brownPlanetPrefab, system.transform.position, Quaternion.identity);
                break;
            case "PLANET_GREEN":
                celestialGO = Instantiate(greenPlanetPrefab, system.transform.position, Quaternion.identity);
                break;
            case "PLANET_GAS":
                celestialGO = Instantiate(gazPlanetPrefab, system.transform.position, Quaternion.identity);
                break;
            default:
                celestialGO = Instantiate(bluePlanetPrefab, system.transform.position, Quaternion.identity);
                break;
        }


        Renderer sphereRenderer = celestialGO.transform.GetChild(1).GetChild(1).GetComponent<Renderer>();

        //Common types
        switch (texture)
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

            case "frurip2hsngx8":
                sphereRenderer.material.mainTexture = hurstonTexture;
                break;

            case "qzf7kii1vu7k7":
                sphereRenderer.material.mainTexture = crusaderTexture;
                break;

            case "cd3676xek3zbw":
                sphereRenderer.material.mainTexture = crusaderTextureNew;
                break;

            case "2wkohq7v67kco":
                sphereRenderer.material.mainTexture = arccorpTexture;
                break;

            case "l0arnhgmoajuy":
                sphereRenderer.material.mainTexture = microtechTexture;
                break;
            
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
        return celestialGO;
    }

    private void drawOrbit(GameObject celestialGO, GameObject orbitCenter, CelestialObject celestialObject, float distance)
    {
        GameObject orbitContainer = new GameObject("orbit");
        orbitContainer.transform.position = orbitCenter.transform.position;
        orbitContainer.transform.parent = celestialGO.transform;

        Mesh mesh = orbitContainer.AddComponent<MeshFilter>().mesh;
        orbitContainer.AddComponent<MeshRenderer>().material = orbitMaterial;

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        for (int i = 0; i <= 180; i++)
        {
            vertices.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * (i * 2)) * distance, 0, Mathf.Sin(Mathf.Deg2Rad * (i * 2)) * distance));
            indices.Add(i);
        }
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.LineStrip, 0);

        orbitContainer.transform.rotation = Quaternion.Euler(0, -celestialObject.longitude, celestialObject.latitude);
        if(celestialObject.type == "MANMADE")orbitContainer.transform.localScale = Vector3.one;
    }

    private void generateInfos(GameObject celestialGO, CelestialObject celestialObject, string name)
    {
        GameObject InfosGO = new GameObject("Infos");
        InfosGO.transform.position = celestialGO.transform.position;
        InfosGO.transform.parent = celestialGO.transform;
        drawOrbit(InfosGO, celestialGO, celestialObject, 1);
        GameObject NameGO = Instantiate(namePrefab, InfosGO.transform.position, Quaternion.identity);
        NameGO.transform.parent = InfosGO.transform;
        AppaerenceAndSizeKeeper appaerence = InfosGO.AddComponent<AppaerenceAndSizeKeeper>();
        appaerence.isCelestialObject = true;
        appaerence.scaleMultiplier = 0.3f;
        appaerence.rescalingTextGO = NameGO;
        appaerence.rescalingCircleGO = InfosGO.transform.GetChild(0).gameObject;
        appaerence.rescalingCircleGO.transform.rotation = Quaternion.Euler(90, 0, 0);
        NameGO.GetComponent<TextMeshPro>().text = "<color=#19e8ff>" + name + "<br><color=#33528C><size=90%>" + celestialObject.type;
    }

    private Color32 parseColor(string color)
    {
        if(color.Length > 7)
        {
            string parsedColor = color.Split('(', ')')[1];
            return new Color32(byte.Parse(parsedColor.Split(',')[0]), byte.Parse(parsedColor.Split(',')[1]), byte.Parse(parsedColor.Split(',')[2]), 255);
        }
        else
        {
            Color parsedColor;
            ColorUtility.TryParseHtmlString(color, out parsedColor);
            return parsedColor;
        }
    }
}
