using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript   : MonoBehaviour
{


    public Transform PlayerTransform;
    private Vector3 _cameraOffset;
    private Vector3 center;
    

    private float speed = 3;
    private float zoomSpeed = 5;
    private float rotateSpeed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        center = PlayerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _cameraOffset = transform.position - center;
        float scroll = Input.GetAxis ("Mouse ScrollWheel");
        transform.Translate(0, 0, scroll * zoomSpeed, Space.Self);

    }
    
    private void LateUpdate() {
        if(Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 newPos = (transform.right * Input.GetAxis("Mouse X") / -10 / speed);
            newPos += (transform.up * Input.GetAxis("Mouse Y") / -10 / speed);
            transform.position += newPos;
            center += newPos;
            transform.LookAt(center);
            
        }


        if(Input.GetKey(KeyCode.Mouse0))
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * 5.0f, Vector3.up);
            double signX = _cameraOffset.x;
            double signZ = _cameraOffset.z;
            
            double cameraMaxY = Vector3.Distance(new Vector3(0,0,0), _cameraOffset) - 0.5;
            double cameraMinY = -cameraMaxY;
            
            if(_cameraOffset.y > 0 && ( _cameraOffset.y < cameraMaxY || Input.GetAxis("Mouse Y") > 0) || _cameraOffset.y < 0 && (_cameraOffset.y > cameraMinY || Input.GetAxis("Mouse Y") < 0)) //prevent the camera from going to far up and down (caused jitter)
            {
                //buggy on the z axis without this
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
            _cameraOffset = camTurnAngle * _cameraOffset;
            /*Debug.Log(_cameraOffset);
            Debug.Log(cameraMaxY);
            Debug.Log(cameraMinY);*/
            Vector3 newPos = center + _cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, 0.5f);
            transform.LookAt(center);
        }

    }

}
