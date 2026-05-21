using System.Collections;
using UnityEngine;

public class SlowBullet : NormalBulletBehaviour
{
    [SerializeField] private SlowExplosion slowExplosionEffect;
    [SerializeField] private float explosionScale = 8f; // Escala de la explosion ralentizadora
    [SerializeField] GameObject slowExplosion;
    [SerializeField] private float slowDuration = 2f; // Duracion del efecto de ralentizacion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        this.currentSpecial = SpecialBullets.Slowing;

        // Asegurarse de que la explosion este desactivada al inicio
        if (slowExplosionEffect != null)
        {
            slowExplosionEffect.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // Inicializa las propiedades de la explosion ralentizadora
        if (slowExplosionEffect != null)
        {
            slowExplosionEffect.SetExplosionScale(explosionScale);
            slowExplosionEffect.SetSlowDuration(slowDuration);
        }
    }

    public override SpecialBullets GetSpecialBullet()
    {
        return currentSpecial;
    }

    // Update is called once per frame
    internal override void CheckEnemy(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage, false);
            OnDeactivate();
        }
    }

    protected override void OnDeactivate()
    {
        // Activa la explosion ralentizadora
        if (slowExplosion != null)
        {
            GameObject slowExplosionInstance = Instantiate(slowExplosion, transform.position, Quaternion.identity);
            slowExplosionInstance.SetActive(true);

            SlowExplosion slowExplo = slowExplosionInstance.GetComponent<SlowExplosion>();
            slowExplo.SetExplosionScale(explosionScale);
            slowExplo.SetSlowDuration(slowDuration);
            slowExplo.startExplosion();
            Debug.Log("Slow explosion activated with scale: " + explosionScale + " and duration: " + slowDuration);
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

    public float GetSlowDuration()
    {
        return slowDuration;
    }

    public void SetSlowDuration(float newDuration)
    {
        slowDuration = newDuration;
    }
}
