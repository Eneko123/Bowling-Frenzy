using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonTutorial : MonoBehaviour
{
    int MenuLevel = 0;
    static bool isFirstTimePlayed = true;
    [SerializeField] Button buttonReturnMainMenu;
    [SerializeField] Button buttonStartFirstPlaythrought;
    UIGameplay uigameplay;
    public void ReturnMenu()
    {
        SceneManager.LoadScene(MenuLevel);
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            buttonReturnMainMenu.gameObject.SetActive(true);
            buttonStartFirstPlaythrought.gameObject.SetActive(false);
        }
        else
        {
            buttonReturnMainMenu.gameObject.SetActive(false);
            buttonStartFirstPlaythrought.gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        uigameplay = GetComponentInParent<UIGameplay>();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (intToBool(PlayerPrefs.GetInt("isFirstTimePlayed", 0)))
            {
                Time.timeScale = 0.0f;
                if (uigameplay != null)
                {
                    uigameplay.gameObject.SetActive(false);
                }
                PlayerPrefs.SetInt("isFirstTimePlayed", boolToInt(isFirstTimePlayed));
                this.gameObject.SetActive(isFirstTimePlayed);
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                this.gameObject.SetActive(isFirstTimePlayed);
            }
        }
    }
    public void StartPlaying()
    {
        this.gameObject.SetActive(false);
        isFirstTimePlayed = false;
        PlayerPrefs.SetInt("isFirstTimePlayed", boolToInt(isFirstTimePlayed));
        Time.timeScale = 1.0f;
        if (uigameplay != null)
        {
            uigameplay.gameObject.SetActive(true);
        }
    }
    int boolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }

    bool intToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }
}
