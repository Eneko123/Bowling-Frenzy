using UnityEngine;

public class Sweeper : MonoBehaviour
{
    private int damage = 99999;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Sweeper collided with: " + other.gameObject.name);
        if (other.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            Debug.Log("Sweeper hit an enemy");
            enemy.ReceiveDamage(damage);
        }
    }


    void DesactiveSweeper()
    {
        this.gameObject.SetActive(false);
    }
}