using System;

using UnityEngine;

namespace Setup
{
    public class SceneInstaller : MonoBehaviour
    {
        public Chest chestUI;

        [Inject]
        public SoundManager sound = null;

        void Awake()
        {
            DIInjector.Populate(this);

            // Populate rest of DI objects:
            var monos = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            DIInjector.PopulateAll(monos);

            // enable UI because it shuts itself down
            if (!chestUI.gameObject.activeSelf)
                chestUI.gameObject.SetActive(true);

            if (TryGetComponent(out AudioSource source))
                sound.Source = source;


            SendMessage("__Install", SendMessageOptions.DontRequireReceiver);

            Destroy(this);
        }
    }
}
