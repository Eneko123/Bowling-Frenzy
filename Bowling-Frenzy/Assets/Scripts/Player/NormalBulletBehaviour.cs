using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
public enum SpecialBullets
{
    None, Explosive, Piercing, Slowing
}
public class NormalBulletBehaviour : MonoBehaviour
{
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float damage = 10;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Rigidbody rbParent;
    [SerializeField] private float lifeTime = 4f;
    private float _currentLifeTime = 4f;
    protected SpecialBullets currentSpecial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(Vector3 initialPos, Vector3 initialDir)
    {
        
        rbParent.linearVelocity = Vector3.zero;
        rbParent.angularVelocity = Vector3.zero;
        // transform.position = initialPos;
        rbParent.transform.position = initialPos;   
        direction = initialDir;
        _currentLifeTime = lifeTime;

        rbParent.AddForce(direction * speed, ForceMode.Force);
    }
    public virtual SpecialBullets GetSpecialBullet()
    {
        return currentSpecial;
    }
    // Update is called once per frame
    void Update()
    {
        if (_currentLifeTime > 0)
        {
            _currentLifeTime -= Time.deltaTime;
        }
        else
        {
            OnDeactivate();
        }

    }
    private void OnTriggerEnter(Collider enemy)
    {
        CheckEnemy(enemy);
    }
    internal virtual void CheckEnemy(Collider collider)
    {

        if (collider.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage);
            OnDeactivate();
        }
    }
    protected virtual void OnDeactivate()
    {
        Debug.Log("Hit Enemy");

        transform.parent.gameObject.SetActive(false);
    }

    public virtual float GetDamage() 
    {
        return damage;
    }

    public virtual void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
}