using UnityEngine;

// 泰拉瑞亚式单向叶子平台：
// 角色从下面往上跳时可以从叶子中间穿过；从上面落下来时可以踩在叶子上。
// 原理：每物理帧判断一次，只在“明确要放行”时才忽略碰撞：
//   1) 脚在叶子碰撞体下边沿以下（在叶子下方走动/下落）；
//   2) 正在向上飞、且还没越过叶子顶面（穿过过程中）。
// 其余情况（从上方落下来、已经站在顶面附近）都保持实心，保证一定踩得住。
// 判断用整个碰撞体高度做容差，避免在临界帧来回切换导致随机漏踩。
public class OneWayLeaf : MonoBehaviour
{
    private Collider2D leafCol;      // 这片叶子的碰撞体
    private Collider2D playerCol;    // 角色的碰撞体
    private Rigidbody2D playerRb;    // 角色的刚体（用来读速度）

    void OnEnable()
    {
        leafCol = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (leafCol == null)
        {
            leafCol = GetComponent<Collider2D>();
            if (leafCol == null)
                return;
        }

        // 懒加载角色引用
        if (playerCol == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;
            playerCol = player.GetComponent<Collider2D>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }
        if (playerCol == null || playerRb == null)
            return;

        bool ignore = ShouldIgnore(playerCol, playerRb);
        Physics2D.IgnoreCollision(playerCol, leafCol, ignore);
    }

    // 什么时候叶子对角色“透明”（不挡人）
    private bool ShouldIgnore(Collider2D playerCol, Rigidbody2D playerRb)
    {
        Bounds leafBounds = leafCol.bounds;
        float top = leafBounds.max.y;        // 叶子顶面高度
        float bottom = leafBounds.min.y;     // 叶子底面高度
        float feetY = playerCol.bounds.min.y; // 角色脚底高度
        float vy = playerRb.velocity.y;

        bool rising = vy > 0.25f;                    // 正在明显向上飞
        bool feetBelowTop = feetY < top - 0.05f;     // 脚还没越过顶面
        bool feetBelowBottom = feetY < bottom - 0.05f; // 脚已经低于整个碰撞体

        // 只在下面这两种情况下放行（透明）：
        // 1) 脚在碰撞体下方：在叶子下面走动/下落都不挡
        // 2) 正在往上穿、且还没越过顶面：允许从下面跳上来穿过
        return feetBelowBottom || (rising && feetBelowTop);
    }
}
