using System.Collections;
using UnityEngine;

//受伤后变红提示，默认持续两秒
[RequireComponent(typeof(Health))]
public class DamageFlash : MonoBehaviour
{
    //变红持续时间
    [SerializeField]
    private float flashDuration = 2f;
    //变红颜色
    [SerializeField]
    private Color flashColor = Color.red;
    //健康组件
    private Health health;
    //需要变色的所有精灵渲染器（含子物体）
    private SpriteRenderer[] renderers;
    //原始颜色缓存
    private Color[] originalColors;
    //当前变红协程
    private Coroutine flashCoroutine;

    void Awake()
    {
        health = GetComponent<Health>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color;
        }
    }

    void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
    }

    //血量变化时触发变红
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetColor(flashColor);
        //使用真实时间，死亡暂停时也能正确恢复
        yield return new WaitForSecondsRealtime(flashDuration);
        RestoreColor();
        flashCoroutine = null;
    }

    private void SetColor(Color color)
    {
        foreach (var r in renderers)
        {
            if (r != null)
            {
                r.color = color;
            }
        }
    }

    private void RestoreColor()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].color = originalColors[i];
            }
        }
    }
}
