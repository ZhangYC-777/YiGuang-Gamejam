using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    [SerializeField] private float growSpeed = 3f; // 藤曼生长速度，即tomato移动速度
    [SerializeField] private Vector2 growDirection = Vector2.up; // 藤曼生长方向

    [SerializeField] private GameObject vineHead; // tomato

    // Start is called before the first frame update
    void Start()
    {
        vineHead = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        vineHead.transform.position += (Vector3)(growSpeed * growDirection * Time.deltaTime);
    }
}
