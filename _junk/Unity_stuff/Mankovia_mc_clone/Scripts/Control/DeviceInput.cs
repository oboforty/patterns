using UnityEngine;
using UnityEngine.EventSystems;

public class DeviceInput
{
    public bool LockedCamera { get; set; }
    public bool LockedSelection { get; set; }
    public bool LockedDragging { get; set; }

    EventSystem evSys;

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
    protected const bool isMobile = true;

    public int TouchCount()
    {
        return Input.touchCount;
    }

    public Touch GetTouch(int i)
    {
        return Input.GetTouch(i);
    }
#elif UNITY_EDITOR || UNITY_STANDALONE
    protected const bool isMobile = false;

    [Header("Editor-only settings:")]
    public float MouseSensitivity = 2f;
    public float PanVsTouchSensitivity = 0.2f;

    // Right mouse button: simulates 2 fingers with shift key
    Touch touch1;
    Touch touch2;
    byte mouseTouchSimCount = 0;
    bool released = true;

    public DeviceInput()
    {
        mouseTouchSimCount = 0;
        released = true;
        
    }

     void SimulateTouchesWithMouse()
     {
        int i;

        // Reset after mouse btn relase
        if (released)
        {
            mouseTouchSimCount = 0;
            touch1 = new Touch();
            touch1.phase = TouchPhase.Canceled;
            touch2 = new Touch();
            touch2.phase = TouchPhase.Canceled;

            released = false;
        }

        ref Touch touch = ref touch1;

        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            mouseTouchSimCount = 0; // reset so that left click doesn't interact with right click

            // Left click
            //touch = ref touch1;
            mouseTouchSimCount |= 1;
            i = 0;
        }
        else if (Input.GetMouseButton(1) || Input.GetMouseButtonUp(1))
        {
            // Right click - simulate 2 fingers
            // Shift down -> right
            if (Input.GetKey(KeyCode.LeftShift))
            {
                touch = ref touch2;
                mouseTouchSimCount |= 2;
            }
            else
            {
                //touch = ref touch1;
                mouseTouchSimCount |= 1;
            }

            i = 1;
        }
        else return;

        touch.fingerId = i;
        touch.deltaPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * MouseSensitivity;
        touch.position = Input.mousePosition;
        touch.tapCount = 1; // @TODO

        // simulate 3 Touch phases
        if (Input.GetMouseButtonUp(i))
        {
            touch.phase = TouchPhase.Ended;
            released = i == 0; // reset only left mouse key
        }
        else if (Input.GetMouseButtonDown(i))
        {
            touch.phase = TouchPhase.Began;
        }
        else if (Input.GetMouseButton(i))
        {
            if (touch.deltaPosition.magnitude > PanVsTouchSensitivity)
                touch.phase = TouchPhase.Moved;
            else
                touch.phase = TouchPhase.Stationary;
        }

        // previous mouse mosition:
        touch.rawPosition = Input.mousePosition;
    }

    public int TouchCount()
    {
        SimulateTouchesWithMouse();

        int n = 0;

        for (byte i = 0; i < 8; i++)
            if (((mouseTouchSimCount >> i) & 1) == 1)
                n++;

        return n;
    }

    public Touch GetTouch(int i)
    {
        if (i == 0 && (mouseTouchSimCount & 1) == 1)
            return touch1;

        else if (i == 1 && (mouseTouchSimCount & 2) == 2)
            return touch2;

        return new Touch();
    }

    public bool IsOverUI()
    {
        if (evSys == null)
            evSys = EventSystem.current;

        return evSys.IsPointerOverGameObject();
    }
#endif
}
