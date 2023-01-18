using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    Vector3 currentDisplacement;
    Vector2 currentRotation;
    float currentZoom;

    Vector3 displacementAcceleration;
    Vector2 rotationAcceleration;
    
    Vector3 cameraOffset;
    Vector3 center;

    //represent the closeness of the camera to the max and min y pos (close = 0, far = 1)
    float closenessToPole;
    float yRotationSpeedModifier;
    
    float displacementSpeed;
    float rotationSpeed;
    float moveToPosSpeed;
    float zoomSpeed;
    (float, float) zoomLimits;


    Vector3 newPos;
    Vector3 newCenter;
    Vector3 newOffset;

    public GameObject scriptContainer;
    StarSystemsScript starSystemsScript;
    
    //UI
    public GameObject UIContainer;
    bool buttonPressed;
    DiscScript discScript;
    InfoboxScript infoboxScript;
    float clickTime;
    bool hitUI;

    //Sounds
    AudioManagerScript AudioManager;
    bool isZoomSoundPlaying;
    bool isRotateSoundPlaying;
    
    //Other
    public GameObject spacebox;

    void Start()
    {
        center = new Vector3(0, 0, 75);
        cameraOffset = transform.position - center;

        displacementSpeed = 0.1f;
        rotationSpeed = 1;
        moveToPosSpeed = 0.5f;
        zoomSpeed = 1;
        zoomLimits = (20, 800);

        hitUI = false;
        buttonPressed = false;
        clickTime = 0;

        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        discScript = UIContainer.GetComponent<DiscScript>();
        infoboxScript = UIContainer.GetComponent<InfoboxScript>();
        starSystemsScript = scriptContainer.GetComponent<StarSystemsScript>();
        
    }

    private void LateUpdate()
    {
        displacementAcceleration = Vector3.zero;
        //prevent camera from moving when clicking UI element 
        if(!buttonPressed)
        {   
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            bool flag = false;
            foreach(var result in raycastResults)
            {
                if(result.gameObject.layer == 5)
                {
                    hitUI = true;
                    flag = true;
                }
            }
            if(!flag)
            {
                hitUI = false;
            }
        }


        if (!hitUI)
        {
            //close disc when clicking on nothing
            if(discScript.isActive)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    clickTime = Time.time;
                }
                if(Input.GetMouseButtonUp(0))
                {
                    if(Time.time - clickTime < 0.08f)
                    {
                        discScript.UnloadDisc();
                        discScript.isInfoboxActive = false;
                        infoboxScript.UnloadInfobox();
                        if(starSystemsScript.cameraMode == 0)
                        {
                            if(!starSystemsScript.selectedSystem.GetComponent<SystemsInfosScript>().lockOpacity)
                            {
                                StartCoroutine(starSystemsScript.setSystemOpacity(starSystemsScript.selectedSystem, 0.5f));
                            }
                            starSystemsScript.selectedSystem = null;
                        }
                        else
                        {
                            if(starSystemsScript.COSelected)
                            {
                                starSystemsScript.UnselectCO();
                            }
                        }
                    }
                }
            }

            if(Input.GetKey(KeyCode.Mouse1))
            {
                buttonPressed = true;
                displacementAcceleration = ((transform.right * Input.GetAxis("Mouse X") + transform.up * Input.GetAxis("Mouse Y")) * -0.1f * displacementSpeed);
            }
            else if(Input.GetKey(KeyCode.Mouse0))
            {
                buttonPressed = true;
                if(closenessToPole < 0.1f)
                {
                    if(Input.GetAxis("Mouse Y") < 0 && cameraOffset.y > 0 || Input.GetAxis("Mouse Y") > 0 && cameraOffset.y < 0)
                    {
                        yRotationSpeedModifier = closenessToPole;
                    }
                    else yRotationSpeedModifier = 1;
                } 
                else yRotationSpeedModifier = 1;
                currentRotation = ((Vector3.left * Input.GetAxis("Mouse Y") * yRotationSpeedModifier + Vector3.up * Input.GetAxis("Mouse X")) * -0.5f  * rotationSpeed);
            }
        }

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        if(Input.GetMouseButton(2))
        {
            zoomInput += Input.GetAxis("Mouse Y");
        }
        
        if(zoomLimits.Item1 > cameraOffset.magnitude && zoomInput > 0 || zoomLimits.Item2 < cameraOffset.magnitude && zoomInput < 0)
        {
            currentZoom /= 1.2f;
        }
        else
        {
            if(zoomInput != 0)
            {
                currentZoom += zoomInput * zoomSpeed * cameraOffset.magnitude / 100; 
                currentZoom = Mathf.Min(10, currentZoom);
            }
        }

        //currentDisplacement /= 1.02f;
        displacementAcceleration -= currentDisplacement / 5;
        currentDisplacement += displacementAcceleration / 10;
        if(closenessToPole < 0.2f)
        {
            currentRotation = new Vector2(currentRotation.x / 1.2f, currentRotation.y / 1.02f);
        }
        else currentRotation /= 1.015f;
        currentZoom /= 1.05f;
        

        applyMovement();
        transform.position = center + cameraOffset;
        displacementSpeed = cameraOffset.magnitude / 15;
        transform.LookAt(center);

        spacebox.transform.position = center;
        spacebox.transform.localScale = new Vector3(cameraOffset.magnitude / 2, cameraOffset.magnitude / 2, cameraOffset.magnitude / 2);

        if(hitUI)
        {
            buttonPressed = false;
        }

        if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            hitUI = false;
            buttonPressed = false;
        }

        //sounds
        if(Mathf.Abs(currentZoom) > 0.0001f)
        {
            if(!isZoomSoundPlaying)
            {
                AudioManager.play("CamZoom");
                isZoomSoundPlaying = true;
            }
        }
        else
        {
            if(isZoomSoundPlaying)
            {
                AudioManager.stop("CamZoom");
                isZoomSoundPlaying = false;
            }
        }
        if(currentRotation.magnitude > 0.01f)
        {
            if(!isRotateSoundPlaying)
            {
                AudioManager.play("CamRotate");
                isRotateSoundPlaying = true;
            }
        }
        else
        {
            if(isRotateSoundPlaying)
            {
                AudioManager.stop("CamRotate");
                isRotateSoundPlaying = false;
            }
        }
    }

    private void applyMovement()
    {
        center += currentDisplacement;
        Vector3 rightVector = new Vector3(-transform.forward.z, 0, transform.forward.x);
        cameraOffset = Quaternion.AngleAxis(currentRotation.x, rightVector) * Quaternion.AngleAxis(currentRotation.y, Vector3.down) * cameraOffset;
        closenessToPole = rightVector.magnitude;
        
        if(cameraOffset.magnitude - 0.05 > Vector3.Distance(cameraOffset, cameraOffset + transform.forward * currentZoom) || currentZoom < 0)
        {
            cameraOffset += transform.forward * currentZoom;
        }
        else
        {
            cameraOffset -= transform.forward * currentZoom;
        }
    }

    public void SelectSystem(GameObject gameObject)
    {
        newCenter = gameObject.transform.position;
        CalculateNewOffset(200);

        StartCoroutine(MoveToPos(center, newCenter, cameraOffset, newOffset));
    }

    public void EnterSystem(GameObject gameObject)
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<AppaerenceAndSizeKeeper>().enabled = false;
        
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        
        float systemSize = 10 + float.Parse(gameObject.GetComponent<SystemsInfosScript>().size);


        zoomLimits = (0.1f, systemSize);
        newCenter = gameObject.transform.position;

        CalculateNewOffset(1 + systemSize / 50);
        StartCoroutine(MoveToPos(center, newCenter, cameraOffset, newOffset));
        
    }

    public void ExitSystem(GameObject gameObject)
    {
        GameObject.Destroy(gameObject.transform.GetChild(2).gameObject);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<AppaerenceAndSizeKeeper>().enabled = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);

        zoomLimits = (20, 800);

        CalculateNewOffset(200);
        StartCoroutine(MoveToPos(center, newCenter, cameraOffset, newOffset));
    }

    public void SelectCO(GameObject gameObject)
    {
        newCenter = gameObject.transform.position;
        CalculateNewOffset(15 * gameObject.transform.lossyScale.x);
        StartCoroutine(MoveToPos(center, newCenter, cameraOffset, newOffset));
    }


    IEnumerator MoveToPos(Vector3 baseCenterPos, Vector3 newCenterPos, Vector3 baseOffset, Vector3 newOffset)
    {
        for(float i = Mathf.PI; i >= 0; i -= moveToPosSpeed / 20 * Time.deltaTime * 100)
        {
            center = Vector3.Lerp(baseCenterPos, newCenterPos, Mathf.Cos(i) / 2 + 0.5f);
            cameraOffset = Vector3.Lerp(baseOffset, newOffset, Mathf.Cos(i) / 2 + 0.5f);
            yield return null;
        }
    }

    private void CalculateNewOffset(float targetLenght)
    {
        newOffset = (transform.position - newCenter) / Vector3.Magnitude(transform.position - newCenter) * targetLenght;

        //if the center stay the same, it cause the distance to be 0, leading to an error when applying the offset to the camera 
        if(newCenter == center) newOffset = cameraOffset / cameraOffset.magnitude * targetLenght;
    }
}
