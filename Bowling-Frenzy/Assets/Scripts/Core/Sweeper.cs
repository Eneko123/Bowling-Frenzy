using UnityEngine;

public class Sweeper : MonoBehaviour
{
    [SerializeField] PowerUps powerUps;
    [SerializeField] RoundsManager roundsManager;

    private int damage = 99999;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage);
        }
    }


    void DesactiveSweeper()
    {
        this.gameObject.SetActive(false);
        powerUps.ShowUpgradesForRound(roundsManager.CurrentRound);
        Cursor.lockState = CursorLockMode.None;
    }
}