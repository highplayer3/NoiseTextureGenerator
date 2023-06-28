using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class NoiseMapWindow : OdinEditorWindow
{
    [MenuItem("MyTools/NoiseMapGenerator")]
    public static void OpenWindow()
    {
        var window = GetWindow<NoiseMapWindow>();
        window.Show();
    }
    [Title("Noise Type")]
    [EnumPaging]
    public NoiseType noiseType;

    [Title("Noise Parameters")]
    [LabelText("Texture's Width")]
    public int width=512;
    [LabelText("Texture's Height")]
    public int height=512;
    [CustomValueDrawer("MyCustomDrawerStatic")]
    public int scale;
    [Range(1, 10)]
    public int octaves = 1;
    [Range(0f, 1f)]
    public float persistance = 0.5f;
    [Range(1f, 5f)]
    public float lacunarity = 2f;
    [InfoBox("The Variable 'seed' can only work for Worley Noise Pro")]
    [EnableIf("noiseType",NoiseType.WorleyNoisePro)]
    [Range(0,10)]
    public int seed;

    [Title("Export Settings")]
    [FolderPath]
    public string SavePath;

    public string TextureName;
    
    [Title("Preview Image")]
    [PreviewField(200, ObjectFieldAlignment.Left)]
    public Texture2D NoiseMap;
    
    [Button(ButtonSizes.Medium)]
    [ButtonGroup("Function Group")]
    private void Preview()
    {
        if (width < 1 || height < 1)
        {
            Debug.LogError("图片长度或宽度必须大于1");
            return;
        }
        NoiseMap = GenerateTexture();
    }

    [ButtonGroup("Function Group")]
    private void Generate()
    {
        if (NoiseMap == null)
        {
            Debug.LogError("Texture还未生成,请先点击Preview");
        }
        if (TextureName == string.Empty)
        {
            Debug.LogError("请先输入Texture的名称");
        }
        File.WriteAllBytes(SavePath + "/" + TextureName + ".png", NoiseMap.EncodeToPNG());
        EditorUtility.DisplayDialog("成功", "噪声图\"" + TextureName + "\"" + "已保存！", "确定", "取消");
    }
    private static float MyCustomDrawerStatic(float value,GUIContent label)
    {
        return EditorGUILayout.Slider(label, value, 50f, 200f);
    }

    public Texture2D GenerateTexture()
    {
        Texture2D tex = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];
        float[,] noiseMap = GenerateNoiseValue(width, height, scale);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                colorMap[y * height + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        tex.SetPixels(colorMap);
        tex.Apply();
        return tex;
    }

    private float[,] GenerateNoiseValue(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];
        float noiseValue;
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;
                    switch (noiseType)
                    {
                        case NoiseType.PerlinNoise:
                            noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                            break;
                        case NoiseType.MyPerlinNoise:
                            noiseValue = Noise.perlinNoise(sampleX, sampleY);
                            break;
                        case NoiseType.SimplexNoise:
                            noiseValue = Noise.SimplexNoise(sampleX, sampleY);
                            break;
                        case NoiseType.WorleyNoise:
                            noiseValue = Noise.WorleyNoise1(sampleX, sampleY);
                            break;
                        case NoiseType.WorleyNoisePro:
                            noiseValue = Noise.WorleyNoise(sampleX, sampleY, seed, CalculateDistance.EuclidianDistanceFunc);
                            break;
                        default:
                            noiseValue = Random.value;
                            break;
                    }
                    noiseHeight += noiseValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }

    public enum NoiseType
    {
        WhiteNoise,
        PerlinNoise,
        MyPerlinNoise,
        SimplexNoise,
        WorleyNoise,
        WorleyNoisePro,
    }

}
