using UnityEngine;
using UnityEngine.UI;

public class CooldownIndicator : MonoBehaviour
{
    public Image CooldownReloader;
    public float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CooldownReloader.GetComponent<Image>();
        CooldownReloader.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void UpdateCooldown(float cooldown)
    {
        timer += Time.deltaTime;
        CooldownReloader.gameObject.SetActive(true);
        CooldownReloader.fillAmount = timer / cooldown;
        if(CooldownReloader.fillAmount >= 1f)
        {
            CooldownReloader.fillAmount = 0f;
            CooldownReloader.gameObject.SetActive(false);
        }
    }
}
