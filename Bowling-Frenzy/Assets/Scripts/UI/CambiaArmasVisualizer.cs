using UnityEngine;
using UnityEngine.UI;

public class CambiaArmasVisualizer : MonoBehaviour
{
    public GameObject ExploActive;
    public GameObject SlowActive;
    public GameObject PierceActive;
    public GenerateBullet mejoras;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
public void UpdateActive(SpecialBullets current)
    {
        switch(current)
        {
            case SpecialBullets.None:
                ExploActive.SetActive(false);
                SlowActive.SetActive(false);
                PierceActive.SetActive(false);
                break;
            case SpecialBullets.Explosive:
                ExploActive.SetActive(true);
                SlowActive.SetActive(false);
                PierceActive.SetActive(false);
                break;
            case SpecialBullets.Piercing:
                ExploActive.SetActive(false);
                SlowActive.SetActive(false);
                PierceActive.SetActive(true);
                break;
            case SpecialBullets.Slowing:
                ExploActive.SetActive(false);
                SlowActive.SetActive(true);
                PierceActive.SetActive(false);
                break;
        }
    }
}
