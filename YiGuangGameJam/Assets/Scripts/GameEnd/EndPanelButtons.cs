using UnityEngine;
using UnityEngine.SceneManagement;

// 结束面板按钮逻辑：重新游玩 / 下一关（最后一关回主界面）
// 挂在与 GameEnd / PlayerDie 相同的物体上，由按钮 OnClick 调用
public class EndPanelButtons : MonoBehaviour
{
    // 重新游玩当前关卡
    public void RestartLevel()
    {
        //恢复游戏时间
        Time.timeScale = 1f;
        //重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 前往下一关；若当前已是最后一关则回到主界面
    public void NextLevel()
    {
        //恢复游戏时间
        Time.timeScale = 1f;
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            //最后一关，回到主界面（Build Index 0）
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(nextIndex);
        }
    }
}
