using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 第一关开局引导牌：竖着的窄长牌子，显示操作说明。
// 只在 Stage_1 出现；R 重开本关也会重新生成。
public static class Stage1Guide
{
    private const string SignName = "Stage1GuideSign";

    // 引导文字（第一关操作说明，长句会自动在牌子里换行堆叠）
    private static readonly string[] GuideLines =
    {
        "按住鼠标左键：\n控制藤蔓生长方向",
        "AD 键：\n控制毛毛虫左右移动",
        "Space 键：\n控制毛毛虫跳跃",
    };

    // 牌子左右位置（相对玩家起点，正数在右边）——想更靠左就调小，负数就在左边
    private const float HorizontalOffset = 2.6f;
    // 牌子底部离地多高
    private const float BottomRaise = 0.4f;
    // 牌子世界宽度（竖牌：窄）
    private const float WorldWidth = 4.7f;

    // 牌子内部布局尺寸（像素，仅用于排版）
    private const float PanelPxW = 640f;
    private const float PanelPxH = 900f;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        // 场景每次加载（包括按 R 重开）后都检查一次
        SceneManager.sceneLoaded += (scene, mode) => EnsureInScene();
        EnsureInScene();
    }

    private static void EnsureInScene()
    {
        if (SceneManager.GetActiveScene().name != "Stage_1")
            return;
        if (GameObject.Find(SignName) != null)
            return; // 已存在，防止重复
        CreateSign();
    }

    private static void CreateSign()
    {
        // ---------- 基准点：玩家起点 ----------
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 basePos = player != null ? player.transform.position : Vector3.zero;

        float scale = WorldWidth / PanelPxW;
        float worldH = PanelPxH * scale; // 牌子世界高度

        // 牌子中心位置：底部在起点上方 BottomRaise 处立起来
        Vector3 pos = basePos + new Vector3(HorizontalOffset, BottomRaise + worldH * 0.5f, 0f);

        // ---------- 创建世界空间 Canvas ----------
        GameObject sign = new GameObject(SignName);
        Canvas canvas = sign.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 20; // 盖在场景贴图上面

        RectTransform rt = sign.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(PanelPxW, PanelPxH);
        sign.transform.localScale = new Vector3(scale, scale, 1f);
        sign.transform.position = pos;

        // 朝向主相机
        Camera cam = Camera.main;
        if (cam != null)
            sign.transform.rotation = cam.transform.rotation;

        // ---------- 半透明底板 ----------
        GameObject bg = new GameObject("Bg", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(sign.transform, false);
        RectTransform brt = bg.GetComponent<RectTransform>();
        brt.anchorMin = Vector2.zero;
        brt.anchorMax = Vector2.one;
        brt.offsetMin = Vector2.zero;
        brt.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.08f, 0.12f, 0.08f, 0.78f);

        // ---------- 文字：三条说明，竖着从上往下排 ----------
        Font font = FindUIFont();
        float xInset = 46f;      // 左右留白
        float yInset = 60f;      // 上下留白
        float rowHeight = (PanelPxH - yInset * 2f) / GuideLines.Length;

        for (int i = 0; i < GuideLines.Length; i++)
        {
            GameObject line = new GameObject("GuideLine" + i, typeof(RectTransform), typeof(Text));
            line.transform.SetParent(sign.transform, false);
            RectTransform lrt = line.GetComponent<RectTransform>();

            // 竖着三等分排布：上面一条，中间一条，下面一条
            float top = 1f - yInset / PanelPxH - i * (rowHeight / PanelPxH);
            float bottom = top - rowHeight / PanelPxH;
            lrt.anchorMin = new Vector2(0f, bottom);
            lrt.anchorMax = new Vector2(1f, top);
            lrt.offsetMin = new Vector2(xInset, 0f);
            lrt.offsetMax = new Vector2(-xInset, 0f);

            Text text = line.GetComponent<Text>();
            text.text = GuideLines[i];
            text.fontSize = 54;
            text.lineSpacing = 1.25f;
            text.alignment = TextAnchor.MiddleCenter;
            // 允许自动换行、超高不裁剪（防止两行文字被切掉）
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.color = new Color(1f, 1f, 0.92f, 1f);

            if (font != null)
            {
                text.font = font;
            }
            else
            {
                text.font = Font.CreateDynamicFontFromOSFont(new[] { "Microsoft YaHei", "SimHei", "Arial" }, 54);
            }
        }
    }

    // 从场景里现有的 UI 文字复制字体（优先工程里导入的中文字体）
    private static Font FindUIFont()
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
