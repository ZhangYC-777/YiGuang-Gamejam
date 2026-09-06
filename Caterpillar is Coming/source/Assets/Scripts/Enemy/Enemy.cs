using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //声明敌人的移动速度
    [SerializeField]
    private float enemySpeed = 5f;
    //声明敌人的移动方向
    private Vector3 moveDirection = Vector3.right;
    //声明敌人的Rigidbody组件
    private Rigidbody2D enemyRigidbody;
    //声明敌人的寿命
    [SerializeField]
    private float enemyLifetime = 5f;
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyRigidbody.velocity = moveDirection * enemySpeed;
    }
    //处理伤害逻辑
     void OnTriggerEnter2D(Collider2D other)
    {
        //检测敌人碰撞的对象是否有IDamageable接口
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            //调用受伤方法
            damageable.TakeDamage(1);
            //命中可受伤对象后销毁敌人
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //处理敌人的生命
        enemyLifetime -= Time.deltaTime;
        if (enemyLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
