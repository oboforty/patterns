using System.Collections.Generic;

using UnityEngine;
using TMPro;
using Assets.Scripts.Objects;
using System;

    public class Chest : MonoBehaviour
    {
        public Inventory inventory;
        public TextMeshProUGUI chestIndicator;
        public GameObject backdrop;

        [Space(12)]
        public AudioClip openSound;
        public AudioClip invSound;
        public AudioClip errorSound;
        public float volume = 0.78f;

        // chests on map:
        List<ChestBlock> chests;
        List<InventorySlot> items = null;
        int opened = 0;

        int layerBlock;
        int layerHighlight;

        public static int SIZE = 14;

        [Inject]
        SoundManager sound;

        private void Awake()
        {
            layerBlock = LayerMask.NameToLayer("block");
            layerHighlight = LayerMask.NameToLayer("block_highlight");

            // 2 rows of items, parse both
            var tc1 = transform.Find("Row1");
            var tc2 = transform.Find("Row2");
            SIZE = tc1.childCount + tc2.childCount;
            items = new List<InventorySlot>(SIZE);
            for (int i = 0; i < tc1.childCount; i++)
                items.Add(tc1.GetChild(i).GetComponent<InventorySlot>());
            for (int i = 0; i < tc2.childCount; i++)
                items.Add(tc2.GetChild(i).GetComponent<InventorySlot>());

            // init empty list of chests
            if (chests == null)
                chests = new List<ChestBlock>();
            chestIndicator.enabled = false;

            // Hide self
            Close();
        }
        
        /**
         * Adds item to chest
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
         * Move item from chest to inventory
         */
        public void OnItemClicked(InventorySlot slot)
        {
            if (slot.isEmpty)
                return;

            if (inventory.AddItem(slot.item))
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

        /**
         * Switches chest content
         */
        public void OpenChest(int ch)
        {
            sound.Play(openSound, volume);

            if (opened < chests.Count)
            {
                // deselect previous chest
                chests[opened].gameObject.layer = layerBlock;
            }

            var chest = chests[ch];
            opened = ch;

            gameObject.SetActive(true);
            backdrop?.SetActive(true);

            Debug.Assert(chest.items.Count <= items.Count, $"Chest size does not equal GUI size #{ch}:   {chest.items.Count} != {items.Count}");

            // copy items
            for (int i = 0; i < chest.items.Count; i++)
                items[i].SetItem(chest.items[i]);

            // show chest ID
            chestIndicator.enabled = true;
            chestIndicator.text = $"Chest {ch}";
            chest.gameObject.layer = layerHighlight;

            // @TODO: ITT: show opened chest VFX - brackeys
        }

        /// Button event
        public void Close()
        {
            //opened = 0;
            chests[opened].gameObject.layer = layerBlock;

            gameObject.SetActive(false);
            backdrop?.SetActive(false);
        }

        /**
         * Shifts between different chests in cyclic order
         */
        public void ToggleChests()
        {
            // it's need to open here so that Awake is called
            //if (chests == null)
            //    gameObject.SetActive(true);
            if (chests.Count == 0)
            {
                Debug.LogWarning("No chests found!");
                // nope
                return;
            }

            if (++opened >= chests.Count)
                opened = 0;

            OpenChest(opened);
        }

        /**
         * Adds chest block
         */
        public int RegisterChest(ChestBlock chestBlock)
        {
            // call awake, which initializes chests
            //if (chests == null)
            //    gameObject.SetActive(true);

            chests.Add(chestBlock);
            return chests.Count - 1;
        }
    }
