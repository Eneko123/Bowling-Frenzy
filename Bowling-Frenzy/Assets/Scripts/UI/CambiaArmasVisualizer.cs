using UnityEngine;
using UnityEngine.UI;

public class CambiaArmasVisualizer : MonoBehaviour
{
    public GameObject ExploActive;
    public GameObject SlowActive;
    public GameObject PierceActive;

    int currentSpecialIndex = 0;

    // Llamado desde MainCharacter cuando pulsa 1/2/3
    public void UpdateActive(int currentSpecial)
    {
        currentSpecialIndex = currentSpecial;
        ApplyVisual(currentSpecialIndex);
    }

    private void ApplyVisual(int index)
    {
        switch (index)
        {
            case 0:
                ExploActive.SetActive(false);
                SlowActive.SetActive(false);
                PierceActive.SetActive(false);
                break;
            case 1:
                ExploActive.SetActive(true);
                SlowActive.SetActive(false);
                PierceActive.SetActive(false);
                break;
            case 2:
                ExploActive.SetActive(false);
                SlowActive.SetActive(true);
                PierceActive.SetActive(false);
                break;
            case 3:
                ExploActive.SetActive(false);
                SlowActive.SetActive(false);
                PierceActive.SetActive(true);
                break;
        }
    }
}
