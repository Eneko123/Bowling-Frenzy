using UnityEngine;

public class SelectDificulty : MonoBehaviour
{
    public void SetEasy() => GameManager.Instance.difficulty = Difficulty.Easy;
    public void SetNormal() => GameManager.Instance.difficulty = Difficulty.Normal;
    public void SetHard() => GameManager.Instance.difficulty = Difficulty.Hard;
}
