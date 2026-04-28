using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveBulletBehaviour : NormalBulletBehaviour
{
    [SerializeField] private Explosion explosionEffect;
    [SerializeField] private float explosionScale = 8f; // Almacena la escala de explosion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        this.currentSpecial = SpecialBullets.Explosive;

        // Asegurase de que la explosion este desactivada al inicio
        if (explosionEffect != null)
        {
            explosionEffect.gameObject.SetActive(false);
        }
    }
    void Start()
    {
        // Inicializa las propiedades de la explosión
        if (explosionEffect != null)
        {
            explosionEffect.SetExplosionScale(explosionScale);
        }
    }

    // Update is called once per frame
    public override SpecialBullets GetSpecialBullet()
    {
        return currentSpecial;
    }

    protected override void OnDeactivate()
    {
        // Activa la explosion en lugar de instanciarla
        if (explosionEffect != null)
        {
            explosionEffect.SetExplosionDamage(damage);
            explosionEffect.SetExplosionScale(explosionScale);
            explosionEffect.gameObject.SetActive(true);
        }

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

    public float GetExplosionScale()
    {
        return explosionScale;
    }

    public void SetExplosionScale(float newScale)
    {
        explosionScale = newScale;
    }
}
