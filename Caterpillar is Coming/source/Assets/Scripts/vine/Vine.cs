using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Vine : MonoBehaviour
{
    [SerializeField] private float growSpeed = 1.8f; // 藤曼生长速度，即藤曼头移动速度（调慢更稳）
    [SerializeField] private float pointDistance = 0.1f; // 记录的两个藤曼点之间的距离
    [SerializeField] private float maxTurnAngle = 60; // 最大转向角
    [SerializeField] private float turnSpeed = 90 * Mathf.Deg2Rad; // 每秒可转过的最大弧度，即转向速度
    [SerializeField] private float reboundTurnSpeed = 200f; // 贴墙滑开时每秒可转过的角度（回弹柔和度）
    [SerializeField] private GameObject leafPrefab; // 叶子预制体
    [SerializeField] private float leafHeightInterval = 0.9f; // 藤曼头每向上爬高这么多，尝试生成一片叶子（叶子垂直间距）
    [SerializeField] private float leafMinSpacing = 0.6f; // 新叶子距离已有叶子的最小间距，防止回头/反射路径重复叠叶
    [SerializeField] private float wallSideCheckDist = 1.5f; // 判断藤蔓是否紧贴左右墙的探测距离，用于决定叶子朝哪边长
    [SerializeField] private float steerLockTime = 0.35f; // 撞墙后强制反弹的时长，期间不接受转向，防止顶着墙卡死
    [SerializeField] private float maxStep = 0.04f; // 藤蔓头每帧分小步移动的长度，防止高速/掉帧时一步穿进墙里
    [SerializeField] private LayerMask wallLayer = 1 << 13; // 墙所在的层，默认 Wall（第13层）
    [SerializeField] private float wallProbePad = 0.06f; // 探测墙的提前量，防止藤曼头穿进墙

    private Transform vineHead; // 藤曼头
    private Vector3 growDirection = Vector2.up; // 藤曼生长方向
    private Vector3 initialDirection = Vector2.up; // 藤曼生长初始方向
    private List<Vector3> pointList = new List<Vector3>(); // 藤曼点位置列表
    private Vector3 lastPoint; // 记录的上一个藤蔓点的位置
    private LineRenderer lineRenderer; // 用来连点成线的
    private Vector3 targetDirection; // 藤曼经过限制后的目标方向
    private bool leftleaf = true; // 叶子是否生成在左侧
    private float lastLeafHeight; // 上次生成叶子时藤蔓头的高度
    private List<Vector3> leafPoints = new List<Vector3>(); // 已生成叶子的位置，用来防叠
    private ContactFilter2D wallFilter; // 只探测墙、且忽略触发器（终点门等）的过滤器
    private RaycastHit2D[] wallProbe = new RaycastHit2D[1]; // 复用的探测结果缓存
    private float steerLockTimer; // 撞墙反弹锁定倒计时
    private Collider2D[] overlapCache = new Collider2D[1]; // 复用的重叠检测缓存

    // Start is called before the first frame update
    void Start()
    {
        vineHead = GameObject.Find("Head").GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();

        initialDirection = growDirection; // 记录藤曼生长初始方向
        lastLeafHeight = vineHead.position.y; // 从藤蔓头当前高度开始算叶子高度间隔

        // 反射探测只认墙这一层，并且不把触发器（比如终点门）当成墙
        wallFilter.useTriggers = false;
        wallFilter.SetLayerMask(wallLayer);

        // 兜底：万一开局藤蔓头就放在墙里，先把它顶出来
        PushOutOfWall();

        DrawLine();
    }

    // Update is called once per frame
    void Update()
    {
        VineGrow();
    }

    // 藤曼生长
    private void VineGrow()
    {
        UpdateGrowDirection();

        VineHeadMove();

        if (Vector3.Distance(vineHead.position, lastPoint) > pointDistance)
        {
            DrawLine();
        }

        // 藤蔓头实际向上爬高了 leafHeightInterval，就尝试生一片叶子（左右交替、始终水平）
        if (vineHead.position.y - lastLeafHeight >= leafHeightInterval)
        {
            TrySpawnLeaf();
        }
    }
    // 藤曼头移动（撞墙后贴墙向上滑开，做小幅度回弹）
    private void VineHeadMove()
    {
        float remaining = growSpeed * Time.deltaTime;
        int guard = 0;
        bool inContact = false;
        Vector2 contactNormal = Vector2.zero;

        // 分成小步移动，避免高速或掉帧时一步跨进墙里
        while (remaining > 0f && guard < 30)
        {
            guard++;
            float step = Mathf.Min(remaining, maxStep);

            // 在头前方探测墙
            int hitCount = Physics2D.Raycast(vineHead.position, growDirection, wallFilter, wallProbe, step + wallProbePad);
            if (hitCount > 0 && wallProbe[0].collider != null)
            {
                RaycastHit2D hit = wallProbe[0];
                Vector2 normal = hit.normal; // 墙面的外法线（指向藤蔓头这一侧）

                // 关键：不把藤蔓头摆到任何命中点上（那会导致拐角处瞬移/穿墙）。
                // 只去掉“朝墙里钻”的分量，剩下的位移贴着墙走。
                Vector2 dir2 = (Vector2)growDirection;
                float intoWall = Vector2.Dot(dir2, -normal); // >0 表示正在朝墙里走
                Vector2 move2 = (intoWall > 0f) ? (dir2 + normal * intoWall) : dir2; // 投影到墙面上
                if (move2.sqrMagnitude < 1e-6f)
                    move2 = UpwardSlideDir(normal); // 完全正撞没有切向时，先朝上贴着墙走

                vineHead.position += (Vector3)move2.normalized * step;

                if (!inContact)
                {
                    inContact = true;
                    contactNormal = normal;
                }
                remaining -= step;
            }
            else
            {
                vineHead.position += (Vector3)growDirection * step;
                remaining -= step;
            }
        }

        // 碰墙期间：把方向朝“继续向上、微微离墙”的目标慢慢转（每帧只转一点 = 小幅度回弹）
        if (inContact)
        {
            RotateAwayFromWall(contactNormal);
            // 接触期间锁定玩家转向，防止鼠标一直顶着墙
            steerLockTimer = Mathf.Max(steerLockTimer, steerLockTime);
        }

        // 兜底：如果头还是陷在墙里（例如开局就在墙内），把它顶出来
        PushOutOfWall();
    }

    // 两条墙面切线里，选“能让藤蔓继续向上”的那条；
    // 撞到的是上/下表面（两条切线都水平）时，选与当前方向更接近的一条
    private Vector2 UpwardSlideDir(Vector2 normal)
    {
        Vector2 t1 = Vector2.Perpendicular(normal);
        Vector2 t2 = -t1;

        if (t1.y > t2.y) return t1;
        if (t2.y > t1.y) return t2;

        if (Vector2.Dot(t1, (Vector2)growDirection) >= Vector2.Dot(t2, (Vector2)growDirection))
            return t1;
        return t2;
    }

    // 碰墙后把生长方向慢慢转向“继续向上 + 微微离墙”，并保证方向永不朝下
    private void RotateAwayFromWall(Vector2 normal)
    {
        Vector2 slide = UpwardSlideDir(normal);
        Vector3 goal = slide;

        // 离墙方向如果朝下（比如撞到头顶的墙），就不加离墙分量，避免藤蔓往下走
        if (normal.y > -0.05f)
            goal += (Vector3)normal * 0.5f;

        goal.z = 0f;
        if (goal.y < 0f) goal.y = 0f;
        goal.Normalize();

        growDirection = Vector3.RotateTowards(growDirection, goal, reboundTurnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
        growDirection.z = 0f;
        ClampUpward();
    }

    // 保证生长方向永不带向下的分量（最少保留一点点向上）
    private void ClampUpward()
    {
        if (growDirection.y < 0.15f)
        {
            growDirection.y = 0.15f;
            growDirection.Normalize();
        }
    }

    // 藤蔓头陷进墙里时，把它顶到最近的墙面外侧
    private void PushOutOfWall()
    {
        int guard = 0;
        while (guard < 30 && Physics2D.OverlapPoint(vineHead.position, wallFilter, overlapCache) > 0)
        {
            guard++;
            Collider2D wall = overlapCache[0];
            Vector2 closest = wall.ClosestPoint(vineHead.position); // 墙面上离藤蔓头最近的点
            Vector2 away = (Vector2)vineHead.position - closest;
            if (away.sqrMagnitude < 1e-6f)
                away = Vector2.up; // 兜底：与墙面完全重合时默认往上顶
            vineHead.position = closest + away.normalized * wallProbePad;
        }

        if (guard > 0)
        {
            // 从墙里出来后暂时不接受转向，避免立刻又被顶回去
            steerLockTimer = Mathf.Max(steerLockTimer, steerLockTime);
            ClampUpward();
        }
    }

    // 绘制藤曼
    private void DrawLine()
    {
        pointList.Add(vineHead.position);
        lastPoint = vineHead.position;

        lineRenderer.positionCount = pointList.Count;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, pointList[i]);
        }
    }

    // 通过鼠标更新藤曼生长方向
    private void UpdateGrowDirection()
    {
        // 撞墙后的强制反弹期间不接受转向输入，避免玩家一直顶着墙把藤蔓卡在墙里
        if (steerLockTimer > 0f)
        {
            steerLockTimer -= Time.deltaTime;
            return;
        }
        // 按下或按住鼠标左键时计算目标方向
        bool pressing = Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);
        if (pressing)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 获取鼠标世界坐标
            mouseWorldPos.z = vineHead.position.z; // 锁定鼠标世界坐标的z轴

            Vector3 Direction = (mouseWorldPos - vineHead.position).normalized; // 更新并单位化目标方向
            float angle = Vector2.SignedAngle(initialDirection, Direction); // 计算目标方向与初始方向之间夹角
            angle = Mathf.Clamp(angle, -maxTurnAngle, maxTurnAngle); // 若夹角数值大于限制的最大角度，将其值修改为限制的最大角度

            targetDirection = Quaternion.AngleAxis(angle, Vector3.forward) * initialDirection; // 确定经过限制的目标方向
        }
        else
        {
            // 没有鼠标输入时，默认慢慢回到“向上”，保证藤蔓始终往上爬
            targetDirection = initialDirection;
        }

        growDirection = Vector3.RotateTowards(growDirection, targetDirection, turnSpeed * Time.deltaTime, 0); // 藤曼生长方向逐渐靠向目标方向，每帧计算
        ClampUpward();
    }

    // 判断当前爬升高度处该不该生叶子（带防叠检查 + 贴墙时只往空旷侧长）
    private void TrySpawnLeaf()
    {
        Vector3 anchor = vineHead.position;

        // 防叠：如果这个位置附近已经长过叶子（例如反射后沿原路折返），就先不长，
        // 等藤蔓头继续爬高、离旧叶子足够远时再长，保证不会原地重复叠叶。
        for (int i = 0; i < leafPoints.Count; i++)
        {
            if (Vector2.Distance(leafPoints[i], anchor) < leafMinSpacing)
                return;
        }

        // 决定这一片叶子朝哪边长：
        // 紧贴右墙 → 全往左长；紧贴左墙 → 全往右长；两侧都空 → 左右交替
        float distLeft = SideClearance(Vector2.left);
        float distRight = SideClearance(Vector2.right);

        bool spawnLeft;
        if (distRight < wallSideCheckDist && distLeft >= wallSideCheckDist)
            spawnLeft = true;   // 右边紧贴墙：叶子只长到左边
        else if (distLeft < wallSideCheckDist && distRight >= wallSideCheckDist)
            spawnLeft = false;  // 左边紧贴墙：叶子只长到右边
        else
        {
            spawnLeft = leftleaf; // 没贴墙：左右交替
            leftleaf = !leftleaf;
        }

        LeafGrow(spawnLeft);
        lastLeafHeight = anchor.y;
        leafPoints.Add(anchor);
    }

    // 从藤蔓头沿某个水平方向探测，返回到墙面的距离；没有墙时返回一个很大的值
    private float SideClearance(Vector2 side)
    {
        int hitCount = Physics2D.Raycast(vineHead.position, side, wallFilter, wallProbe, wallSideCheckDist * 2f);
        if (hitCount > 0 && wallProbe[0].collider != null)
            return wallProbe[0].distance;
        return Mathf.Infinity;
    }

    // 生成叶子：叶子始终水平放置（世界坐标里不旋转），
    // 把叶柄连接点(AttachmentPoint)对准藤蔓当前生长点，叶片朝世界左右两侧交替伸出。
    // 这样不管藤蔓本身是什么角度，长出来的叶子都是水平的平台。
    private void LeafGrow(bool left)
    {
        Vector3 anchor = vineHead.position; // 叶子连接点：藤蔓头当前位置

        // 水平实例化（rotation 为 0），镜像方向用 scale.x 控制
        GameObject leaf = Instantiate(leafPrefab, anchor, Quaternion.identity);
        Vector3 scale = leaf.transform.localScale;
        scale.x = left ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x); // 左/右镜像，让叶片分别朝世界左右伸出
        leaf.transform.localScale = scale;

        // 把叶柄连接点对准藤蔓的生长点，而不是把图片中心放在那里
        Transform attachment = leaf.transform.Find("AttachmentPoint");
        if (attachment != null)
        {
            leaf.transform.position += anchor - attachment.position;
        }

        // 泰拉瑞亚式单向平台：角色从下往上跳可以穿过叶子，从上往下落能踩住
        // （具体逻辑在 OneWayLeaf 里，每帧根据角色脚底位置和速度切换碰撞）
        if (leaf.GetComponent<OneWayLeaf>() == null)
            leaf.AddComponent<OneWayLeaf>();
    }
}
