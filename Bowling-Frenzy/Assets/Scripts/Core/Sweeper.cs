using UnityEngine;

public class Sweeper : MonoBehaviour
{
    [SerializeField] PowerUps powerUps;
    [SerializeField] RoundsManager roundsManager;

    public static Sweeper instance;

    private int damage = 99999;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage, true);
        }
    }


    void DesactiveSweeper()
    {
        this.gameObject.SetActive(false);
        powerUps.ShowUpgradesForRound(roundsManager.CurrentRound);
        Cursor.lockState = CursorLockMode.None;
    }
}