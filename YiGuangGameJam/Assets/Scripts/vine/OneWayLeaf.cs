using UnityEngine;

// 泰拉瑞亚式单向叶子平台：
// 角色从下面往上跳时可以从叶子中间穿过；从上面落下来时可以踩在叶子上。
// 原理：每物理帧判断一次，只要“角色脚底还在叶子顶面以下”或“角色还在向上飞”，
// 就让叶子和角色忽略碰撞；等角色开始下落时再恢复碰撞，就能稳稳踩住。
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
        float leafTop = leafCol.bounds.max.y;    // 叶子顶面高度
        float feetY = playerCol.bounds.min.y;    // 角色脚底高度

        // 1) 脚底在叶子顶面以下：从下面跳、在叶子下方走动都不挡
        if (feetY < leafTop - 0.05f)
            return true;

        // 2) 正在向上飞（穿过叶子的过程还没结束）：也不挡，等开始下落再踩住
        if (playerRb.velocity.y > 0.2f)
            return true;

        return false; // 已经越过顶面且开始下落/站住 → 叶子变实心，可以踩
    }
}
