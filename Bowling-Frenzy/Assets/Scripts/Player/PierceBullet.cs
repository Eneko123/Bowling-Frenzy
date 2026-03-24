using UnityEngine;

public class PierceBullet : NormalBulletBehaviour
{
    int MaxPierce = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        this.currentSpecial = SpecialBullets.Piercing;
    }
    void Start()
    {
        //Init(transform.position, Vector3.zero);
    }
    //internal void AddPierce()
    //{
    //    for (int i = 0; i < GenerateBullet.instance.listOfHabilities.Length; i++)
    //    {
    //        if (GenerateBullet.instance.listOfHabilities[i] == null)
    //        {
    //            GenerateBullet.instance.listOfHabilities[i] = this.gameObject;
    //            break;
    //        }
    //    }
    //}
    internal SpecialBullets GetSpecialBullet()
    {
        return currentSpecial;
    }
    internal override void CheckEnemy(Collider collider)
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

    public override float GetDamage()
    {
        return this.damage;
    }

    public override void SetDamage(float newDamage)
    {
        this.damage = newDamage;
    }
}
