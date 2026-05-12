using UnityEngine;

public class EnemyGroundAlignment : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer; // Capa del suelo
    [SerializeField] private float raycastDistance = 2f; // Distancia del raycast
    [SerializeField] private float raycastOffset = 0.5f; // Altura desde donde se lanza el raycast

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f; // Velocidad de rotacion suave
    [SerializeField] private bool alignToGround = true; // Activar/desactivar alineación
    [SerializeField] private float maxTiltAngle = 45f; // Ángulo máximo de inclinación permitido

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = false; // Mostrar rayos en Scene view

    private Vector3 groundNormal = Vector3.up; // Normal del suelo actual

    void Update()
    {
        if (alignToGround)
        {
            DetectGroundAngle();
            AlignToGround();
        }
    }

    void DetectGroundAngle()
    {
        // Punto de origen del raycast (ligeramente por encima del enemigo)
        Vector3 rayOrigin = transform.position + Vector3.up * raycastOffset;

        RaycastHit hit;

        // Lanzar raycast hacia abajo
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            if (hit.collider.gameObject == gameObject) return; // Ignorar auto-colisión

            // Obtener la normal de la superficie
            groundNormal = hit.normal;

            // Debug visual
            if (showDebugRays)
            {
                Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.green);
                Debug.DrawRay(hit.point, hit.normal * 2f, Color.blue);
            }
        }
        else
        {
            // Si no detecta suelo, volver a vertical
            groundNormal = Vector3.up;

            if (showDebugRays)
            {
                Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.red);
            }
        }
    }

    void AlignToGround()
    {
        // Obtener la direccion forward actual del enemigo (en espacio mundial)
        Vector3 currentForward = transform.forward;

        // Proyectar el forward sobre el plano definido por la normal del suelo
        // Esto evita que el enemigo "gire" inesperadamente al inclinarse
        Vector3 projectedForward = Vector3.ProjectOnPlane(currentForward, groundNormal).normalized;

        // Si la proyeccion es muy pequenia (ej: enemigo casi vertical), usar fallback
        if (projectedForward.sqrMagnitude < 0.01f)
        {
            projectedForward = transform.right; // o cualquier dirección alternativa
        }

        // Crear rotación objetivo: forward proyectado + up = normal del suelo
        Quaternion targetRotation = Quaternion.LookRotation(projectedForward, groundNormal);

        // Limitar ángulo de inclinación si es necesario
        float tiltAngle = Vector3.Angle(Vector3.up, groundNormal);
        if (tiltAngle > maxTiltAngle)
        {
            Vector3 limitedNormal = Vector3.Slerp(Vector3.up, groundNormal, maxTiltAngle / tiltAngle);
            targetRotation = Quaternion.LookRotation(projectedForward, limitedNormal);
        }

        // Aplicar rotación suave
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Metodo publico para obtener el angulo de inclinacion actual
    public float GetCurrentTiltAngle()
    {
        return Vector3.Angle(Vector3.up, groundNormal);
    }

    // Método publico para obtener la normal del suelo
    public Vector3 GetGroundNormal()
    {
        return groundNormal;
    }

    // Metodo para activar/desactivar la alineacion
    public void SetAlignmentEnabled(bool enabled)
    {
        alignToGround = enabled;

        // Si se desactiva, volver a la rotacion vertical
        if (!enabled)
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar el área de deteccion en el editor
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + Vector3.up * raycastOffset;
        Gizmos.DrawWireSphere(rayOrigin, 0.1f);
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * raycastDistance);
    }
}