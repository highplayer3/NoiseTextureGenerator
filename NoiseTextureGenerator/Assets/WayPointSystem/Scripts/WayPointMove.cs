using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointMove : MonoBehaviour
{
    /*[SerializeField]*/
    private Vector3[] movePoints;
    [SerializeField] private WayPointPath pathContainer;
    [SerializeField] private float maxSpeed;
    private float reserveSpeed;
    private int index;
    private bool isReverse = false;
    private bool isOnCoroutine = false;


    private void Start()
    {
        index = 0;
        reserveSpeed = maxSpeed;
    }
    private void Update()
    {
        if (pathContainer.WayPoints.Length>0)
            MovePos(pathContainer.WayPoints);
    }

    private void MovePos(Vector3[] movePoints)
    {
        //if (index > movePoints.Length - 1 || index < 0) return;
        if (isReverse == false)
        {
            if (index < movePoints.Length)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoints[index], maxSpeed * Time.deltaTime);
                if (Vector3.Distance(movePoints[index], transform.position) < 0.00001f)
                {
                    //延时调用
                    //Invoke("Delay", movePoints[index].stopTime);
                    StartCoroutine(HandleWithPoint());
                    //index++;
                }
            }
            if (index == movePoints.Length)
            {
                isReverse = true;
                index -= 2;
            }
        }
        else
        {
            if (index >= 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoints[index], maxSpeed * Time.deltaTime);
                if (Vector3.Distance(movePoints[index], transform.position) < 0.00001f)
                {
                    //Invoke("Delay", movePoints[index].stopTime);
                    StartCoroutine(HandleWithPoint());
                }
            }
            if (index == -1)
            {
                isReverse = false;
                index = 0;
            }
        }

    }

    private IEnumerator HandleWithPoint()
    {
        if (isOnCoroutine) yield break;
        if (index % WayPointPath.countBetween2Point != 0)
        {
            if (isReverse) index--;
            else index++;
            yield break;
        }
        isOnCoroutine = true;
        Debug.Log("开启协程" + index + "-" + index / WayPointPath.countBetween2Point);
        maxSpeed = 0f;
        float timer = pathContainer.Points[index / WayPointPath.countBetween2Point].stopTime;
        pathContainer.Points[index / WayPointPath.countBetween2Point].eventInPoint.Invoke();
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //Debug.Log("第" + index / WayPointPath.countBetween2Point + "个点正在等待");
            yield return null;
        }
        if (isReverse) index--;
        else index++;
        maxSpeed = reserveSpeed;
        isOnCoroutine = false;
    }
    public void TransformScale()
    {
        transform.localScale = new Vector3(1, 0.5f, 1);
    }
    public void TransformOrigin()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
}
