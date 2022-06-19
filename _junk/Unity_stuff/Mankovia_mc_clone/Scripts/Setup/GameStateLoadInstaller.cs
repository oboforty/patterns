using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Setup
{
    class GameStateLoadInstaller : MonoBehaviour
    {
        public Chonk chonk;
        public Inventory inventory;
        public MinionFactory minions;

        [Space(12)]
        public bool loadOnAwake = true;
        public bool saveGameTask = true;
        public int saveInterval = 30;

        /// External GameEvent
        public void __Install()
        {
            if (loadOnAwake)
            {
                LoadGame();
            }

            // @TODO: 
        }

        private void LoadGame()
        {

        }

#if UNITY_EDITOR
        [ContextMenu("Load Game")]
        void __ctxLoad()
        {
            chonk.Clear();
        }

        [ContextMenu("Save Game")]
        void __ctxSave()
        {
            chonk.Clear();
        }
# endif
    }
}
