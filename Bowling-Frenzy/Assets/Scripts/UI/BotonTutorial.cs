using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonTutorial : MonoBehaviour
{
    int MenuLevel = 0;
    static bool isFirstTimePlayed = true;
    [SerializeField] Button buttonReturnMainMenu;
    [SerializeField] Button buttonStartFirstPlaythrought;
    [SerializeField] GameObject uigameplay;
    static bool isTutorialOpen;
    public static BotonTutorial instance;
    public void ReturnMenu()
    {
        SceneManager.LoadScene(MenuLevel);
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(this);
        }
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
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (intToBool(PlayerPrefs.GetInt("isFirstTimePlayed")))
            {
                Time.timeScale = 0.0f;
                if (uigameplay != null)
                {
                    uigameplay.gameObject.SetActive(false);
                }
                this.gameObject.SetActive(isFirstTimePlayed);
                isTutorialOpen = true;
            }
            else
            {
                isTutorialOpen = false;
                isFirstTimePlayed = false;
                this.gameObject.SetActive(isFirstTimePlayed);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    public void StartPlaying()
    {
        this.gameObject.SetActive(false);
        isFirstTimePlayed = false;
        PlayerPrefs.SetInt("isFirstTimePlayed", boolToInt(isFirstTimePlayed));
        PlayerPrefs.Save();
        Time.timeScale = 1.0f;
        isTutorialOpen = false;
        if (uigameplay != null)
        {
            uigameplay.gameObject.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.Locked;
    }
    int boolToInt(bool val)
    {
        if (!val)
            return 1;
        else
            return 0;
    }

    bool intToBool(int val)
    {
        if (val == 0)
            return true;
        else
            return false;
    }
    public bool GetFirstPlayedChecker()
    {
        return isFirstTimePlayed;
    }
    public bool GetIfTutorialIsOpen()
    {
        return isTutorialOpen;
    }
}
