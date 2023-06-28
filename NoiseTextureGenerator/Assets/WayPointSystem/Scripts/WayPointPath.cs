using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class WayPointPath : MonoBehaviour
{
    //[SerializeField] private Transform[] points;
    [SerializeField] private List<Point> points;
    private Vector3[] wayPoints;
    private LineRenderer lineRenderer;
    public const int countBetween2Point = 20;

    public Vector3[] WayPoints { get => wayPoints;}
    public List<Point> Points { get => points;}

    Vector3 firstPos, curPos, nextPos, lastPos;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        wayPoints = new Vector3[countBetween2Point * (points.Count - 1) + 1];
        lineRenderer.positionCount = wayPoints.Length;
    }

    private void Update()
    {
        CalculateCurve();
        lineRenderer.SetPositions(wayPoints);
    }

    void CalculateCurve()
    {
        //依次计算相邻两点间曲线
        //由四个点确定一条曲线（当前相邻两点p1,p2，以及前后各一点p0,p3）
        for (int i = 0; i < points.Count - 1; i++)
        {
            //特殊位置增加虚拟点
            //如果p1点是第一个点，不存在p0点，由p1,p2确定一条直线，在向量(p2p1)方向确定虚拟点p0
            if (i == 0)
                firstPos = points[i].transform.position * 2 - points[i + 1].transform.position;
            else
                firstPos = points[i - 1].transform.position;
            //中间点
            curPos = points[i].transform.position;
            nextPos = points[i + 1].transform.position;
            //特殊位置增加虚拟点，同上
            if (i == points.Count - 2)
                lastPos = points[i + 1].transform.position * 2 - points[i].transform.position;
            else
                lastPos = points[i + 2].transform.position;

            CatmulRom(firstPos, curPos, nextPos, lastPos, ref wayPoints, countBetween2Point * i);
        }
        //加入最后一个点位
        wayPoints[wayPoints.Length - 1] = points[points.Count - 1].transform.position;
    }

    //平滑过渡两点间曲线（p1,p2为端点，p0,p3是控制点）
    void CatmulRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, ref Vector3[] points, int startIndex)
    {
        //计算Catmull-Rom样条曲线
        float t0 = 0;
        float t1 = GetT(t0, p0, p1);
        float t2 = GetT(t1, p1, p2);
        float t3 = GetT(t2, p2, p3);

        float t;
        for (int i = 0; i < countBetween2Point; i++)
        {
            t = t1 + (t2 - t1) / countBetween2Point * i;

            Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
            Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
            Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

            Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
            Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

            Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

            points[startIndex + i] = C;
        }
    }

    float GetT(float t, Vector3 p0, Vector3 p1)
    {
        return t + Mathf.Pow(Mathf.Pow((p1.x - p0.x), 2) + Mathf.Pow((p1.y - p0.y), 2) + Mathf.Pow((p1.z - p0.z), 2), 0.5f);
    }
}
