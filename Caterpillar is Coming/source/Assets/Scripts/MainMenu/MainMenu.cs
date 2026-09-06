using UnityEngine;
using UnityEngine.SceneManagement;

// 主界面中控：开始新游戏 / 选择关卡 / 退出游戏
public class MainMenu : MonoBehaviour
{
    [Header("面板引用")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject levelSelectPanel;

    private void Start()
    {
        // 保证从暂停状态返回主菜单时时间正常流动
        Time.timeScale = 1f;
        ShowMainPanel();
    }

    // 开始新游戏：加载第一关（Build Index 1）
    public void StartNewGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    // 打开选关面板
    public void ShowLevelSelect()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    // 返回主面板
    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    // 加载指定 Build Index 的关卡
    public void LoadLevel(int buildIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    // 退出游戏
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
