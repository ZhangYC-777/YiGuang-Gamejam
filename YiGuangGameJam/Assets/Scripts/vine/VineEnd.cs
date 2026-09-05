using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineEnd : MonoBehaviour
{
    //声明游戏中控，在Inspector中拖入GameManager
    public gameManager manager;
    //声明终点门的触发器，在Inspector中拖入GameFlow
    public Collider2D endDoor;
    //声明藤蔓是否已经到达终点
    private bool isFinished = false;

    //检测藤蔓头进入终点门
    private void OnTriggerEnter2D(Collider2D other)
    {
        //只处理终点门，并防止重复通知
        if (isFinished || other != endDoor)
            return;

        isFinished = true;
        //通知中控结束生长，切换到玩家移动状态
        manager.GrowthFinished();
    }
}
