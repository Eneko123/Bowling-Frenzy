using UnityEngine;
using UnityEngine.EventSystems;

public class DetectFloor : MonoBehaviour
{
    [SerializeField] private EnemyBase enemy;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private LayerMask floor;
    
    void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
        particle = GetComponentInParent<ParticleSystem>();
        floor = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystemActiveOrNot();
    }

    bool IsTouchingFloor()
    {
        PhysicsRaycaster hit = null;

        if (Physics.Raycast(transform.position, -transform.up, 0.2f, floor))
        {
            if (hit.eventMask == floor)
            {
                return true;
            }
        }
        return false;
    }

    void ParticleSystemActiveOrNot()
    {
        if (!IsTouchingFloor() || enemy.GetHealth() > 0)
        {
            particle.enableEmission = false;
        }
        particle.enableEmission = true;
    }
}
