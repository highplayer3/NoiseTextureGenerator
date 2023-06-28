using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Noise
{
    /// <summary>
    /// 用于获得伪随机梯度的排列表，有Perlin本人提供
    /// </summary>
    private static readonly int[] p = {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151
    };

    private static readonly int[] perm = new int[512];

    #region 插值运算
    /// <summary>
    /// 余弦插值
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private static float Cos_Interpolate(float a, float b, float t)
    {
        double ft = t * Math.PI;
        t = (float)((1 - Math.Cos(ft)) * 0.5);
        return a * (1 - t) + t * b;
    }
    /// <summary>
    /// heimite插值
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private static float Hermite_Interpolate(float a, float b, float t)
    {
        return Mathf.SmoothStep(a, b, t);
    }

    #endregion

    #region Perlin Noise
    public static float perlinNoise(float x, float y)
    {
        for (int i = 0; i < 512; i++)
        {
            perm[i] = p[i & 255];
        }
        List<Vector2> gradientList = CalculateGradient();
        Vector2 pos = new Vector2(x, y);
        Vector2 rightUp = new Vector2((int)x + 1, (int)y + 1);
        Vector2 rightDown = new Vector2((int)x + 1, (int)y);
        Vector2 leftUp = new Vector2((int)x, (int)y + 1);
        Vector2 leftDown = new Vector2((int)x, (int)y);

        float v1 = DotGridGradient(gradientList[GradientIndex((int)leftDown.x, (int)leftDown.y)], GetDisVec(pos, leftDown));
        float v2 = DotGridGradient(gradientList[GradientIndex((int)rightDown.x, (int)rightDown.y)], GetDisVec(pos, rightDown));
        float interpolation1 = Cos_Interpolate(v1, v2, x - (int)x);

        float v3 = DotGridGradient(gradientList[GradientIndex((int)leftUp.x, (int)leftUp.y)], GetDisVec(pos, leftUp));
        float v4 = DotGridGradient(gradientList[GradientIndex((int)rightUp.x, (int)rightUp.y)], GetDisVec(pos, rightUp));
        float interpolation2 = Cos_Interpolate(v3, v4, x - (int)x);

        return Cos_Interpolate(interpolation1, interpolation2, y - (int)y);
    }
    #region PerlinNoise版本2
    public static float perlinNoise2(float x, float y)
    {
        Vector2 pos = new Vector2(x, y);
        //声明该点所处的'格子'的四个顶点坐标
        Vector2 rightUp = new Vector2((int)x + 1, (int)y + 1);
        Vector2 rightDown = new Vector2((int)x + 1, (int)y);
        Vector2 leftUp = new Vector2((int)x, (int)y + 1);
        Vector2 leftDown = new Vector2((int)x, (int)y);
        float v1 = dotGridGradient(leftDown, pos);
        float v2 = dotGridGradient(rightDown, pos);
        float interpolation1 = Cos_Interpolate(v1, v2, x - (int)x);

        //计算y上的插值
        float v3 = dotGridGradient(leftUp, pos);
        float v4 = dotGridGradient(rightUp, pos);
        float interpolation2 = Cos_Interpolate(v3, v4, x - (int)x);

        float value = Cos_Interpolate(interpolation1, interpolation2, y - (int)y);
        return value;
    }
    private static float dotGridGradient(Vector2 p1, Vector2 p2)
    {
        Vector2 gradient = randomVector2(p1);
        Vector2 offset = p2 - p1;
        return Vector2.Dot(gradient, offset) / 2 + 0.5f;
    }
    private static Vector2 randomVector2(Vector2 p)
    {
        float random = Mathf.Sin(666 + p.x * 5678 + p.y * 1234) * 4321;
        return new Vector2(Mathf.Sin(random), Mathf.Cos(random));
    }
    #endregion

    /// <summary>
    /// 获取各个点的伪随机梯度
    /// </summary>
    /// <param name="x">点的X坐标</param>
    /// <param name="y">点的Y坐标</param>
    /// <returns></returns>
    private static int GradientIndex(int x, int y)
    {
        return perm[(x + perm[y & 255]) & 255] & 7;//&7是为了保证取梯度向量时不越界
    }
    /// <summary>
    /// 计算伪随机向量
    /// </summary>
    /// <returns></returns>
    private static List<Vector2> CalculateGradient()
    {
        //可以采用Perlin在论文中给出的12个向量
        //我们这里采用在单位圆上分布均匀的单位向量
        List<Vector2> tempList = new List<Vector2>();
        for (int i = 0; i < 8; i++)
        {
            //
            Vector2 vec = new Vector2(Mathf.Cos(Mathf.PI / 4 * i), Mathf.Sin(Mathf.PI / 4 * i));
            tempList.Add(vec);
        }
        return tempList;
    }
    private static Vector2 CalculateGradient2(Vector2 p)
    {
        List<Vector2> tempList = new List<Vector2>();

        float random = Mathf.Sin(666 + p.x * 5678 + p.y * 1234) * 4321;
        Vector2 vec = new Vector2(Mathf.Sin(random), Mathf.Cos(random));

        return vec;
    }
    /// <summary>
    /// 获得各顶点到指定点的方向向量
    /// </summary>
    /// <param name="p1">指定点</param>
    /// <param name="p2">分别为四个顶点</param>
    /// <returns></returns>
    private static Vector2 GetDisVec(Vector2 p1, Vector2 p2)
    {
        return p1 - p2;
    }
    /// <summary>
    /// 将梯度向量和个方向向量进行点乘
    /// </summary>
    /// <param name="gradient"></param>
    /// <param name="dirVec"></param>
    /// <returns></returns>
    private static float DotGridGradient(Vector2 gradient, Vector2 dirVec)
    {
        return Vector2.Dot(gradient, dirVec) / 2 + 0.5f;
    }
    #endregion

    #region Simplex Noise
    //梯度表
    private static int[,] gradArray = new int[,] { { 1, 1, 0 }, {-1,1,0 }, {1,-1,0 },{-1,-1,0 },
                                                   {1,0,1 },{-1,0,1 },{1,0,-1 },{-1,0,-1 },
                                                    { 0,1,1},{0,-1,1 },{0,1,-1 },{0,-1,-1 } };
    public static float SimplexNoise(float x, float y)
    {
        for (int i = 0; i < 512; i++)
        {
            perm[i] = p[i & 255];
        }
        //①:坐标转换
        double n0, n1, n2;//单形中三个顶点的影响值
        double F2 = 0.5 * (Math.Sqrt(3.0) - 1.0);
        //转化到正超晶体坐标中
        double X = x + (x + y) * F2;
        double Y = y + (x + y) * F2;
        //得到正超晶体左下角的坐标
        int I0 = GetFloor(X);
        int J0 = GetFloor(Y);

        double G2 = (3.0 - Math.Sqrt(3.0)) / 6.0;
        //将上一步得到的I0，J0转化回单形坐标系
        double i0 = I0 - (I0 + J0) * G2;
        double j0 = J0 - (I0 + J0) * G2;
        //得到单形坐标系中输入点到初始点的坐标(每个正超晶体的左下角为初始点)
        double dx0 = x - i0;
        double dy0 = y - j0;

        //②:确定输入点位于上边的单形还是下边的
        int I1, J1;
        if (dx0 > dy0)
        {
            I1 = 1; J1 = 0;
        }
        else
        {
            I1 = 0; J1 = 1;
        }
        //求输出点到单形坐标系中另外两个顶点的距离
        double dx1 = dx0 - I1 + G2;
        double dy1 = dy0 - J1 + G2;
        double dx2 = dx0 - 1.0 + 2.0 * G2;
        double dy2 = dy0 - 1.0 + 2.0 * G2;

        //③:梯度值选择
        int ii = I0 & 255;
        int jj = J0 & 255;
        int g0 = perm[ii + perm[jj]] % 12;
        int g1 = perm[ii + I1 + perm[jj + J1]] % 12;
        int g2 = perm[ii + 1 + perm[jj + 1]] % 12;

        //④;计算三个单形顶点分别对输入点的贡献值
        double t0 = 0.5 - dx0 * dx0 - dy0 * dy0;
        if (t0 < 0) n0 = 0.0;
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * (gradArray[g0, 0] * dx0 + gradArray[g0, 1] * dy0);
        }
        double t1 = 0.5 - dx1 * dx1 - dy1 * dy1;
        if (t1 < 0) n1 = 0.0;
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * (gradArray[g1, 0] * dx1 + gradArray[g1, 1] * dy1);
        }
        double t2 = 0.5 - dx2 * dx2 - dy2 * dy2;
        if (t2 < 0) n2 = 0.0;
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * (gradArray[g2, 0] * dx2 + gradArray[g2, 1] * dy2);
        }
        return (float)(70.0 * (n0 + n1 + n2) + 1) / 2;
    }

    private static int GetFloor(double variable)
    {
        return variable > 0 ? (int)variable : (int)variable - 1;
    }
    #endregion

    #region Worely Noise(简易版)
    private static Vector2 Hash(Vector2 p)
    {
        float random = Mathf.Sin(66 + p.x * 5678 + p.y * 1234) * 4321;
        return new Vector2(p.x + Mathf.Sin(random) / 2 + 0.5f, p.y + Mathf.Cos(random) / 2 + 0.5f);
    }
    public static float WorleyNoise1(float x, float y)
    {
        float distance = float.MaxValue;
        for (int Y = -1; Y <= 1; Y++)
        {
            for (int X = -1; X <= 1; X++)
            {
                Vector2 cellPoint = Hash(new Vector2((int)x + X, (int)y + Y));
                distance = Mathf.Min(distance, Vector2.Distance(cellPoint, new Vector2(x, y)));
            }
        }
        return distance;
    }
    #endregion

    #region Worley Noise(权威版)
    public static float WorleyNoise(float x, float y, int seed, Func<Vector2, Vector2, float> distanceFunc)
    {
        uint lastRandom, numberOfFeaturePoints;
        Vector2 randomDiff, featurePoint;
        //用于保存距离的列表
        List<float> distanceList = new List<float>();
        int X = GetFloor(x);
        int Y = GetFloor(y);
        int cubeX, cubeY;

        for (int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                cubeX = X + i;
                cubeY = Y + j;
                //对应步骤②，生成一个随机数
                lastRandom = LCGRandomGenerator(FNV_Hash((uint)(cubeX + seed), (uint)(cubeY)));
                //步骤③,决定有多少个特征点在目前的晶格中
                numberOfFeaturePoints = proLookup(lastRandom);

                for (int k = 0; k < numberOfFeaturePoints; ++k)
                {
                    lastRandom = LCGRandomGenerator(lastRandom);
                    randomDiff.x = (float)lastRandom / 0x100000000;

                    lastRandom = LCGRandomGenerator(lastRandom);
                    randomDiff.y = (float)lastRandom / 0x100000000;

                    featurePoint = new Vector2(randomDiff.x + (float)cubeX, randomDiff.y + (float)cubeY);
                    float distance = distanceFunc(new Vector2(x, y), featurePoint);
                    distanceList.Add(distance);//最好使用优先队列，插入后即可得到最小值
                }
            }
        }
        distanceList.Sort();
        return distanceList[0];
    }



    /// <summary>
    /// 让特征点随机非均匀地分布在该晶格中
    /// </summary>
    /// <param name="lastRandom"></param>
    /// <returns></returns>
    private static uint proLookup(uint value)
    {
        if (value < 393325350) return 1;
        if (value < 1022645910) return 2;
        if (value < 1861739990) return 3;
        if (value < 2700834071) return 4;
        if (value < 3372109335) return 5;
        if (value < 3819626178) return 6;
        if (value < 4075350088) return 7;
        if (value < 4203212043) return 8;
        return 9;
    }
    /// <summary>
    /// FNVHash算法
    /// </summary>
    private const uint OFFSET_BASIS = 2166136261;
    private const uint FNV_PRIME = 16777619;
    private static uint FNV_Hash(uint i, uint j)
    {
        return (uint)((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME);
    }

    /// <summary>
    /// LCG随机数生成器
    /// </summary>
    /// <param name="lastValue"></param>
    /// <returns></returns>
    private static uint LCGRandomGenerator(uint lastValue)
    {
        return (uint)((1103515245u * lastValue + 12345u) % 0x100000000u);
    }

    #endregion
}

