using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockConfig block;
    [Space(12)]
    public Vector3Int Coord;
    [Tooltip("When interacting with the block, this prefab is spawned instead of being mined")]
    public GameObject spawnPrefab = null;

    Renderer m_Renderer;

    private void Awake()
    {
        // https://www.youtube.com/watch?v=sQHq2IwHSX4

        if (block != null)
            UpdateBlockRenderer();
    }

    public void UpdateBlockRenderer()
    {
        if (m_Renderer == null)
            TryGetComponent(out m_Renderer);

        if (block == null || m_Renderer == null)
        {
            Debug.LogError($"Missing components for b: {block != null}, r: {m_Renderer != null}");
            return;
        }

        //name = block.name;
        block.ShowOn(m_Renderer);
    }

    public override bool Equals(object other)
    {
        return Coord.Equals((other as Block).Coord);
    }

    public override int GetHashCode()
    {
        return Coord.GetHashCode();
    }

# if UNITY_EDITOR
    double last_update = 0;
    //static Dictionary<string, long> ellapsedTimes = new Dictionary<string, long>();

    private void OnDrawGizmosSelected()
    {
        //var watch = new System.Diagnostics.Stopwatch();
        //watch.Start();
        var now = EditorApplication.timeSinceStartup;

        // this makes the editor less laggy
        if (now - last_update < 1)
            return;

        block.Clear();
        UpdateBlockRenderer();

        last_update = now;
        //watch.Stop();
        //ellapsedTimes[name] = watch.ElapsedMilliseconds;
    }

    [ContextMenu("Update block render")]
    void __ctxUpdateBlock() 
    {
        block.Clear();
        UpdateBlockRenderer();
    }

    //[ContextMenu("Measure Render time")]
    //void __ctxGetRenderTime()
    //{
    //    Debug.Log(ellapsedTimes.Values.Sum());
    //}

#endif
}
