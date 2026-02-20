using UnityEngine;
public enum SpecialBullets
{
    Explosive,Perforating,Slowing
}
public class NormalBulletBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float lifeTime = 4f;
    private float currentLifeTime;
    protected SpecialBullets currentSpecial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(Vector3 initialPos, Vector3 initialDir)
    {
        Rigidbody r = GetComponent<Rigidbody>();
        r.linearVelocity = Vector3.zero;
        r.angularVelocity = Vector3.zero;
        transform.position = initialPos;
        direction = initialDir;
        currentLifeTime = lifeTime;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (currentLifeTime > 0)
        {
            currentLifeTime -= Time.deltaTime;
        }
        else
        {
            OnDeactivate();
        }
    }
    protected virtual void OnDeactivate()
    {
               gameObject.SetActive(false);
    }
}