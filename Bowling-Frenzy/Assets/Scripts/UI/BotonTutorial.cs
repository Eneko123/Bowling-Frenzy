using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonTutorial : MonoBehaviour
{
    int MenuLevel = 0;
    public void ReturnMenu()
    {
        SceneManager.LoadScene(MenuLevel);
    }
}
