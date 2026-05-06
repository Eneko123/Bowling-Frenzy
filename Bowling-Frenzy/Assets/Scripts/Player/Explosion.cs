using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] Transform explosionPos;
    [SerializeField] private float explosionTime = 0.5f;
    [SerializeField] private float explosionScale = 8f;
    [SerializeField] private float damage;

    private Coroutine explosionCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        // Reinicia la escala y comienza la animacion cada vez que se activa
        transform.position = explosionPos.position;
        transform.localScale = Vector3.zero;
        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
        }
        explosionCoroutine = StartCoroutine(ExplosionAnimation());
    }

    IEnumerator ExplosionAnimation()
    {
        float timer = 0f;
        while (timer < explosionTime)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * explosionScale, timer / explosionTime);
            yield return null;
        }
        // Desactiva en lugar de destruir
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    internal void CheckEnemy(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage);
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
