using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public GameObject jump;
    public GameObject atack;

    void StopAttacks()
    {
        jump.SetActive(false);
        atack.SetActive(false);
        jump.transform.localScale = new Vector3(10f, 0.01f, 10f);
        atack.transform.localScale = new Vector3(10f, 0.01f, 10f);
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