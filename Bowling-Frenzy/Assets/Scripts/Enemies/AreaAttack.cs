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

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by area attack");
        }
    }

}