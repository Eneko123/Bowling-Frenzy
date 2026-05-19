using UnityEngine;

public class StopAtacks : MonoBehaviour
{
    [SerializeField] private GameObject jump;
    [SerializeField] private GameObject atack;

    void StopAttacks()
    {
        if (jump != null)
        {
            jump.SetActive(false);
        }

        if (atack != null)
        {
            atack.SetActive(false);
        }
    }
}
