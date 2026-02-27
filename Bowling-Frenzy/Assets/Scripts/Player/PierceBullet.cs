using UnityEngine;

public class PierceBullet : NormalBulletBehaviour
{
    int MaxPierce = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(transform.position, Vector3.zero);
        currentSpecial = SpecialBullets.Piercing;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    internal virtual void CheckEnemy(Collider collider)
    {

        if (collider.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage);
            MaxPierce--;

            if (MaxPierce <= 0)
            {
                OnDeactivate();
            }
        }
    }
}
