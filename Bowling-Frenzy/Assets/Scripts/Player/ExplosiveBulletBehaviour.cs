using UnityEngine;

public class ExplosiveBulletBehaviour : NormalBulletBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(transform.position, Vector3.zero);
    }

    // Update is called once per frame

    protected override void OnDeactivate()
    {
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);



        base.OnDeactivate();
    }
}
