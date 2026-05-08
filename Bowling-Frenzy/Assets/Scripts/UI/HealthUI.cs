using UnityEngine;
using UnityEngine.UI;  
using TMPro;
public class HealthUI : MonoBehaviour
{
    public Image healthbarBackground;
    public Image healthbarFill;
    public TextMeshProUGUI healthText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthbarBackground.GetComponent<Image>();
        healthbarFill.GetComponent<Image>();
    }

    // Update is called once per frame
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthbarFill.fillAmount = currentHealth / maxHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}
