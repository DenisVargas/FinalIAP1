using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour {
    public GameObject operativeCamera;
    public Vector3 targetLock;
    public bool freeCamera = true;
    public bool lockToTargets = false;

    [SerializeField] private float CameraSpanVelocity;
    [SerializeField] private float panBorderThickness;
    public Vector2 panLimits;

    public float scrollSpeed = 20f;
    public float rotationSpeed = 20f;
    public Vector3 ZoomMin, ZoomMax;

    public float zoomSpeed;
    public float ZoomDist;
    // Update is called once per frame

    private void Awake()
    {
        ZoomMax = new Vector3(operativeCamera.transform.localPosition.x,operativeCamera.transform.localPosition.y,operativeCamera.transform.localPosition.z);
        var BNormal = operativeCamera.transform.forward;
        ZoomMin = ZoomMax + BNormal * ZoomDist;
    }
    void Update () {
        if (Input.GetKey(KeyCode.Space))
            lockToTargets = true;
        if (Input.GetKeyUp(KeyCode.Space))
            lockToTargets = false;

        #region Movimiento

        if (freeCamera)
        {
            if (!lockToTargets)
            {
                // Input en Y
                if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
                    transform.position += transform.forward * CameraSpanVelocity * Time.deltaTime;

                if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
                    transform.position += -transform.forward * CameraSpanVelocity * Time.deltaTime;

                // Input en X
                if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
                    transform.position += transform.right * CameraSpanVelocity * Time.deltaTime;

                if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
                    transform.position -= transform.right * CameraSpanVelocity * Time.deltaTime;
                //La z es mi valor forward, mientras que mi x es mi right.
                var xpos = transform.position.x;
                xpos = Mathf.Clamp(xpos, -panLimits.x, panLimits.x);
                var zPos = transform.position.z;
                zPos = Mathf.Clamp(zPos, -panLimits.y, panLimits.y);

                transform.position = new Vector3(xpos, transform.position.y, zPos);
            }
            else
            {
                var xpos = targetLock.x;
                xpos = Mathf.Clamp(xpos, -panLimits.x, panLimits.x);
                var zPos = targetLock.z;
                zPos = Mathf.Clamp(zPos, -panLimits.y, panLimits.y);
                transform.position = new Vector3(xpos, transform.position.y, zPos);
            }
            
        }
        #endregion

        #region Zoom
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            operativeCamera.transform.localPosition = Vector3.Lerp(operativeCamera.transform.localPosition,ZoomMin,(zoomSpeed * 100f *Time.deltaTime) / 100f);
        }
        else if (scroll < 0)
        {
            operativeCamera.transform.localPosition = Vector3.Lerp(operativeCamera.transform.localPosition, ZoomMax, (zoomSpeed * 100f * Time.deltaTime) / 100f);
        }
        
        
        #endregion

        #region Rotacion
        if (Input.GetKey("e"))
        {
            //Usamos quaterniones, pero traducidos con eulerAngles, de otro modo es imposible trabajar las rotaciones con vectores tridimencionales.
            Quaternion A = transform.rotation; //Valor por defecto.
            Quaternion B = transform.rotation; //Valor que será modificado(necesario para tener los valores orig).
            B = Quaternion.Euler(B.eulerAngles.x,B.eulerAngles.y + rotationSpeed,B.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(A,B,rotationSpeed * Time.deltaTime);
            
        }
        
        if (Input.GetKey("q"))
        {
            Quaternion A = transform.rotation;
            Quaternion B = transform.rotation;
            B = Quaternion.Euler(B.eulerAngles.x, B.eulerAngles.y - rotationSpeed, B.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(A, B, rotationSpeed * Time.deltaTime);
        }
        #endregion
        
    }
}
