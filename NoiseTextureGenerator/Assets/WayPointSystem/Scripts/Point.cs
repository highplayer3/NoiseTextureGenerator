using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

[System.Serializable]
public class Point
{
    public Transform transform;
    public float stopTime;
    public UnityEvent eventInPoint;
    public Point() { }
    public Point(Transform thisTrasnform)
    {
        transform = thisTrasnform;
        stopTime = 0f;
    }
}
