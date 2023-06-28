using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BezierUtility
{
    /// <summary>
    /// 线性插值
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">终点</param>
    /// <param name="t">[0,1]</param>
    /// <returns>插值后的点</returns>
    public static Vector3 Interpolation(Vector3 p0, Vector3 p1, float t)
    {
        return (1 - t) * p0 + t * p1;
    }
    /// <summary>
    /// 二次插值
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点</param>
    /// <param name="p2">终点</param>
    /// <param name="t"></param>
    /// <returns>Equation:(1-t)^2*P0+2(1-t)t*P1+t^2*P2</returns>
    public static Vector3 QuadraticInterpolation(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Interpolation(p0, p1, t);
        Vector3 b = Interpolation(p1, p2, t);
        return Interpolation(a, b, t);
    }
    /// <summary>
    /// 三次插值
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点1</param>
    /// <param name="p2">控制点2</param>
    /// <param name="p3">终点</param>
    /// <param name="t"></param>
    /// <returns>(1-t)^3*P0+3(1-t)^2*t*P1+3(1-t)t^2*P2+t^3*P3</returns>
    public static Vector3 CubicInterpolation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = QuadraticInterpolation(p0, p1, p2, t);
        Vector3 b = QuadraticInterpolation(p1, p2, p3, t);
        return Interpolation(a, b, t);
    }
    /// <summary>
    /// 获取一次贝塞尔曲线上的点
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="segmentNumber">采样点的数量</param>
    /// <returns></returns>
    public static List<Vector3> GetLinearBezier(Vector3 startPoint, Vector3 endPoint, int segmentNumber)
    {
        List<Vector3> path = new List<Vector3>();
        for (int i = 0; i < segmentNumber; i++)
        {
            float t = i / (float)segmentNumber;
            Vector3 tempPoint = Interpolation(startPoint, endPoint, t);
            path.Add(tempPoint);
        }
        //path.Add(endPoint);
        return path;
    }
    /// <summary>
    /// 获取二次贝塞尔曲线的上点
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="controlPoint">控制点</param>
    /// <param name="endPoint"></param>
    /// <param name="segmentNumber"></param>
    /// <returns></returns>
    public static List<Vector3> GetQuadraticBezier(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNumber)
    {
        List<Vector3> path = new List<Vector3>();
        for (int i = 0; i < segmentNumber; i++)
        {
            float t = i / (float)segmentNumber;
            Vector3 tempPoint = QuadraticInterpolation(startPoint, controlPoint, endPoint, t);
            path.Add(tempPoint);
        }
        //path.Add(endPoint);
        return path;
    }

    public static List<Vector3> GetCubicBezier(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 endPoint, int segmentNumber)
    {
        List<Vector3> path = new List<Vector3>();
        for (int i = 0; i < segmentNumber; i++)
        {
            float t = i / (float)segmentNumber;
            Vector3 tempPoint = CubicInterpolation(startPoint, controlPoint1, controlPoint2, endPoint, t);
            path.Add(tempPoint);
        }
        //path.Add(endPoint);
        return path;
    }
}
