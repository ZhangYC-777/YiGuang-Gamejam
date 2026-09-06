using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpwan : MonoBehaviour
{
    //声明玩家对象
    [SerializeField]
    private GameObject player;
    //声明敌人预制体
    [SerializeField]
    private GameObject enemyPrefab;
    //声明敌人生成的时间间隔
    [SerializeField]
    private float spawnInterval = 3f;
    //敌人下次生成的时间
    [SerializeField]
    private float nextSpawnTime = 0f;
    //声明摄像机对象
    [SerializeField]
    private Camera cam;
    void Start()
    {
        //获取玩家的位置
        player = GameObject.FindGameObjectWithTag("Player");
        nextSpawnTime = Time.time + spawnInterval;

    }

    // Update is called once per frame
    void Update()
    {
        //检查是否到了生成敌人的时间
        if (Time.time >= nextSpawnTime)
        {

            //生成敌人
            Instantiate(enemyPrefab, SpwanPosition(), Quaternion.identity);
            //更新下次生成的时间
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    //声明一个函数计算敌人生成的位置
    private Vector3 SpwanPosition()
    {
        //获取玩家的位置
        Vector3 playerPosition = player.transform.position;
        //计算敌人生成的位置
        Vector3 enemyPosition = new Vector3(cam.transform.position.x - cam.orthographicSize * cam.aspect - 1f, playerPosition.y, 0);
        return enemyPosition;
    }
}
