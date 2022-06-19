using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class ChestBlock : MonoBehaviour
    {
        public Chest chestUI;

        [Space(12)]
        public List<BlockConfig> items;

        int ch;

        private void Awake()
        {
            if (items == null)
                items = new List<BlockConfig>();

            ch = chestUI.RegisterChest(this);
        }


        // @TODO: later: click
    }
}
