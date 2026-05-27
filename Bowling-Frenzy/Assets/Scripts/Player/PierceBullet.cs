using UnityEngine;

public class PierceBullet : NormalBulletBehaviour
{
    [SerializeField] private int MaxPierce;
    int currentPierce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        currentPierce = MaxPierce;
        this.currentSpecial = SpecialBullets.Piercing;
    }
    void Start()
    {

    }
    public override SpecialBullets GetSpecialBullet()
    {
        return currentSpecial;
    }
    internal override void CheckEnemy(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage, false);
            currentPierce--;
            AudioManager.Instance.PlaySFX("PierceBulletEffect");
            if (currentPierce <= 0)
            {
                currentPierce = MaxPierce;
                OnDeactivate();
            }
        }
    }

    public override float GetDamage()
    {
        return this.damage;
    }

    public override void SetDamage(float newDamage)
    {
        this.damage = newDamage;
    }

    public int GetMaxPierce()
    {
        return MaxPierce;
    }

    public int GetPierce()
        { return currentPierce; }

    public void SetMaxPierce(int newMaxPierce)
    {
        MaxPierce = newMaxPierce;
    }

    public void SetPierce(int newPierce)
        { currentPierce = newPierce; }
}
