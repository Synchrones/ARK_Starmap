using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO: fix various bugs
public class CameraScript : MonoBehaviour
{


    public Transform PlayerTransform;
    private Vector3 cameraOffset;
    public Vector3 center;

    private bool completed;
    private Vector3 newPos;
    private Vector3 newPosZoom;
    private float scroll;
    private Vector3 newCenter;
    private bool centered;
    private bool arrived;
    private int stopDistance;

    private float speed;
    private float zoomSpeed;
    private float rotateSpeed;
    private float rotateTime;
    private float camMoveSpeed;

    private Vector3 velocity = Vector3.zero;

    private bool InSystem;
    public bool COSelected;

    private float maxZoom;
    private float minZoom;

    //inertia things
    private Vector3 prevPos = Vector3.zero;
    private Vector3 frameVelocity;
    private Vector3 curVelocity;
    private bool underInertia;
    private bool isRotation;
    private float smoothTime = 3;
    private float time = 0.0f;

    
    //UI
    public GameObject UIContainer;
    private bool buttonPressed;
    //Sounds
    AudioManagerScript AudioManager;
    bool isZoomSoundPlaying;
    bool isRotateSoundPlaying;
    

    // Start is called before the first frame update
    void Start()
    {
        newPosZoom = transform.position;
        scroll = 0;
        completed = true;
        center = PlayerTransform.position;
        maxZoom = 750;
        minZoom = 20;
        speed = 200;
        zoomSpeed = 50;
        rotateSpeed = 0.17f;
        rotateTime = 0;
        InSystem = false;
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        buttonPressed = false;
    }

    private void LateUpdate() {
        bool hitUI = false;
        if(!buttonPressed)
        {   
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            foreach(var result in raycastResults)
            {
                if(result.gameObject.layer == 5) hitUI = true; 
            }
        }
        
        
        if(Input.GetAxis("Mouse ScrollWheel") != 0 && !COSelected)
        {
            underInertia = false;
            time = 0.0f;

            scroll = Input.GetAxis("Mouse ScrollWheel");
            if(Vector3.Distance(transform.position + transform.forward * scroll * zoomSpeed, center) > minZoom && scroll > 0 || Vector3.Distance(transform.position + transform.forward * scroll * zoomSpeed, center) < maxZoom && scroll < 0)
            {
                newPosZoom += transform.forward * scroll * zoomSpeed;
            } 
            if(!isZoomSoundPlaying)
            {
                AudioManager.play("CamZoom");
                isZoomSoundPlaying = true;
            }
            if(isRotateSoundPlaying)
            {
                AudioManager.stop("CamRotate");
                isRotateSoundPlaying = false;
            }
        }
        
        if(transform.position != newPosZoom && !underInertia && !Input.GetKey(KeyCode.Mouse1) && !Input.GetKey(KeyCode.Mouse0) && (Vector3.Distance(transform.position, center) > minZoom / 100 || scroll < 0))
        {
            transform.position = Vector3.SmoothDamp(transform.position, newPosZoom, ref velocity, 0.3f);
        }
        if(Vector3.Distance(transform.position, newPosZoom) < 0.1f)
        {
            if(isZoomSoundPlaying)
            {
                AudioManager.stop("CamZoom");
                isZoomSoundPlaying = false;
            }
            
        }
        
        
        
        cameraOffset = transform.position - center;
    
        if(Input.GetKey(KeyCode.Mouse1) && completed == true && !hitUI)
        {
            
            underInertia = false;
            time = 0.0f;

            buttonPressed = true;


            float distance;
            if(InSystem)
            {
                distance = Vector3.Distance(transform.position, center) / 2;
                distance = Mathf.Clamp(distance, 0, 3);
            }
            else
            {
                distance = 1;
            }


            Vector3 newPos = (transform.right * Input.GetAxis("Mouse X") * - speed * distance);
            newPos += (transform.up * Input.GetAxis("Mouse Y") * - speed * distance);
            
            transform.Translate(newPos * Time.deltaTime, Space.World);
            center = transform.position - cameraOffset;
            transform.LookAt(center);
            
            curVelocity = (transform.position - prevPos) / Time.deltaTime;
            frameVelocity = Vector3.Lerp(frameVelocity, curVelocity, 0.1f) / 1.5f;
            prevPos = transform.position;
            
        }

        


        if(Input.GetKey(KeyCode.Mouse0) && completed == true && !hitUI)
        {
            underInertia = false;
            time = 0.0f;

            buttonPressed = true;
            
            newPosZoom = transform.position;

            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * 5.0f * rotateSpeed, Vector3.up);
            double signX = cameraOffset.x;
            double signZ = cameraOffset.z;
            
            double cameraMaxY = Vector3.Distance(Vector3.zero, cameraOffset) * 95 / 100;
            double cameraMinY = -cameraMaxY;
            
            if(cameraOffset.y > 0 && ( cameraOffset.y < cameraMaxY || Input.GetAxis("Mouse Y") > 0) || cameraOffset.y < 0 && (cameraOffset.y > cameraMinY || Input.GetAxis("Mouse Y") < 0)) //prevent the camera from going to far up and down (caused jitter)
            {
                //bug on the z axis without this
                if(signZ * signX < 0) 
                {
                    if(signZ > 0)
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.back);
                    }
                    else
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.back);
                    }
                    
                    
                }
                else 
                {
                    if(signZ > 0)
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * 5.0f * rotateSpeed, Vector3.back);
                    }
                    else 
                    {
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.right);
                        camTurnAngle *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -5.0f * rotateSpeed, Vector3.back);
                    }
                    
                }
                
            }
            cameraOffset = camTurnAngle * cameraOffset;
            
            Vector3 newPos = center + cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, 0.5f);
            transform.LookAt(center);
            
            curVelocity = (transform.position - prevPos) /Time.deltaTime;
            frameVelocity = Vector3.Lerp(frameVelocity, curVelocity, 0.1f) / 1.5f;
            prevPos = transform.position;
            if(Input.GetAxis("Mouse X") > 0 && frameVelocity.x > 0 || Input.GetAxis("Mouse X") < 0 && frameVelocity.x < 0) frameVelocity.x = -frameVelocity.x;
            if(Input.GetAxis("Mouse X") > 0 && frameVelocity.z > 0 || Input.GetAxis("Mouse X") < 0 && frameVelocity.z < 0) frameVelocity.z = -frameVelocity.z;

            
            if(!isRotateSoundPlaying)
            {
                AudioManager.play("CamRotate");
                isRotateSoundPlaying = true;
            }
        }

        if(underInertia && time < 1)
        {
            if(isRotation)
            {    
                double cameraMaxY = Vector3.Distance(Vector3.zero, cameraOffset) * 0.95f;
                double cameraMinY = -cameraMaxY;

                if(cameraOffset.y > 0 && ( cameraOffset.y < cameraMaxY || -frameVelocity.y > 0) || cameraOffset.y < 0 && (cameraOffset.y > cameraMinY || -frameVelocity.y < 0))
                {
                    float distance = Vector3.Distance(Vector3.zero, cameraOffset);
                    transform.position = center;
                    transform.Rotate(Vector3.right, frameVelocity.y / 1.8f / (distance / 5));
                    transform.Rotate(Vector3.up, -(frameVelocity.x + frameVelocity.z) / 1.8f / (distance / 5), Space.World);
                    transform.Translate(new Vector3(0,0, -distance));
                    
                }
                
                float t = time / smoothTime;
                t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                
                frameVelocity = Vector3.Lerp(frameVelocity, Vector3.zero, t);
                time += Time.smoothDeltaTime / 2;               

            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + (frameVelocity / 2), 0.1f);
                center = Vector3.Lerp(center, center + (frameVelocity / 2), 0.1f);

                float t = time / smoothTime;
                t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                frameVelocity = Vector3.Lerp(frameVelocity, Vector3.zero, t);
                time += Time.smoothDeltaTime;
                
            }
            newPosZoom = transform.position;
            if(time > 0.4f)
            {
                if(isRotateSoundPlaying)
                {
                    AudioManager.stop("CamRotate");
                    isRotateSoundPlaying = false;   
                }
            }
        }
        else
        {
            underInertia = false;
            time = 0.0f;
        }


        if(!completed)
        {
            if(Vector3.Distance(transform.position, newPos) < stopDistance) arrived = true;
            MooveToPos();
            if(centered && arrived)
            {
                completed = true;
                center = newCenter;
                rotateTime = 0;
            } 
            
        }
        

        if(Input.GetMouseButtonUp(1))
        {
            underInertia = true;
            isRotation = false;

            buttonPressed = false;
        }
        if(Input.GetMouseButtonUp(0))
        {
            underInertia = true;
            isRotation = true;

            buttonPressed = false;
        }

    }
    

    public void SelectSystem(GameObject gameObject)
    {
        centered = false;
        arrived = false;
        completed = false;

        newPos = gameObject.transform.position;
        newCenter = newPos;
        
        camMoveSpeed = Vector3.Distance(transform.position, newPos) * 2;
        stopDistance = 75;
        
        
    }
    public void EnterSystem(GameObject gameObject)
    {
        underInertia = false;
        time = 0;

        newPosZoom = transform.position;

        centered = false;
        arrived = false;
        completed = false;

        InSystem = true;
        
        float zOffset = transform.position.z - gameObject.transform.position.z;
        newPos = gameObject.transform.position + new Vector3(0, 5, 5 * Mathf.Clamp(zOffset, -1, 1));
        newCenter = gameObject.transform.position;

        camMoveSpeed = Vector3.Distance(transform.position, newPos) * 2;
        speed = 5;
        zoomSpeed = 2;
        maxZoom = 40;
        minZoom = 0.05f;
        stopDistance = 5;
        
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<AppaerenceAndSizeKeeper>().enabled = false;
        
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);


    }

    public void ExitSystem(GameObject gameObject)
    {
        underInertia = false;
        time = 0;

        newPosZoom = transform.position;

        centered = false;
        arrived = false;
        completed = false;

        InSystem = false;

        newPos = transform.TransformPoint(0, 0, -50);
        newCenter = gameObject.transform.position;

        camMoveSpeed = Vector3.Distance(transform.position, newPos) * 2;
        maxZoom = 750;
        minZoom = 20;
        speed = 200;
        zoomSpeed = 50;
        stopDistance = 1;

        GameObject.Destroy(gameObject.transform.GetChild(2).gameObject);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<AppaerenceAndSizeKeeper>().enabled = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void MoveToCO(GameObject gameObject)
    {
        underInertia = false;
        time = 0;

        newPosZoom = transform.position;

        centered = false;
        arrived = false;
        completed = false;

        COSelected = true;

        stopDistance = 2;
        newPos = gameObject.transform.position;
        newCenter = newPos;

        camMoveSpeed = Vector3.Distance(transform.position, gameObject.transform.position) * 2;

        stopDistance = 1;

        
    }
    
    public void MooveToPos()
    {
        if(!arrived)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, camMoveSpeed * Time.deltaTime);

        }
            
        if(!centered)
        {
            float t = rotateTime / smoothTime;
            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

            var newPosRotation = Quaternion.LookRotation(newCenter - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, newPosRotation, t);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, newPosRotation, t); might be better, IDK
            rotateTime += Time.smoothDeltaTime;

            if(newPosRotation == transform.rotation) centered = true;
        }
        else
        {
            transform.LookAt(newCenter);
        }
           
    }


}
