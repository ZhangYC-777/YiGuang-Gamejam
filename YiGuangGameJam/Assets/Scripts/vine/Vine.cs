using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    [SerializeField] private float growSpeed = 3f; // 藤曼生长速度，即藤曼头移动速度
    [SerializeField] private Vector2 growDirection = Vector2.up; // 藤曼生长方向
    [SerializeField] private float pointDistance = 0.1f; // 记录的两个藤曼点之间的距离
    [SerializeField] private List<Vector3> pointList; // 藤曼点位置列表

    private Transform vineHead; // 藤曼头
    private Vector3 lastPoint; // 记录的上一个藤蔓点的位置
    private LineRenderer lineRenderer; // 用来连点成线的
 
    // Start is called before the first frame update
    void Start()
    {
        vineHead = GameObject.Find("Head").GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();

        DrawLine();
    }

    // Update is called once per frame
    void Update()
    {
        VineGrow();
    }

    // 藤曼生长
    private void VineGrow()
    {
        vineHead.transform.position += (Vector3)(growSpeed * growDirection * Time.deltaTime);

        if (Vector3.Distance(vineHead.position, lastPoint) > pointDistance)
        {
            DrawLine();
        }
    }

    // 绘制藤曼
    private void DrawLine()
    {
        pointList.Add(vineHead.position);
        lastPoint = vineHead.position;

        lineRenderer.positionCount = pointList.Count;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, pointList[i]);
        }
    }
}
