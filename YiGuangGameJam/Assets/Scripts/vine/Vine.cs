using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Vine : MonoBehaviour
{
    [SerializeField] private float growSpeed = 3f; // 藤曼生长速度，即藤曼头移动速度
    [SerializeField] private float pointDistance = 0.1f; // 记录的两个藤曼点之间的距离
    [SerializeField] private float maxTurnAngle = 60; // 最大转向角
    [SerializeField] private float turnSpeed = 90 * Mathf.Deg2Rad; // 每秒可转过的最大弧度，即转向速度
    [SerializeField] private GameObject leafPrefab; // 叶子预制体
    [SerializeField] private int leafIndexInterval = 20; // 间隔多少个藤蔓点生成叶子
    [SerializeField] private float leafOffset = 0.2f; //叶子偏移距离

    private Transform vineHead; // 藤曼头
    private Vector3 growDirection = Vector2.up; // 藤曼生长方向
    private Vector3 initialDirection = Vector2.up; // 藤曼生长初始方向
    private List<Vector3> pointList = new List<Vector3>(); // 藤曼点位置列表
    private Vector3 lastPoint; // 记录的上一个藤蔓点的位置
    private LineRenderer lineRenderer; // 用来连点成线的
    private Vector3 targetDirection; // 藤曼经过限制后的目标方向
    private int lastLeafIndex = 0; // 上一个生成叶子的藤蔓点索引
    private bool leftleaf = true; // 叶子是否生成在左侧

 
    // Start is called before the first frame update
    void Start()
    {
        vineHead = GameObject.Find("Head").GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();

        initialDirection = growDirection; // 记录藤曼生长初始方向

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
        UpdateGrowDirection();

        VineHeadMove();

        if (Vector3.Distance(vineHead.position, lastPoint) > pointDistance)
        {
            DrawLine();
        }

        if ((pointList.Count - 1 - lastLeafIndex) >= leafIndexInterval)
        {
            LeafGrow(leftleaf);
            leftleaf = !leftleaf;
            lastLeafIndex = lineRenderer.positionCount - 1;
        }
    }
    // 藤曼头移动
    private void VineHeadMove()
    {
        vineHead.transform.position += (Vector3)(growSpeed * growDirection * Time.deltaTime);
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

    // 通过鼠标更新藤曼生长方向
    private void UpdateGrowDirection()
    {   // 按下或按住鼠标左键时计算目标方向
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 获取鼠标世界坐标
            mouseWorldPos.z = vineHead.position.z; // 锁定鼠标世界坐标的z轴

            Vector3 Direction = (mouseWorldPos - vineHead.position).normalized; // 更新并单位化目标方向
            float angle = Vector2.SignedAngle(initialDirection, Direction); // 计算目标方向与初始方向之间夹角
            angle = Mathf.Clamp(angle, -maxTurnAngle, maxTurnAngle); // 若夹角数值大于限制的最大角度，将其值修改为限制的最大角度

            targetDirection = Quaternion.AngleAxis(angle, Vector3.forward) * initialDirection; // 确定经过限制的目标方向
        }

        growDirection = Vector3.RotateTowards(growDirection, targetDirection, turnSpeed * Time.deltaTime, 0); // 藤曼生长方向逐渐靠向目标方向，每帧计算
    }

    // 叶子生长
    private void LeafGrow(bool left)
    {
        int index = pointList.Count - 1;
        Vector3 vineDirection = (pointList[index] - pointList[index - 1]).normalized; // 叶子的生长方向
        Vector3 leafPosition; // 叶子生长位置
        float angle; // 叶子生长角度
        if (left)
        {
            Vector3 leftDirection = Quaternion.Euler(0, 0, -90) * vineDirection;
            leafPosition = pointList[index] + leftDirection * leafOffset;
            angle = Mathf.Atan2(leftDirection.y, leftDirection.x) * Mathf.Rad2Deg;
        }
        else
        {
            Vector3 rightDirection = Quaternion.Euler(0, 0, 90) * vineDirection;
            leafPosition = pointList[index] + rightDirection * leafOffset;
            angle = Mathf.Atan2(rightDirection.y, rightDirection.x) * Mathf.Rad2Deg;
        }
        GameObject leaf = Instantiate(leafPrefab, leafPosition, Quaternion.Euler(0, 0, angle)); // 实例化对应朝向的叶子
        Vector3 scale = leaf.transform.localScale;
        if (left)
        {
            scale.y = Mathf.Abs(scale.y);
        }
        else
        {
            scale.y = -Mathf.Abs(scale.y); // 右边的叶子得上下翻转一下
        }
        leaf.transform.localScale = scale;
    }
}
