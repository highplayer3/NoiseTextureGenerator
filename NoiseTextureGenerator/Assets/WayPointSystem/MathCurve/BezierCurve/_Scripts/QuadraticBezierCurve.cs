using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class QuadraticBezierCurve : MonoBehaviour
{
    [SerializeField] private Transform start, end, control;
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
        curvePath = BezierUtility.GetQuadraticBezier(start.position, control.position, end.position, segmentNum);
        lineRenderer.positionCount = curvePath.Count;
        lineRenderer.SetPositions(curvePath.ToArray());
    }
}
