using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "new Block", menuName = "Saludia/New Block")]
public class BlockConfig : ScriptableObject
{
    // VISUALIZATION
    [SerializeField]
    Sprite sprite = null;
    [SerializeField]
    Sprite spriteTop = null; // top sprite. if null = sprite
    public Sprite icon;

    Texture2D m_texture;
    Texture2D m_textureTop;
    public bool isAnimating { get; set; } = false;

    // CRAFTING
    public bool isCrafted = false;
    public bool isRefined = false;
    public BlockConfig[] recipe; // required recipe of crafting

    // Interaction
    public bool isInteractive = false;
    public bool isInteracted { get; set; } = false;

    const string TEXT = "_BaseMap";

    MaterialPropertyBlock propBlock;

    private void Awake()
    {
        if (icon == null)
            icon = sprite;
    }

    public void ShowOn(Renderer _render)
    {
        var mats = _render.sharedMaterials;

        if (mats.Length == 0)
        {
            Debug.LogError("Materials not found bro: " + name);
            return;
        }
        else if (texture == null)
        {
            Debug.LogError("Block Texture not found bro: " + name);
            return;
        }
        propBlock = new MaterialPropertyBlock();

        // Set Sides:
        _render.GetPropertyBlock(propBlock, 0);
        propBlock.SetTexture(TEXT, texture);
        _render.SetPropertyBlock(propBlock, 0);

        if (mats.Length == 1)
            return;

        // Set top texture:
        _render.GetPropertyBlock(propBlock, 1);
        propBlock.SetTexture(TEXT, textureTop);
        _render.SetPropertyBlock(propBlock, 1);
    }

    Texture2D texture
    {
        get
        {
            if (m_texture == null)
                m_texture = GitText(sprite);
            return m_texture;
        }
    }

    Texture2D textureTop
    {
        get
        {
            if (m_textureTop == null)
            {
                if (spriteTop == null)
                    m_textureTop = texture;
                else
                    m_textureTop = GitText(spriteTop);
            }

            return m_textureTop;
        }
    }


    Texture2D GitText(Sprite sprite)
    {
        int w = Mathf.FloorToInt(sprite.rect.width);
        int h = Mathf.FloorToInt(sprite.rect.height);
        int ox = Mathf.RoundToInt(sprite.textureRect.x);
        int oy = Mathf.RoundToInt(sprite.textureRect.y);

        var texture = new Texture2D(w, h);
        var pixels = sprite.texture.GetPixels(ox, oy, w, h);

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    public void Clear()
    {
        // Gecc
        m_texture = null;
        m_textureTop = null;
    }

#if UNITY_EDITOR
    [MenuItem("Window/Saludia/Spawn 10 Blocks")]
    static void SpawnALot()
    {

        for(int i =0; i < 10; i++)
        {
            // Get current folder:
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();

            // Spawn new asset
            BlockConfig asset = CreateInstance<BlockConfig>();

            AssetDatabase.CreateAsset(asset, $"{pathToCurrentFolder}/New Block {i}.asset");
            AssetDatabase.SaveAssets();
        }
    }
#endif
}