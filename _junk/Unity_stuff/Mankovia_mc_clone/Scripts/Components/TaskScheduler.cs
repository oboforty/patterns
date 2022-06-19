using System.Collections.Generic;
using UnityEngine;

public class TaskScheduler : MonoBehaviour
{
    public List<Minion> freeMinions;
    public Inventory inventory;

    [Space(12)]
    public AudioClip selectSound;

    private List<Block> m_ToBeMined;
    private List<Block> m_ToBeBuilt;
    private Chonk chonk;


    private void Start()
    {
        m_ToBeMined = new List<Block>();
        TryGetComponent(out chonk);
    }

    /**
     * Toggles block to be mined
     */
    public bool ToggleBlock(Block block, Vector3Int whichSide)
    {
        // @TEMP: immediate mine

        // Removes a block, adding it to inventory

        // @TODO: VFX & SFX
        if (inventory.Selected != null)
        {
            // block selected, place new block

            // @TODO: ITT: how to find which side has been clicked?

            // @TEMPORAL - spawn immediately
            // the block above selected is the place to be spawned
            var newPos = block.Coord + whichSide;

            var newBlock = chonk.Spawn(inventory.Selected, newPos);

            if (newBlock != null)
            {
                inventory.RemoveSelected();

                return true;
            }

            // @error vfx ??
            return false;
        }
        else
        {
            // select block on Chonk => toggle mine task

            if (m_ToBeMined.Contains(block))
                m_ToBeMined.Remove(block);
            else
                m_ToBeMined.Add(block);


            // @TEMPORAL - mine immediately
            if (inventory.AddItem(block.block))
            {
                chonk.Remove(block);
                return true;
            }

            // can't mine out because inventory is full!
            // @todo: error VFX & SFX
            return false;
        }
    }

    //Block m_Selected;
    //public void IDK ()
    //{

    //    if (m_Selected != null)
    //    {
    //        var target = chonk.FindFreeSpace(block._Coordinate);

    //        if (target.y <= -9999)
    //        {
    //            Debug.LogError($"Free space not found next to {block._Coordinate}");
    //            return false;
    //        }

    //        Debug.Log(chonk.FindPathAStar(m_Selected._Coordinate, target));

    //        m_Selected = null;
    //    }
    //    else
    //    {
    //        Debug.Log(block.block.name, block.gameObject);

    //        m_Selected = block;
    //    }

    //}
}
