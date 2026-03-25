using UnityEngine;
public enum Difficulty { Easy, Normal, Hard }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Difficulty difficulty = Difficulty.Normal;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
}
