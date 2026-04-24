using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveBulletBehaviour : NormalBulletBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        this.currentSpecial = SpecialBullets.Explosive;
    }
    void Start()
    {
        //Init(transform.position, Vector3.zero);
    }
    //internal void AddExplosive()
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

    // Update is called once per frame
    internal SpecialBullets GetSpecialBullet()
    {
        return currentSpecial;
    }

    protected override void OnDeactivate()
    {
        GameObject e = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        e.TryGetComponent<Explosion>(out Explosion explosion);
        explosion.SetExplosionDamage(damage);
        base.OnDeactivate();
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
