using System;
using System.Collections.Generic;
using UnityEngine;

    public class Inventory : MonoBehaviour
    {
        public Chest chest = null;

        List<InventorySlot> items;
        InventorySlot selected = null;

        [Space(12)]
        public AudioClip invSound;
        public AudioClip errorSound;
        public float volume = 0.78f;

        public BlockConfig Selected => selected == null ? null : selected.item;

        [Inject]
        SoundManager sound;

        private void Awake()
        {
            items = new List<InventorySlot>();

            for(int i = 0; i < transform.childCount; i++)
                items.Add(transform.GetChild(i).GetComponent<InventorySlot>());
        }

        public void OnItemClicked(InventorySlot slot)
        {
            if (selected != null)
            {
                bool sameSelected = selected == slot;

                // deselect previous selection
                selected.SetSelected(false);
                selected = null;

                // double tap => deselect item
                if (sameSelected)
                    return;
            }

            if (chest != null)
            {
                if (slot.isEmpty)
                    return;

                // chest open: transfer from inv to chest
                if (chest.AddItem(slot.item))
                {
                    slot.SetItem(null);

                    // SFX
                    sound.Play(invSound, volume);
                }
                else
                {
                    // error SFX
                    sound.Play(errorSound, volume);
                }
            }
            else if (false)
            {
                // crafting open

                // @TODO
            }
            else
            {
                // nothing is open, prepare for building
                selected = slot;
                selected.SetSelected(true);

                // SFX
                sound.Play(invSound, 0.34f);
            }
        }

        /**
         * Adds item to inventory
         * returns true if has empty slot
         */
        public bool AddItem(BlockConfig Item)
        {
            foreach (var item in items)
                if (item.isEmpty)
                {
                    item.SetItem(Item);
                    return true;
                }

            // item slot not found
            return false;
        }

        /**
         * Remove item from inventory
         */
        public void RemoveSelected()
        {
            selected.SetItem(null);

            selected.SetSelected(false);
            selected = null;
        }

        /**
         * Remove item from inventory
         */
        public void Remove(BlockConfig selected)
        {
            throw new NotImplementedException();
        }

    }