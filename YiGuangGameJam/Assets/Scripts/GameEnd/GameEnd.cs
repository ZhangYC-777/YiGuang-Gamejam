using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        //通关了：确保能点按钮，并显示“下一关”按钮
        EnsureEventSystem();
        AddNextLevelButton();
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

    //是否还有下一关（关卡按 Build Settings 里的顺序排）
    private bool HasNextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        return index >= 0 && index + 1 < SceneManager.sceneCountInBuildSettings;
    }

    //进入下一关
    private void NextLevel()
    {
        //恢复游戏时间
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next);
    }

    //按钮点击需要 EventSystem；关卡场景里没有，现场补一个
    private void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;

        GameObject go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    //在通关面板上创建一个“下一关”按钮（最后一关没有下一关就不创建）
    private void AddNextLevelButton()
    {
        if (!HasNextLevel())
            return;

        //防止重复创建
        if (gameEndPanel.transform.Find("NextLevelButton") != null)
            return;

        Canvas canvas = gameEndPanel.GetComponentInParent<Canvas>();
        if (canvas == null)
            return; //没有画布就没法放UI按钮

        //关键：Canvas 上必须要有 GraphicRaycaster，EventSystem 才能点到 UI；
        //你们的场景 Canvas 上只有 CanvasScaler、缺 GraphicRaycaster，这里自动补上。
        if (canvas.GetComponent<GraphicRaycaster>() == null)
            canvas.gameObject.AddComponent<GraphicRaycaster>();

        Font font = FindUIFont();

        //按钮本体
        GameObject btnGO = new GameObject("NextLevelButton", typeof(RectTransform), typeof(Image), typeof(Button));
        btnGO.transform.SetParent(gameEndPanel.transform, false);
        btnGO.transform.SetAsLastSibling(); //放到最上层，别被面板背景挡住
        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.22f);
        rt.anchorMax = new Vector2(0.5f, 0.22f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(280f, 64f);

        Image img = btnGO.GetComponent<Image>();
        img.color = new Color(0.22f, 0.60f, 0.30f, 1f); //绿色按钮

        Button button = btnGO.GetComponent<Button>();
        button.onClick.AddListener(NextLevel);

        //按钮文字
        GameObject txtGO = new GameObject("Text", typeof(RectTransform), typeof(Text));
        txtGO.transform.SetParent(btnGO.transform, false);
        RectTransform trt = txtGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;

        Text text = txtGO.GetComponent<Text>();
        text.text = "下一关";
        text.fontSize = 34;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        if (font != null)
        {
            text.font = font;
        }
        else
        {
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 34); //兜底字体
        }
    }

    //从场景里现有的UI文字复制字体，保证中文能正常显示
    private Font FindUIFont()
    {
        Text[] all = Resources.FindObjectsOfTypeAll<Text>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].font != null && all[i].gameObject.scene.IsValid())
                return all[i].font;
        }
        return null;
    }
}
