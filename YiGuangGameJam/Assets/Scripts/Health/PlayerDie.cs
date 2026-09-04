using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : MonoBehaviour
{
    //声明游戏结束的UI面板
    [SerializeField]
    private GameObject gameOverPanel;
    //声明玩家的健康组件
    private Health playerHealth;
    //声明玩家是否已经死亡
    private bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        //开始游戏时隐藏死亡面板
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //实现按下R键重新加载场景
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            //恢复游戏时间
            Time.timeScale = 1f;
            //重新加载当前场景
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    //实现订阅
    void OnEnable()
    {
        FindPlayer();
        if(playerHealth != null)
        {
            playerHealth.Died += Die;
        }
    }
    //实现退订操作
     void OnDisable()
    {
        if(playerHealth != null)
        {
            playerHealth.Died -= Die;
        }
    }
    //声明一个方法去处理玩家死亡
    public void Die()
    {
        //防止重复处理死亡
        if(isDead)
        {
            return;
        }
        isDead = true;
        //停止玩家移动，保留对象供摄像机和敌人引用
        playerHealth.GetComponent<PlayerMove>().enabled = false;
        //显示游戏结束面板
        ShowGameOverPanel();
        //暂停游戏
        Time.timeScale = 0f;    
    }
     private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerHealth = player.GetComponent<Health>();
            Debug.Log("找到了玩家");
        }
        else
        {
            Debug.Log("未找到玩家");
            return;
        }
    }
    //声明一个方法去显示游戏结束的UI面板
    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
}
