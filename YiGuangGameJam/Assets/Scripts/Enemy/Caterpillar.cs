using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpillar : MonoBehaviour
{
    //声明毛毛虫的寿命
    [SerializeField]
    private float lifetime = 6f;
    //声明毛毛虫掉落时的最大翻滚速度
    [SerializeField]
    private float maxTumbleSpeed = 300f;
    //声明毛毛虫掉落时的水平漂移速度
    [SerializeField]
    private float driftSpeed = 1f;
    //声明毛毛虫的Rigidbody组件
    private Rigidbody2D caterpillarRigidbody;
    void Start()
    {
        caterpillarRigidbody = GetComponent<Rigidbody2D>();
        //给毛毛虫一个随机的翻滚角速度，掉落时不停翻滚
        caterpillarRigidbody.angularVelocity = Random.Range(-maxTumbleSpeed, maxTumbleSpeed);
        //给毛毛虫一个随机的水平漂移速度，让掉落轨迹更自然
        caterpillarRigidbody.velocity = new Vector2(Random.Range(-driftSpeed, driftSpeed), caterpillarRigidbody.velocity.y);
    }
    //处理伤害逻辑
    void OnTriggerEnter2D(Collider2D other)
    {
        //检测毛毛虫碰撞的对象是否有IDamageable接口
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            //调用受伤方法
            damageable.TakeDamage(1);
            //命中可受伤对象后销毁毛毛虫
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //处理毛毛虫的寿命
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}