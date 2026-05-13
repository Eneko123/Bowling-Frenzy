using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SpecialBulletCooldownUI : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private SpecialBullets bulletType;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private GameObject readyIndicator;

    [Header("Colores")]
    [SerializeField] private Color coolingColor = new Color(1f, 0.3f, 0.3f, 0.8f);
    [SerializeField] private Color readyColor = new Color(0.3f, 1f, 0.3f, 1f);

    private Coroutine cooldownCoroutine;
    public bool IsReady { get; private set; } = true;
    public SpecialBullets BulletType => bulletType;

    void Awake() => SetReady();

    // Llamado desde un gestor central
    public void StartCooldown(float duration)
    {
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        IsReady = false;
        cooldownCoroutine = StartCoroutine(RunCooldownCoroutine(duration));
    }

    private IEnumerator RunCooldownCoroutine(float duration)
    {
        float elapsed = 0f;
        cooldownImage.color = coolingColor;
        readyIndicator?.SetActive(false);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float remaining = Mathf.Max(0f, duration - elapsed);

            cooldownImage.fillAmount = progress;
            cooldownText.text = $"{remaining:F1}s";

            yield return null;
        }

        SetReady();
    }

    public void SetReady()
    {
        IsReady = true;
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
            cooldownImage.color = readyColor;
        }
        if (cooldownText != null)
            cooldownText.text = "LISTO";
        if (readyIndicator != null)
            readyIndicator.SetActive(true);
    }
}