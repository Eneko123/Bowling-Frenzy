using UnityEngine;
using UnityEngine.UI;

public class CambiaArmasVisualizer : MonoBehaviour
{
    public GameObject[] Selectors;
    int currentSpecialIndex = 0;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTabPressed()
    {
        for(int i = 0; i < GenerateBullet.instance.specialBullets.Count; i++)
        {
            Debug.Log((GenerateBullet.instance.specialBullets[i]));
        }
        Selectors[currentSpecialIndex].SetActive(false);
        currentSpecialIndex++;
        if(currentSpecialIndex >= GenerateBullet.instance.specialBullets.Count)
        {
            currentSpecialIndex = 0;
        }
        Selectors[currentSpecialIndex].SetActive(true);
    }
}
