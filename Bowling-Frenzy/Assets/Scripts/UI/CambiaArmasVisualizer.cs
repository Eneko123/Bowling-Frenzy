using UnityEngine;
using UnityEngine.UI;

public class CambiaArmasVisualizer : MonoBehaviour
{
    public GameObject ExploActive;
    public GameObject SlowActive;
    public GameObject PierceActive;

    // Número total de especiales (1, 2, 3). El 0 es "ninguno".
    private const int TotalSpecials = 3;

    int currentSpecialIndex = 0;

    // Llamado desde MainCharacter cuando pulsa 1/2/3
    public void UpdateActive(int currentSpecial)
    {
        currentSpecialIndex = currentSpecial;
        ApplyVisual(currentSpecialIndex);
    }

    // Llamado desde MainCharacter cuando pulsa Tab
    // Cicla: 1 → 2 → 3 → 1 → ...
    // Si no había ninguno seleccionado (0), empieza en 1
    public int CycleLeft()
    {
        int next = (currentSpecialIndex % TotalSpecials) + 1;
        currentSpecialIndex = next;
        ApplyVisual(currentSpecialIndex);
        return currentSpecialIndex; // Devuelve el índice para que MainCharacter actualice su estado
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
