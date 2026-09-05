using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    //声明结束游戏的UI面板
    [SerializeField]
    private GameObject gameEndPanel;
    //声明玩家对象
    [SerializeField]
    private GameObject player;
    //声明游戏是否结束
    private bool isEnded = false;
    void Start()
    {
        gameEndPanel.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        //实现按下R键重新加载场景
        if (isEnded && Input.GetKeyDown(KeyCode.R))
        {
            //恢复游戏时间
            Time.timeScale = 1f;
            //重新加载当前场景
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    //处理玩家进入范围结束游戏
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EndGame();
        }
    }
    //声明一个方法去展示游戏结束的UI面板
    public void ShowGameEndPanel()
    {
        gameEndPanel.SetActive(true);
    }
     public void EndGame()
    {
        //防止重复处理游戏结束
        if(isEnded)
        {
            return;
        }
        isEnded = true;
        //停止玩家移动，保留对象供摄像机和敌人引用
        player.GetComponent<PlayerMove>().enabled = false;
        //显示游戏结束面板
        ShowGameEndPanel();
        //暂停游戏
        Time.timeScale = 0f;    
    }
}
