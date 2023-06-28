using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CubicBezierCurve : MonoBehaviour
{
    [SerializeField] private Transform start, end, control1,control2;
    [SerializeField] private int segmentNum;
    private LineRenderer lineRenderer;
    private List<Vector3> curvePath;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        DrawBezier();
    }

    private void DrawBezier()
    {
        curvePath = BezierUtility.GetCubicBezier(start.position, control1.position,control2.position,end.position, segmentNum);
        lineRenderer.positionCount = curvePath.Count;
        lineRenderer.SetPositions(curvePath.ToArray());
    }
}
