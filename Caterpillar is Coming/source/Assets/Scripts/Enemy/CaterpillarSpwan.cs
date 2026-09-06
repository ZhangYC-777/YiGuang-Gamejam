using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaterpillarSpwan : MonoBehaviour
{
    //声明玩家对象
    [SerializeField]
    private GameObject player;
    //声明毛毛虫预制体
    [SerializeField]
    private GameObject caterpillarPrefab;
    //声明每波毛毛虫生成的间隔时间
    [SerializeField]
    private float spawnInterval = 2.5f;
    //毛毛虫下次生成的时间
    [SerializeField]
    private float nextSpawnTime = 0f;
    //声明每波生成的毛毛虫数量
    [SerializeField]
    private int spawnCountPerWave = 3;
    //声明毛毛虫生成时高于玩家的高度
    [SerializeField]
    private float spawnHeightAbovePlayer = 9f;
    //声明每一层毛毛虫之间的高度差，让掉落错开有层次
    [SerializeField]
    private float layerHeightGap = 2.5f;
    //声明相邻毛毛虫之间的水平间距，中间一只对准玩家，左右各一只对称分布
    [SerializeField]
    private float horizontalSpacing = 4.2f;
    void Start()
    {
        //获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player");
        nextSpawnTime = Time.time + spawnInterval;

    }

    // Update is called once per frame
    void Update()
    {
        //检查是否到了生成毛毛虫的时间
        if (Time.time >= nextSpawnTime)
        {

            //生成一波毛毛虫雨
            SpwanWave();
            //更新下次生成的时间
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    //声明一个函数生成一波毛毛虫雨
    private void SpwanWave()
    {
        //获取玩家的位置
        Vector3 playerPosition = player.transform.position;
        //逐个生成毛毛虫，每一只比上一只更高，掉落时间错开形成层次
        for (int i = 0; i < spawnCountPerWave; i++)
        {
            //以玩家为中心计算水平偏移，中间那只对准玩家，左右两侧对称分布
            float offsetX = (i - spawnCountPerWave / 2) * horizontalSpacing;
            //每一层比上一层更高，保证掉落先后有序
            float spawnY = playerPosition.y + spawnHeightAbovePlayer + layerHeightGap * i;
            //生成毛毛虫
            Instantiate(caterpillarPrefab, new Vector3(playerPosition.x + offsetX, spawnY, 0), Quaternion.identity);
        }
    }
}