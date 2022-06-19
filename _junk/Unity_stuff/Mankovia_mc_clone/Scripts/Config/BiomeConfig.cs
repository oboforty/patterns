using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new Biome", menuName = "Saludia/New Biome")]
public class BiomeConfig : ScriptableObject
{
    public string Name;
    public Vector3Int WorldBounds = new Vector3Int(8, 8, 16);
    public int athmosphereDepth = 3;

    [Header("Tree blocks")]
    public BlockConfig Tree;
    public BlockConfig TreeLeaf;
    [Range(0.0f, 1.0f)]
    public float TreeToGrassRatio = 0.5f;
    public int treeLeafMax = 2;
    public int treeLeafMin = 2;
    public int treeTrunkMax = 1;
    public int treeTrunkMin = 1;

    [Header("Fillings")]
    public BlockConfig DirtSurface;
    public BlockConfig StoneSurface;
    [Tooltip("If no special block is spawned then this is the chance of stone spawning at depth level")]
    public AnimationCurve defaultStoneChance = AnimationCurve.Constant(0,1,0.1f);
    public BlockConfig Dirt;
    public BlockConfig Stone;

    //[Header("Area spawns")]

    [Header("Rare metals")]
    public Spawnable[] spawnableBlocks;
}

[Serializable]
public class Spawnable
{
    public BlockConfig block;
    public BlockConfig blockSurface = null;

    public int totalSpawnNumber = 1;
    [Tooltip("Chance of spawning at depth level")]
    public AnimationCurve spawnRate = AnimationCurve.Constant(0, 1, 0.1f);
}
