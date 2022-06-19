using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Setup
{
    public class SpawnerInstaller : MonoBehaviour
    {
        // @todo: @later: only install if game files found
        public Chonk chonk;

        [Header("Spawning biome")]
        public bool dontSpawnJunk = false;
        public bool spawnOnAwake = true;
        public int Seed = 255;

        [Header("Spawning minions")]
        public List<GameObject> minionPrefabs;


        HashSet<Vector3Int> occupiedBlocks;
        Queue<(int, BlockConfig)> leftOvers;

        int groundLvl => chonk.biome.athmosphereDepth;
        int Depth => chonk.biome.WorldBounds.y;
        Vector2Int Dim => new Vector2Int(chonk.biome.WorldBounds.x, chonk.biome.WorldBounds.z);


        /// External GameEvent
        public void __Install()
        {
            if (spawnOnAwake)
            {
                chonk.Clear();
                SpawnBiome();

                Destroy(this);
            }
        }

        public void SpawnBiome()
        {
            Random.InitState(Seed == 0 ? Random.Range(0, 65565) : Seed);

            var biome = chonk.biome;
            chonk.name = "Chonk " + biome.Name;
            occupiedBlocks = new HashSet<Vector3Int>();
            leftOvers = new Queue<(int, BlockConfig)>();

            // Generate Trees
            GenerateTrees();

            // Generate Surface
            GenerateAir();

            // Generate Metals
            foreach(var spawn in biome.spawnableBlocks)
                Generate(spawn);

            // Generate Special objects

            // Fill rest of area with stone or dirt
            if (!dontSpawnJunk)
                GenerateRestAs(biome.Stone, biome.StoneSurface, biome.Dirt, biome.DirtSurface, biome.defaultStoneChance);
        }

        private void GenerateTrees()
        {
            // generate trees
            int surfacegrids = Dim.x * Dim.y;
            int ntrees = Mathf.RoundToInt(chonk.biome.TreeToGrassRatio * surfacegrids);

            for (int i = 0; i < ntrees; i++)
            {
                int y = groundLvl - 1;
                int x = 0, z = 0;
                
                for (int attempt = 0; attempt < 5;  attempt++)
                {
                    x = Random.Range(0, Dim.x);
                    z = Random.Range(0, Dim.y);

                    if (!occupiedBlocks.Contains(new Vector3Int(x, y, z)))
                        break;
                }

                // unlucky, no trees generated
                if (occupiedBlocks.Contains(new Vector3Int(x, y, z)))
                    return;

                // only base of tree is added as occupied
                occupiedBlocks.Add(new Vector3Int(x, y, z));

                int T = Random.Range(chonk.biome.treeTrunkMin, chonk.biome.treeTrunkMax + 1);
                int L = Random.Range(chonk.biome.treeLeafMin, chonk.biome.treeLeafMax + 1);

                // tree trunk
                for (int j = 0; j < T; j++)
                    chonk.Spawn(chonk.biome.Tree, new Vector3Int(x, y - j, z), false);

                // tree leaves
                for (int j = 0; j < L; j++)
                    if (y - j - T > 0)
                    {
                        // spawn the leaves
                        Block leaf = chonk.Spawn(chonk.biome.TreeLeaf, new Vector3Int(x, y - j - T, z), false);

                        // but also leaves spawn minions
                        if (minionPrefabs.Count > 0)
                            leaf.spawnPrefab = minionPrefabs[Random.Range(0, minionPrefabs.Count-1)];
                    }
            }
        }

        private void GenerateAir()
        {
            for (int y = 0; y < groundLvl; y++)
            {
                // clear first levels
                for (int x = 0; x < Dim.x; x++)
                    for (int z = 0; z < Dim.y; z++)
                    {
                        var v = new Vector3Int(x, y, z);

                        if (!occupiedBlocks.Contains(v))
                            chonk.freeSpaces.Add(v);
                                                
                        // add free space anyway as occupied, because trees aren't
                        occupiedBlocks.Add(v);
                    }
            }
        }

        void Generate(Spawnable sp)
        {
            // 1. define Y level distrib table
            var dist = new DiscreteDistributionAnimSampler(groundLvl+1, Depth, sp.spawnRate);

            for (int i = 0; i < sp.totalSpawnNumber; i++)
            {
                Vector3Int v = Vector3Int.zero;

                // 2. determine x,y,z (Y level based on distribution)
                // make 5 attempts to generate rnd coordinate
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    int x = Random.Range(0, Dim.x);
                    int z = Random.Range(0, Dim.y);
                    int y = dist.Sample();
                    v = new Vector3Int(x, y, z);

                    if (!occupiedBlocks.Contains(v))
                        break;
                }

                // check if space is occuped
                if (occupiedBlocks.Contains(v))
                {
                    // we'll add the block later
                    leftOvers.Enqueue((v.y, sp.block));
                    continue;
                }

                // 3. Spawn block
                var block = chonk.Spawn(sp.block, v, false);
                // @TODO: determine if TOP surface!?!

                // 4. Mark as occupied
                occupiedBlocks.Add(v);
            }
        }

        /**
         * posBlock is generated on a positive event, neg vice versa
         */
        void GenerateRestAs(BlockConfig posBlock, BlockConfig posBlockTop, BlockConfig negBlock, BlockConfig negBlockTop, AnimationCurve curve)
        {
            int minLevel = 0;
            int maxLevel = Depth;

            // 1. add leftover blocks
            Debug.Log("Leftover blocks: " + leftOvers.Count);
            foreach((int y, BlockConfig block) in leftOvers)
            {
                bool stop = false;

                for (int x = 0; x < Dim.x; x++)
                {
                    for (int z = 0; z < Dim.y; z++)
                    {
                        var v = new Vector3Int(x, y, z);
                        if (!occupiedBlocks.Contains(v))
                        {
                            // Spawn leftover at different place
                            chonk.Spawn(block, v, false);
                            stop = true;
                        }
                    }

                    if (stop)
                        break;
                }
            }

            // 2. define Y level distrib table
            var dist = new UniformVariableFromDiscreteDistributionAnim(minLevel, maxLevel, curve);

            for (int y = groundLvl; y < Depth; y++)
            {
                var stoneChance = dist.GetProbabilityAt(y);

                for (int x = 0; x < Dim.x; x++)
                    for (int z = 0; z < Dim.y; z++)
                    {
                        var c = new Vector3Int(x, y, z);
                        if (occupiedBlocks.Contains(c))
                            continue;

                        Block block;
                        var v = new Vector3Int(x, y, z);

                        // 3. spawn missing block with stone or dirt
                        if (Random.value <= stoneChance)
                        {
                            if (y == groundLvl) block = chonk.Spawn(posBlockTop, v, false);
                            else                block = chonk.Spawn(posBlock, v, false);
                        }
                        else
                        {
                            if (y == groundLvl) block = chonk.Spawn(negBlockTop, v, false);
                            else                block = chonk.Spawn(negBlock, v, false);
                        }
                    }
            }
        }



#if UNITY_EDITOR
        [ContextMenu("Spawn blocks")]
        void __ctxUpdateSpawn()
        {
            chonk.Clear();
            SpawnBiome();
        }

        [ContextMenu("Clear")]
        void __ctxUpdateClear()
        {
            chonk.Clear();
        }
# endif
    }
}
