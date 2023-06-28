using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CalculateDistance
{
    public static float EuclidianDistanceFunc(Vector2 p1, Vector2 p2)
    {
        return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
    }
    public static float ManhattanDistanceFunc(Vector2 p1, Vector2 p2)
    {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }
    public static float ChebyshevDistanceFunc(Vector2 p1, Vector2 p2)
    {
        Vector2 diff = p1 - p2;
        return Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.y));
    }
}