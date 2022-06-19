using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class Chonk : MonoBehaviour
{
    public GameObject blockPrefab;
    public BiomeConfig biome;

    public HashSet<Vector3Int> freeSpaces { get; private set; } = new HashSet<Vector3Int>();

    // @TODO: refactor these to a custom mono?

    private static readonly Vector3Int[] ndir = {
        Vector3Int.right,
        Vector3Int.left,
        new Vector3Int(0,0,1),
        new Vector3Int(0,0,-1),
        Vector3Int.down,
        Vector3Int.up,
    };

    Regex junkRegex = new Regex(@"^\[.?\]\s(Stone|Dirt)$");

    private void Awake()
    {
        freeSpaces = new HashSet<Vector3Int>();
    }

    private void Start()
    {
        
    }

    /**
     * Creates a new block
     */
    public Block Spawn(BlockConfig cfg, Vector3Int pos, bool handleSurface = true)
    {
        // outside of world
        if (!CheckValidCoordinate(pos))
            return null;

        var obj = Instantiate(blockPrefab, transform).GetComponent<Block>();
        obj.block = cfg;

        var block = obj.GetComponent<Block>();

        block.Coord = pos;
        block.name = $"{pos.x},{pos.y},{pos.z}";
        //block.name = $"block {block.block.name} ({pos.x},{pos.y},{pos.z})";
        block.transform.position = Grid2WorldPos(pos);

        // show surface block if it's uppermost
        if (!handleSurface)
            HandleSurfaceBlocks(block);

        // update renderer
        obj.UpdateBlockRenderer();

        return block;
    }

    /**
     * Removes a block
     */
    public void Remove(Block block)
    {
        var bBelow = transform.Find($"{block.Coord.x},{block.Coord.y + 1},{block.Coord.z}");

        Destroy(block.gameObject);

        // handle surface block below
        if (bBelow != null)
        {
            var belowBlock = bBelow.GetComponent<Block>();
            SetSurface(belowBlock, true);
            belowBlock.UpdateBlockRenderer();
        }
    }

    public void Remove(Vector3Int pos)
    {
        throw new NotImplementedException();
    }

    /**
     * Updates block outputs that are shadowed from above
     * e.g. grassy/nongrassy dirt
     */
    public void HandleSurfaceBlocks(Block block) 
    {
        // y is Depth!
        var bAbove = transform.Find($"{block.Coord.x},{block.Coord.y-1},{block.Coord.z}");
        var bBelow = transform.Find($"{block.Coord.x},{block.Coord.y+1},{block.Coord.z}");

        // check if there is a block above
        SetSurface(block, bAbove == null);

        if (bBelow != null)
        {
            var belowBlock = bBelow.GetComponent<Block>();
            SetSurface(belowBlock, false);
            belowBlock.UpdateBlockRenderer();
        }
    }

    /**
     * Changes normal block to surface block or vice versa
     */
    private void SetSurface(Block block, bool isSurface = true)
    {
        string blockName = block.block.name;

        BlockConfig surfaceBlock = null;
        BlockConfig undergroundBlock = null;

        // big ass if statement
        Match match = junkRegex.Match(blockName);
        if (match != null)
        {
            // Dirt or stone
            var junkType = match.Groups[1].Value;

            if (junkType.Equals("Dirt"))
            {
                surfaceBlock = biome.DirtSurface;
                undergroundBlock = biome.Dirt;
            }
            else if (junkType.Equals("Stone"))
            {
                surfaceBlock = biome.StoneSurface;
                undergroundBlock = biome.Stone;
            }
        }
        else
        {
            // any other block: find in biome spawn list
            foreach(var spawnable in biome.spawnableBlocks)
            {
                if (blockName.Equals(spawnable.block.name) || blockName.Equals(spawnable.blockSurface.name))
                {
                    // found the block in spawn list
                    surfaceBlock = spawnable.blockSurface;
                    undergroundBlock = spawnable.block;

                    break;
                }
            }
        }

        if (surfaceBlock == null || undergroundBlock == null)
        {
            // no surface version found for this block. we leave it be

            //Debug.LogError($"[SetSurface] block not found in biome spawn list: {blockName}, regex match: {match.Groups.Count}");
            return;
        }

        // change render:
        if (isSurface && surfaceBlock != null)
            block.block = surfaceBlock;
        else
            block.block = undergroundBlock;
        //block.UpdateBlockRenderer();
    }

    public bool CheckValidCoordinate(Vector3Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.z >= 0 &&
            pos.x <= biome.WorldBounds.x && pos.y <= biome.WorldBounds.y && pos.z <= biome.WorldBounds.z;
    }

    public Vector3 Grid2WorldPos(Vector3Int pos)
    {
        // transform coord space x,y,z to world space
        // @TEMPORAL: for now this is as-is
        return new Vector3(pos.x, -pos.y, pos.z) * 2f;
    }

    public IEnumerable<Vector3Int> Neighbors(Vector3Int p0, bool check2D = false)
    {
        if (!check2D)
            foreach (var dir in ndir)
                yield return p0 + dir;
        else
            for (int i = 2; i < ndir.Length; i++)
                yield return p0 + ndir[i];
    }

    internal Vector3Int FindFreeSpace(Vector3Int c, bool horizontal = false)
    {
        foreach (Vector3Int neighbour in Neighbors(c, horizontal))
            if (freeSpaces.Contains(neighbour))
                return neighbour;
        return Vector3Int.down*9999;
    }

    public bool FindPathAStar(Vector3Int Start, Vector3Int End)
    {
        Queue<Vector3Int> q = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        q.Enqueue(Start);


        while (q.Count > 0)
        {
            var cell = q.Dequeue();

            if (cell == End)
                return true;

            foreach (Vector3Int neighbour in Neighbors(cell))
            {
                if (!visited.Contains(neighbour) && freeSpaces.Contains(neighbour))
                {
                    q.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }
        }

        return false;
    }

    public void Clear()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // display free spaces
        Gizmos.color = Color.cyan;

        foreach (var free in freeSpaces)
        {
            var p = free * 2;
            p.y *= -1;
            Gizmos.DrawWireCube(p, Vector3.one * 1.6f);
        }
    }

    [ContextMenu("Reset Surface blocks")]
    void __ctxUpdateSurfaces()
    {
        foreach(Transform child in transform)
        {
            HandleSurfaceBlocks(child.GetComponent<Block>());
        }
    }

    [ContextMenu("List Free Spaces")]
    void __ctxUpdateDebug()
    {
        Debug.Log("Free spaces: " +string.Join(", ", freeSpaces));
    }

    [ContextMenu("Clear")]
    void __ctxUpdateClear()
    {
        Clear();
    }
# endif
}