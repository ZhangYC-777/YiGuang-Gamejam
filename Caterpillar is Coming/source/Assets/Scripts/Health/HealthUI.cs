using UnityEngine;
using UnityEngine.UI;

//简单的生命值UI，数字显示玩家剩余血量
public class HealthUI : MonoBehaviour
{
    //显示血量的文本
    [SerializeField]
    private Text healthText;
    //血量显示前缀
    [SerializeField]
    private string prefix = "HP: ";
    //玩家的健康组件
    private Health playerHealth;

    void OnEnable()
    {
        FindPlayer();
        if (playerHealth != null)
        {
            playerHealth.HealthChanged += OnHealthChanged;
        }
    }

    void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= OnHealthChanged;
        }
    }

    //查找玩家并初始化显示
    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            //初始显示满血
            OnHealthChanged(playerHealth.MaxHealth, playerHealth.MaxHealth);
        }
        else
        {
            Debug.LogWarning("HealthUI: 未找到玩家");
        }
    }

    //血量变化时更新显示
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = prefix + currentHealth + " / " + maxHealth;
        }
    }
}
