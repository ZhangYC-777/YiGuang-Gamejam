using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//设立两个状态
enum GameState
{
    Growing, //藤蔓生长
    PlayerMoving //玩家移动
}
public class gameManager : MonoBehaviour
{
   //设立当前的状态
    private GameState currentState = GameState.Growing;
    //声明玩家对象
    public GameObject player;
    //声明藤蔓对象
    public GameObject vine;
    //声明另一个藤蔓生长脚本，在Inspector中赋值
    public VineGrow vineGrow;
    //声明敌人中控
    public GameObject enemyManager;
    //声明虚拟摄像机，在Inspector中拖入PlayerCamera
    public CinemachineVirtualCamera mainCamera;
    void Start()
    {
        //获取玩家对象
        player = GameObject.FindGameObjectWithTag("Player");
        //获取藤蔓对象
        vine = GameObject.FindGameObjectWithTag("Vine");
        //获取敌人中控对象
        enemyManager = GameObject.FindGameObjectWithTag("Enemy");
        //初始状态玩家不可移动
        player.GetComponent<PlayerMove>().enabled = false;
        //初始状态敌人中控失活
        enemyManager.SetActive(false);
    }

    //声明一个公共方法，供藤蔓生长结束时调用
    public void GrowthFinished()
    {
        ChangeState(GameState.PlayerMoving);
    }

    //声明一个方法去改变状态
    private void ChangeState(GameState newState)
    {
        //相同状态不重复切换
        if (currentState == newState)
            return;

        currentState = newState;
        //根据当前状态执行不同的逻辑
        switch (currentState)
        {
            case GameState.Growing:


               
                break;
            case GameState.PlayerMoving:
                //玩家移动状态下，启用玩家移动脚本
                player.GetComponent<PlayerMove>().enabled = true;
                //启用敌人中控
                enemyManager.SetActive(true);
                //禁用另一个对象上的藤蔓生长脚本
                if (vineGrow != null)
                    vineGrow.enabled = false;
                //禁用藤蔓生长脚本
                vine.GetComponent<Vine>().enabled = false;
                //改变摄像机的跟随目标为玩家
                mainCamera.Follow = player.transform;

                
                break;
        }
    }


}
