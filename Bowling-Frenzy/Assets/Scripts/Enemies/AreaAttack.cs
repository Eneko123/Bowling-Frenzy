using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public GameObject jump;
    public GameObject atack;

    void StopAttacks()
    {
        jump.SetActive(false);
        atack.SetActive(false);
    }
}