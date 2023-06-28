using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongWithCurve:MonoBehaviour
{
    /*[SerializeField]*/ private Vector3[] movePoints;
    [SerializeField] private CatmullRomSpline catmullRomSpline;
    [SerializeField] private float maxSpeed;
    private int index;
    private bool isReverse = false;
    

    private void Start()
    {
        index = 0;
    }
    private void Update()
    {
        if(catmullRomSpline.CurvePoints!=null&&catmullRomSpline.CurvePoints.Length>0)
            MovePos(catmullRomSpline.CurvePoints);
    }

    private void MovePos(Vector3[] movePoints)
    {
        //if (index > movePoints.Length - 1 || index < 0) return;
        if (isReverse == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoints[index], maxSpeed * Time.deltaTime);
            if (Vector3.Distance(movePoints[index], transform.position) < 0.00001f)
            {
                //延时调用
                index++;
            }
            if (index == movePoints.Length)
            {
                isReverse = true;
                index -= 2;
            }
        }
        else 
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoints[index], maxSpeed * Time.deltaTime);
            if (Vector3.Distance(movePoints[index], transform.position) < 0.00001f)
            {
                index--;
            }
            if (index == -1)
            {
                isReverse = false;
                index = 0;
            }
        }
        
    }
}
