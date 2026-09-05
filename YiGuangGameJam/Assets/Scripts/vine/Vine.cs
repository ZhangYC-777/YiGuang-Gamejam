using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    [SerializeField] private float growSpeed = 3f; // 藤曼生长速度，即藤曼头移动速度
    [SerializeField] private Vector2 growDirection = Vector2.up; // 藤曼生长方向
    [SerializeField] private float pointDistance = 1.0f; // 记录的两个藤曼点之间的距离
    [SerializeField] private List<Vector3> pointList; // 藤曼点位置列表

    private Transform vineHead; // 藤曼头
    private Vector3 lastPoint; // 记录的上一个藤蔓点的位置
 
    // Start is called before the first frame update
    void Start()
    {
        vineHead = GameObject.Find("Head").GetComponent<Transform>();
        pointList.Add(vineHead.position);
        lastPoint = vineHead.position;
    }

    // Update is called once per frame
    void Update()
    {
        vineHead.transform.position += (Vector3)(growSpeed * growDirection * Time.deltaTime);

        if (Vector3.Distance(vineHead.position, lastPoint) > pointDistance)
        {
            pointList.Add(vineHead.position);
            lastPoint = vineHead.position;
            Debug.Log("aaa");
        }
    }
}
