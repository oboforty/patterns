using UnityEngine;

using Core.Manager;
using Setup;

namespace Core.Setup
{
    public class GameInstaller : MonoBehaviour
    {

        public string locale = "EN";
        public bool RedirectEnabled = true;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Main()
        {
            var inst = Resources.Load<GameInstaller>("GameInstaller");

            if (Debug.isDebugBuild && inst is null)
            {
                Debug.LogError("[GameInstaller] couldn't find instance");
                return;
            }

            inst.InjectGlobals();

            //if (Debug.isDebugBuild)
            //    Debug.Log("<color=white>[GameInstaller] Setup done</color>");
        }

        private void InjectGlobals()
        {
            DIInjector.Bind(new DeviceInput());
            DIInjector.Bind(new SoundManager());

            //GameManager.Init(RedirectEnabled);
        }

        private void OnApplicationQuit()
        {
            Brexit();
        }

        bool isDisposed = false;
        void Brexit()
        {
            //if (!isDisposed)
            //{
            //    Debug.Log("<color=white>[GameInstaller] Quitting application</color>");

            //    if (server != null)
            //    {
            //        server.Dispose();
            //        server = null;
            //    }

            //    isDisposed = true;
            //}
        }
    }
}
