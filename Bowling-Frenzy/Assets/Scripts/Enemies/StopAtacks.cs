using UnityEngine;

public class StopAtacks : MonoBehaviour
{
    [SerializeField] private GameObject jump;
    [SerializeField] private GameObject atack;

    private float damage;
    private float damageCooldown = 0;

    private void Awake()
    {
        damage = BoloEBoos.Instance.GetComponent<AreaAttack>().damage;
    }

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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out MainCharacter player))
        {
            // Reducir el cooldown constantemente
            damageCooldown -= Time.deltaTime;

            // Hacer danio solo cuando el cooldown llega a 0 o menos
            if (damageCooldown <= 0)
            {
                player.damageHealthPlayer(damage);
                AudioManager.Instance.PlaySFX("AtqNormalBolos");
                damageCooldown = 1.5f; // Reiniciar cooldown
            }
        }
    }
}
