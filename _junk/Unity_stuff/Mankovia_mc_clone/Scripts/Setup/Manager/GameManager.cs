using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace Core.Manager
{
    public static class GameManager
    {
        static bool Redirect;
        
        static string[] BuildScenes = new string[]{
           
        };

        public static string currentScene => SceneManager.GetActiveScene().name;

        public static void Init(bool shallRedirect)
        {
            Redirect = shallRedirect;
        }

        public static void LoadScene(string scene)
        {
            // Prevent redirection with development scenes
            if (!BuildScenes.Contains(currentScene))
                return;

            if (!Redirect)
                scene = "ValidateLoadScene";

            if (currentScene != scene)
            {
                if (Debug.isDebugBuild)
                    Debug.Log($"<color=white>[GameManager] Switching to {scene}");

                SceneManager.LoadScene(scene);
                //else if (Debug.isDebugBuild)
                //    Debug.Log($"[GameManager] Scene routing disabled (-->{scene} requested)");
            }
        }
    }
}
