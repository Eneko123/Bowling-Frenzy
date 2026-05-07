using UnityEngine;
using UnityEngine.UI;

public class CooldownIndicator : MonoBehaviour
{
    public Image CooldownReloader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CooldownReloader.GetComponent<Image>();
    }

    // Update is called once per frame
    public void UpdateCooldown(float cooldown)
    {
        CooldownReloader.fillAmount = cooldown;
        if(CooldownReloader.fillAmount = 1)
        {
            CooldownReloader.fillAmount = 0;
        }
    }
}
