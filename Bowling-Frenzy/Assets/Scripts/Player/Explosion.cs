using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Explosion : MonoBehaviour
{
    [SerializeField] public Transform explosionPos;
    [SerializeField] private float explosionTime = 0.5f;
    [SerializeField] private float explosionScale = 8f;
    [SerializeField] private float damage;

    private Coroutine explosionCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {

        
    }

    public void startExplosion()
    {
        if (explosionPos != null)
        {
            transform.position = explosionPos.position;
        }// Reinicia la escala y comienza la animacion cada vez que se activa
        transform.localScale = Vector3.zero;
        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
        }
        explosionCoroutine = StartCoroutine(ExplosionAnimation());
    }

    IEnumerator ExplosionAnimation()
    {
        AudioManager.Instance.PlaySFX("ExplosionBulletEffect");
        float timer = 0f;
        while (timer < explosionTime)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * explosionScale, timer / explosionTime);
            yield return null;
        }
        // Desactiva en lugar de destruir
        gameObject.SetActive(false);
        Destroy(this);
    }

    // Update is called once per frame
    internal void CheckEnemy(Collider collider)
    {
        
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage, false);
            Debug.Log("Damage inflicted: " + damage);
        }
    }
    public void SetExplosionDamage(float newDamage)
    {
        damage = newDamage;
    }   
    public float GetExposionScale()
    {
        return explosionScale;
    }
    public void SetExplosionScale(float newScale)
    {
        explosionScale = newScale;
    }
}
