using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionTime = 0.5f;
    [SerializeField] private float explosionScale = 8f;
    [SerializeField] private float damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        float timer = 0f;
        while (timer < explosionTime)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * explosionScale, timer / explosionTime);
            yield return null;
        }
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider enemy)
    {
        CheckEnemy(enemy);
        Debug.Log("Explosion hit: " + enemy.gameObject.name);
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
    public void SetExposionScale(float newScale)
    {
        explosionScale = newScale;
    }
}
