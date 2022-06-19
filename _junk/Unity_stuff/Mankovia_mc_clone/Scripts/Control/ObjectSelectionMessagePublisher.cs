using UnityEngine;
using UnityEngine.EventSystems;




namespace Core.Control
{
    public enum TouchVariety { SINGLE, DOUBLE, LONG, NOTOUCH }


    public class ObjectSelectionMessagePublisher : MonoBehaviour
    {
        /**
         * Handles Selection within game 
         * Selection is forwarded as GameMessage
         */

        [Header("Selection Events")]
        public float touchTimeout = 0.3f;

        public TaskScheduler task;
        public MinionFactory factory;

        Touch? currentTouch = null;
        float TouchStartTime = 0;        // start of first touch
        bool TouchHasEnded = false;


        // Raycast max distance:
        [Header("Raycasting")]
        public float maxRayDist = 2500f;
        public string[] selectableLayers = { 
            "block"
        };
        int selectableMask;
        Camera cam;

        [Inject]
        DeviceInput input = null;
        EventSystem evtsys;

        void Start()
        {
            cam = Camera.main;
            evtsys = EventSystem.current;
            selectableMask = LayerMask.GetMask(selectableLayers);

            currentTouch = null;
            TouchHasEnded = false;

            if (input == null)
                Debug.LogError("[ObjSelMsgPub] input = null");
        }

        private void HandleTouch(Touch touch, TouchVariety touchType)
        {
            /**
             * Single touch selects game objects.
             * 
             * This means showing information about game object
             * and respective game data in a toolbar.
             **/

            Ray ray = cam.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDist, selectableMask))
            {
                var layer = LayerMask.LayerToName(hit.transform.gameObject.layer);

                switch (layer)
                {
                    case "block":
                        if (hit.transform.TryGetComponent(out Block block))
                        {
                            if (block.spawnPrefab != null)
                                factory.Create(block);
                            else
                            {
                                var side = hit.normal.RoundToInt();
                                if (side.y != 0)
                                    side.y = -side.y;
                                task.ToggleBlock(block, side);
                            }
                        }
                        break;
                }
            }
            else
            {
                if (Physics.Raycast(ray, out hit, maxRayDist)) {
                    Debug.Log("ObjectSelection: WTF?? " + hit.transform.name);

                }
                // todo: deselect
            }
        }

        void Update()
        {
            // prevent interacting with UI
            if (input == null || input.LockedSelection || input.IsOverUI())
                return;

            if (input.TouchCount() == 1)
            {
                // @TODO: how to lock camera efficiently?
                //input.LockedCamera = true;

                // Touch event => single, double, long tap
                var touch = input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    // Touch begin:
                    currentTouch = touch;
                    TouchStartTime = Time.time;
                    TouchHasEnded = false;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    // Touch ends:
                    TouchHasEnded = true;
                }
            }

            if (currentTouch != null && Time.time - TouchStartTime >= touchTimeout)
            {
                var passTouch = currentTouch;

                // End touch -- reset
                currentTouch = null;
                TouchHasEnded = false;
                TouchStartTime = 0;
                //input.LockedCamera = false;

                if (TouchHasEnded)
                {
                    // Single Touch := if we started touch before, and it ended before the timeout
                    HandleTouch((Touch)passTouch, TouchVariety.SINGLE);
                    // @TODO: later: handle double tap?
                }
                else
                {
                    // Long Touch := timed out before end of finger Touch
                    HandleTouch((Touch) passTouch, TouchVariety.LONG);
                }

            }
        }
   }
}
