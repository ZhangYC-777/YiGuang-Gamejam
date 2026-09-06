using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public bool isRun;
    public bool isJump;
    public bool isGround;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float force;

    private Rigidbody2D rb;
    private float moveController;
    private int groundContacts; // 脚下接触的地面/叶子数量，由碰撞回调维护（代替原来的向下射线）

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 连续碰撞检测：高速下落/上跳时不会一步穿过叶子等薄平台
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Jump();
    }

    private void FixedUpdate()
    {
        if (isJump)
            rb.AddForce(Vector2.down * force);

        // 每物理帧根据“脚下接触 + 速度方向”判定着地：
        // 只要碰着地面/叶子、并且没有在往上飞就算着地。
        // 这样站在叶子边缘、站在窄平台上也能正常起跳（不再依赖向下的射线）。
        isGround = groundContacts > 0 && rb.velocity.y <= 0.5f;
    }

    private void Run()
    {
        moveController = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveSpeed * moveController, rb.velocity.y);
        if (moveController > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        if (moveController < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        isRun = (moveController != 0);
    }

    private void Jump()
    {
        // 着地判定由脚下碰撞回调更新，这里不再做射线检测
        if (Input.GetButtonDown("Jump") && isGround)
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

        // 上升中且不在着地 → 跳跃状态；否则是下落/待机
        if (rb.velocity.y > 0 && !isGround)
            isJump = true;
        else
            isJump = false;
    }

    // 脚下碰到地面/叶子时记录接触（相当于脚下触发器）
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
            groundContacts++;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
            groundContacts = Mathf.Max(0, groundContacts - 1);
    }

    // 判断对象是否属于“可以站立的地面”（地面、叶子所在的层）
    private bool IsGroundLayer(GameObject go)
    {
        return ((1 << go.layer) & groundLayer.value) != 0;
    }
}
