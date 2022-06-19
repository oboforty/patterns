using UnityEngine;
using UnityEngine.UI;



    public class InventorySlot : MonoBehaviour
    {
        public Image icon;
        public bool isEmpty => item == null;

        public BlockConfig item { get; private set; }

        Image img;

        public Color SelectedColor = Color.yellow;

        private void Awake()
        {
            TryGetComponent(out img);
            img.color = Color.white;
        }

        public void SetItem(BlockConfig b)
        {
            item = b;

            if (item != null)
            {
                icon.sprite = item.icon;
                icon.enabled = true;
            }
            else
            {
                icon.enabled = false;
            }
        }

        public void SwitchWith(InventorySlot other)
        {
            // Switch
            var sol = other.item;

            other.SetItem(item);
            SetItem(sol);
        }

        public void SetSelected(bool sel)
        {
            img.color = sel ? SelectedColor : Color.white;
        }

        public override string ToString()
        {
            return isEmpty ? "?" : item.ToString();
        }
    }
