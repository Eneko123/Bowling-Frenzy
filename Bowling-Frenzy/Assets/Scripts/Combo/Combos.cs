using UnityEngine;
using UnityEngine.UI;   
using TMPro;
public class Combos : MonoBehaviour
{
    public int currentComboCount = 0;
    public float comboResetTime = 10f; // Time in seconds to reset the combo
    private float comboTimer = 0f;
    public Image ComboBar;
    public Image ComboBarFill;
    public TextMeshProUGUI ComboText;
    public static Combos Instance;
    private float ComboSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector2 XLimit = new Vector2(-600f, 600f);
    private Vector2 YLimit = new Vector2(-600f, -130f);
    void Start()
    {
        ComboBar.GetComponent<Image>();
        ComboBarFill.GetComponent<Image>();
        ComboBar.gameObject.SetActive(false);
    }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {

        if(currentComboCount > 0)
        {
            comboTimer += Time.deltaTime * ComboSpeed;
            ComboBarFill.fillAmount = 1 - (comboTimer / comboResetTime);
            if (comboTimer >= comboResetTime)
            {
                ResetCombo();
            }
        }
    }
     void ResetCombo()
    {
        currentComboCount = 0;
        comboTimer = 0f;
        ComboBar.gameObject.SetActive(false);
    }
    public void IncrementCombo()
    {
        if(currentComboCount == 0)
        {
            ComboBar.gameObject.SetActive(true);
        }
        comboTimer = 0f; // Reset the timer whenever a new combo is started
        currentComboCount++;
        ComboBarFill.fillAmount = 1;
        ComboText.text = "x" + currentComboCount.ToString();
        ComboText.rectTransform.anchoredPosition = new Vector2(Random.Range(XLimit.x, XLimit.y), Random.Range(YLimit.x, YLimit.y));
        if (currentComboCount % 5 == 0 && currentComboCount <= 40)
        {
            ComboSpeed += 0.75f;// Reduce the reset time by 1 second for every 5 combos, up to a maximum of 40 combos
            ComboText.text = "x" + currentComboCount.ToString() + "!!";
        }
        if(currentComboCount % 10 == 0)
        {
            MainCharacter.Instance.RestoreHealthByCombo();
        }
    }
}
