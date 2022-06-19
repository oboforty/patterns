using UnityEngine;

public class MinionFactory : MonoBehaviour
{
    TaskScheduler tasks;
    public Chonk chonk;

    private void Awake()
    {
        if (chonk.TryGetComponent(out tasks))
        {
            // discover already existing minions
            for (int i = 0; i < transform.childCount; i++)
                tasks.freeMinions.Add(transform.GetChild(i).GetComponent<Minion>());
        }
    }

    public void Create(Block block)
    {
       
        // @TODO: ITT: 
        //      minion kiugrik a fabol
        //      remove both leaves if it has 2
        //      find landing site for jump (first free space that is not in floating air)
        //      anim vfx + SFX




        // find the next free space
        var v = chonk.FindFreeSpace(block.Coord, horizontal: true);

        if (v.x >= 9999)
        {
            Debug.LogError("Free space not found ");

            // @TODO: error vfx & sfx
            return;
        }

        var obj = Instantiate(block.spawnPrefab, transform);
        // since we're spawning from leaf, the minion spawns one level below it
        //v.y += 1;
        //obj.transform.position = chonk.Grid2WorldPos(v);
        obj.transform.position = chonk.Grid2WorldPos(block.Coord);

        if (obj.TryGetComponent(out Minion minion))
        {
            // set up minion
            if (tasks != null)
                tasks.freeMinions.Add(minion);
            else
                Debug.LogError("Tasks script not found at minion spawn");
        }

        // Deletes the block that spawned the minion
        Destroy(block.gameObject);
    }
}
