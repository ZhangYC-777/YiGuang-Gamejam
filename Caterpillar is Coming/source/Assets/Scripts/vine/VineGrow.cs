using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineGrow : MonoBehaviour
{
    //声明藤曼的生长速度
    [SerializeField]
    private float growthSpeed = 0.3f;
    //声明藤曼的当前高度
    private float currentHeight = 0.0f;
    //声明藤蔓的sprite渲染器
    private SpriteRenderer vineRenderer;
    //声明bool类型的变量isGrowing，表示藤蔓是否正在生长
    private bool isGrowing = true;
    //声明终点的高度
    [SerializeField]
    private float targetHeight;
    //声明叶子预制体
    [SerializeField]
    private GameObject leafPrefab;
    //判断下一个叶子是否生成在左边
    private bool isNextLeafOnLeft = true;
    //声明下一个叶子应该在哪个高度生成
    private float nextLeafHeight = 1f;
    //声明叶子生成的间隔高度
    [SerializeField]
    private float leafSpawnInterval = 0.5f;
    void Start()
    {
        //初始化藤曼高度和获取藤曼的sprite渲染器组件
        vineRenderer = GetComponent<SpriteRenderer>();
        currentHeight = vineRenderer.size.y;

    }

    // Update is called once per frame
    void Update()
    {
        StopGrowing();
        //执行生长逻辑
        if (isGrowing)
        {
            Growing();
        }
        //处理叶子生成逻辑
        while (leafSpawnInterval > 0f && currentHeight >= nextLeafHeight)
        {
            if (isNextLeafOnLeft)
            {
                SpawnLeftLeaf();
            }
            else
            {
                SpawnRightLeaf();
            }
            //更新下一个叶子的生成高度
            nextLeafHeight += leafSpawnInterval;
            //切换下一个叶子的生成位置
            isNextLeafOnLeft = !isNextLeafOnLeft;
        }
    }
    //声明一个公共方法Growing;
    private void Growing()
    {
        //增加当前高度
        currentHeight += growthSpeed * Time.deltaTime;
        //更新藤曼的sprite渲染器的大小
        vineRenderer.size = new Vector2(vineRenderer.size.x, currentHeight);
    }
    //声明一个公共方法StopGrowing，用于停止生长
    public void StopGrowing()
    {
        if (currentHeight >= targetHeight)
        {
            isGrowing = false;
        }
    }
    //声明两个个公共方法SpawnLeaf，用于生成叶子
    //右边叶子
    private void SpawnRightLeaf()
    {
        //表示叶子的生成位置
        //先确定藤蔓局部的右边缘和预定高度，再转换成世界坐标（包含缩放）
        Vector3 spawnPosition = transform.TransformPoint(new Vector3(vineRenderer.size.x / 2, nextLeafHeight - 0.2f, 0f));
        //实例化叶子预制体
        GameObject leaf = Instantiate(leafPrefab, spawnPosition, Quaternion.identity);
        //当前素材叶柄在右边，右侧叶子需要镜像；只修改这次生成的实例
        Vector3 scale = leaf.transform.localScale;
        leaf.transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        //把叶柄连接点移到生成位置，而不是把图片中心放在那里
        Transform attachment = leaf.transform.Find("AttachmentPoint");
        leaf.transform.position += spawnPosition - attachment.position;
    }
    //左边叶子
    private void SpawnLeftLeaf()
    {
        //表示叶子的生成位置
        //与右侧相同，只把局部X改成左边缘
        Vector3 spawnPosition = transform.TransformPoint(new Vector3(-vineRenderer.size.x / 2, nextLeafHeight - 0.2f, 0f));
        //实例化叶子预制体
        GameObject leaf = Instantiate(leafPrefab, spawnPosition, Quaternion.identity);
        //设置叶子实例的x轴方向；当前素材向左伸展，左侧保留正缩放
        Vector3 scale = leaf.transform.localScale;
        leaf.transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        //把叶柄连接点移到生成位置
        Transform attachment = leaf.transform.Find("AttachmentPoint");
        leaf.transform.position += spawnPosition - attachment.position;
    }

}
