using UnityEngine;

public class DetectFloor : MonoBehaviour
{
    [SerializeField] private EnemyBase enemy;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private LayerMask floor;
    [SerializeField] private float rayDistance = 0.2f;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
        particle = GetComponentInParent<ParticleSystem>();
    }

    void Update()
    {
        ParticleSystemActiveOrNot();
    }

    bool IsTouchingFloor()
    {
        RaycastHit hit;

        // Lanzar el raycast hacia abajo
        if (Physics.Raycast(transform.position, -transform.up, out hit, rayDistance, floor))
        {
            // Debug para visualizar el ray
            Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.green);
            return true;
        }

        Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.red);
        return false;
    }

    void ParticleSystemActiveOrNot()
    {
        if (particle == null || enemy == null) return;

        // Activar particulas solo si esta tocando el suelo y esta vivo
        bool shouldEmit = IsTouchingFloor() && enemy.GetHealth() > 0;
        var emission = particle.emission;
        emission.enabled = shouldEmit;
    }
}