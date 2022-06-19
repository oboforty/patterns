using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    /**
     * Script that adds mobile-friendly Camera
     */

    public Camera m_Camera;
    public Collider Ground;
    public DeviceInput input;

    public Text text;

    [Header("Use left shift to simulate 2 finger control.")]
    public bool AllowCamMovements = true;

    [Space(10)]
    public float PanSpeed = 4f;
    public float PanDampening = 6f;

    [Space(10)]
    public float MinZoom = 1f;
    public float MaxZoom = 9f;
    public float ZoomSpeed = 2f;

    [Space(10)]
    public float RotateSpeed = 1f;
    public bool RotateAxisUp = true;

    [Space(10)]
    public float touchTimeout = 0.3f;

    // Raycast max distance:
    public float MaxRayDist = 2500f;

    bool m_EnableCamera = true;
    public bool EnableCamera {
        get {
            return m_EnableCamera && AllowCamMovements && !EventSystem.current.IsPointerOverGameObject() && !input.LockedCamera;        
        } 
            
        set => m_EnableCamera = value; 
    }
        
    protected Transform m_CamTrans;
    protected Transform m_ParentTrans;

    float initZoomDistance = 0f;
    float initCamDistance;


    void Awake()
    {
        if (m_Camera != null)
            this.m_CamTrans = m_Camera.transform;
        else
            this.m_CamTrans = Camera.main.transform;

        this.m_ParentTrans = m_CamTrans.parent;


        //Input.simulateMouseWithTouches = true;

        EnableCamera = true;
    }

    // start of first touch
    //float? T0 = null;
    // is the current Touch-chain a drag event?
    //bool isDrag;
    // is it a dragging event, but for the camera?
    //bool isDragCamera;
    // is the current Touch-chain a long touch event?
    //bool isLong;
    // this is set by the camera script
    //bool locked;

    // int NMoves = 0;

    void Update()
    {
        // prevent interacting with UI
        if (!EnableCamera)
            return;

        if (input.TouchCount() >= 2)
        {
            // Pinch & strech => rotate and zoom with 2 fingers
            var touch0 = input.GetTouch(0);
            var touch1 = input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch0.phase == TouchPhase.Began)
            {
                // Record initial distance between 2 fingers and camera

                initZoomDistance = Vector2.Distance(touch0.position, touch1.position);
                initCamDistance = m_CamTrans.localPosition.z;
            }

            // rotate camera
            if (touch1.phase == TouchPhase.Moved || touch0.phase == TouchPhase.Moved)
                RotateAndZoomCamera(touch0.position, touch0.deltaPosition, touch1.position, touch1.deltaPosition);
        }
        else if (input.TouchCount() == 1)
        {
            // Touch event => single, double, long tap
            var touch = input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended && touch.phase == TouchPhase.Stationary)
                return;

            PanCamera(touch.position, touch.deltaPosition);
        }
    }

    void PanCamera(Vector2 ScreenPos, Vector2 ScreenDelta)
    {
        // count delta
        if (
            Raycast(ScreenPos, out Vector3 pointNow) &&
            Raycast(ScreenPos - ScreenDelta, out Vector3 pointBefore)
        )
        {
            m_LocalPanning = (pointBefore - pointNow).normalized;

            // planar translation:
            m_LocalPanning.y = 0;
        }
        else
        {
            m_LocalPanning = Vector3.zero;
        }
    }

    public bool Raycast(Vector2 pos, out Vector3 point)
    {
        if (Ground.Raycast(m_Camera.ScreenPointToRay(pos), out RaycastHit hit, MaxRayDist))
        {
            point = hit.point;
            return true;
        }
        else
        {
            point = Vector3.zero;
            return false;
        }
    }

    void RotateAndZoomCamera(Vector2 scP0, Vector2 scD0, Vector2 scP1, Vector2 scD1)
    {
        // Get center point between 2 touches:
        Vector2 scCen = (scP1 + scP0) / 2;


        if (Ground.Raycast(m_Camera.ScreenPointToRay(scCen), out RaycastHit hit, MaxRayDist))
        {
            if (EnableCamera)
            {
                float zoom = Vector2.Distance(scP0, scP1) / initZoomDistance;

                if (zoom > 1.0f)
                    m_LocalZoom = Mathf.Lerp(initCamDistance, MaxZoom, (zoom - 1f) * ZoomSpeed);
                else if (zoom < 1.0f)
                    m_LocalZoom = Mathf.Lerp(initCamDistance, MinZoom, (1f - zoom) * ZoomSpeed);
            }

            // @TODO: left drag interferes with right drag

            if (EnableCamera)
            {
                // rotate := calculate angle between the 2 delta touch-pairs relative to the center point

                // previous touches. don't touch yourself!
                Vector2 scP0_ = scP0 - scD0;
                Vector2 scP1_ = scP1 - scD1;

                // center point and vector in world-space:
                Vector3 wCen = hit.point;
                Vector3 wRotateAxis = RotateAxisUp ? Vector3.up : hit.normal;
                //Vector3 wCam = m_Camera.transform.position;

                // angles are relative to the center point
                float angle0 = Vector2.SignedAngle(scP0 - scCen, scP0_ - scCen);
                float angle1 = Vector2.SignedAngle(scP1 - scCen, scP1_ - scCen);

                // overall rotation angle is the average of the two
                var angle = (angle0 + angle1) / 2;
                angle = -angle * RotateSpeed;

                m_ParentTrans.RotateAround(wCen, wRotateAxis, angle);
            }
        }
    }

    Vector3 m_LocalPanning;
    float m_LocalZoom;

    void LateUpdate()
    {
        // prevent interacting with UI
        if (!EnableCamera)
            return;

        if (m_LocalPanning.magnitude > 0.01f)
        {
            // Apply camera panning:
            m_ParentTrans.Translate(m_LocalPanning * PanSpeed, Space.World);

            // slow down
            if (PanDampening > 0)
                m_LocalPanning = Vector3.Lerp(m_LocalPanning, Vector3.zero, Time.deltaTime * PanDampening);
            else
                m_LocalPanning = Vector3.zero;
        }

        if (m_LocalZoom > 0.00001f)
        {
            // set Z vector of camera position
            var vp = m_CamTrans.localPosition;
            vp.z = m_LocalZoom;
            m_CamTrans.localPosition = vp;

            // reset
            m_LocalZoom = 0.0f;
        }
    }
}
