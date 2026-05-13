using UnityEngine;
using UnityEngine.AI;

public class EnemyGroundAlignment : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer; // Capa(s) que se consideran como suelo
    [SerializeField] private float raycastDistance = 2f; // Distancia maxima para detectar el suelo
    [SerializeField] private float raycastOffset = 0.5f; // Altura desde donde se lanza el raycast (evita colision propia)

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f; // Velocidad de rotacion suave (mayor = mas rapido)
    [SerializeField] private bool alignToGround = true; // Activar/desactivar alineacion al suelo
    [SerializeField] private float maxTiltAngle = 45f; // angulo maximo de inclinacion permitido (0-90°)

    [Header("Exclusions")]
    [SerializeField] private Transform minimapIndicator; // Objeto que NO debe inclinarse
    [SerializeField] private Transform bolo; // Objeto al que sigue

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true; // Mostrar rayos de debug en Scene view

    // Variables privadas
    private Vector3 groundNormal = Vector3.up; // Normal de la superficie actual (Vector3.up = suelo plano)
    private Collider[] myColliders; // Cache de colliders propios para ignorarlos en raycasts
    private NavMeshAgent agent; // Referencia al NavMeshAgent (para obtener direccion de movimiento)
    private Rigidbody rb; // Referencia al Rigidbody (para aplicar rotacion fisica correctamente)

    // Variables para preservar rotacion del minimapa
    private Quaternion minimapOriginalLocalRotation; // Rotacion local inicial del indicador
    private bool hasMinimapIndicator; // Flag para saber si existe el indicador

    void Start()
    {
        // Inicializar referencias
        myColliders = GetComponentsInChildren<Collider>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Desactivar rotacion automatica del NavMeshAgent
        // Si no hacemos esto, el NavMeshAgent sobreescribira la rotacion personalizada
        if (agent != null)
        {
            agent.updateRotation = false; // El agente ya NO controla la rotacion horizontal
            agent.updateUpAxis = false;   // El agente ya NO fuerza el eje Y hacia arriba
        }

        // Guardar rotacion inicial del indicador de minimapa
        if (minimapIndicator != null)
        {
            minimapOriginalLocalRotation = minimapIndicator.localRotation;
            hasMinimapIndicator = true;
        }
    }

    void Update()
    {
        if (alignToGround)
        {
            DetectGroundAngle();
            AlignToGround();

            // Restaurar rotacion del indicador despues de alinear
            if (hasMinimapIndicator)
            {
                PreserveMinimapRotation();
            }
        }
    }

    // Detecta la inclinacion del suelo mediante un raycast hacia abajo
    // Actualiza la variable groundNormal con la normal de la superficie detectada
    void DetectGroundAngle()
    {
        // Punto de origen del raycast (ligeramente por encima del enemigo)
        // Usamos raycastOffset para evitar que el rayo empiece dentro del propio collider
        Vector3 rayOrigin = transform.position + Vector3.up * raycastOffset;

        RaycastHit hit; // Informacion del impacto del raycast

        // Lanzar rayo hacia abajo para detectar el suelo
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            // Verificar si el raycast golpeo algun collider propio (auto-colision)
            bool isMyCollider = false;
            foreach (Collider col in myColliders)
            {
                if (hit.collider == col)
                {
                    isMyCollider = true;
                    break;
                }
            }

            // Si golpeamos nuestro propio collider, ignorar y usar normal vertical
            if (isMyCollider)
            {
                groundNormal = Vector3.up;
                //if (showDebugRays)
                //{
                //    Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.yellow); // Amarillo = auto-colision
                //}
                return;
            }

            // Obtener la normal de la superficie (vector perpendicular al suelo)
            groundNormal = hit.normal;

            // Debug: Visualizar el raycast exitoso
            //if (showDebugRays)
            //{
            //    Debug.DrawRay(rayOrigin, Vector3.down * hit.distance, Color.green); // Verde = suelo detectado
            //    Debug.DrawRay(hit.point, hit.normal * 2f, Color.cyan); // Cyan = normal del suelo
            //}
        }
        else
        {
            // No se detecto suelo: volver a orientacion vertical por defecto
            groundNormal = Vector3.up;

            // Debug: Visualizar raycast fallido
            //if (showDebugRays)
            //{
            //    Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.red); // Rojo = sin suelo
            //}
        }
    }

    // Alinea la rotacion del enemigo con la normal del suelo detectada
    // Mantiene la direccion de movimiento mientras se inclina segun el terreno
    void AlignToGround()
    {
        // === PASO 1: Obtener direccion de movimiento horizontal (sin componente Y) ===
        Vector3 worldForward;

        // Si el NavMeshAgent se esta moviendo, usar su direccion de velocidad
        if (agent != null && agent.velocity.sqrMagnitude > 0.1f)
        {
            worldForward = agent.velocity;
            worldForward.y = 0; // Proyectar al plano XZ (horizontal)
            worldForward.Normalize();
        }
        else
        {
            // Si esta parado, mantener la direccion forward actual
            worldForward = transform.forward;
            worldForward.y = 0; // Proyectar al plano XZ

            // Fallback si el vector es casi cero
            if (worldForward.magnitude < 0.01f)
            {
                worldForward = Vector3.forward;
            }
            worldForward.Normalize();
        }

        // === PASO 2: Aplicar limite de angulo de inclinacion ===
        Vector3 normalToUse = groundNormal;
        float tiltAngle = Vector3.Angle(Vector3.up, groundNormal); // angulo entre vertical y suelo

        // Si el suelo esta mas inclinado que el limite, interpolar hacia una inclinacion menor
        if (tiltAngle > maxTiltAngle)
        {
            // Slerp crea una interpolacion suave entre Vector3.up (vertical) y groundNormal
            // El factor (maxTiltAngle / tiltAngle) limita la inclinacion al maximo permitido
            normalToUse = Vector3.Slerp(Vector3.up, groundNormal, maxTiltAngle / tiltAngle);
        }

        // === PASO 3: Calcular sistema de coordenadas ortogonal ===
        // Calculamos un vector "derecha" perpendicular a la normal y al forward
        Vector3 right = Vector3.Cross(normalToUse, worldForward);

        // Validacion: Si el cross product falla (vectores paralelos), usar vector alternativo
        if (right.magnitude < 0.01f)
        {
            right = Vector3.Cross(normalToUse, Vector3.right);
            if (right.magnitude < 0.01f)
            {
                right = Vector3.Cross(normalToUse, Vector3.back);
            }
        }
        right.Normalize();

        // Calcular el nuevo forward perpendicular al "right" y a la normal
        // Esto asegura un sistema de coordenadas ortogonal (perpendicular en los 3 ejes)
        Vector3 newForward = Vector3.Cross(right, normalToUse).normalized;

        // === PASO 4: Crear rotacion objetivo ===
        // LookRotation crea un Quaternion donde:
        // - El eje Z (forward) apunta a newForward
        // - El eje Y (up) apunta a normalToUse
        Quaternion targetRotation = Quaternion.LookRotation(newForward, normalToUse);

        // === PASO 5: Aplicar rotacion con interpolacion suave ===
        // Clamp01 asegura que t este entre 0 y 1
        float t = Mathf.Clamp01(Time.deltaTime * rotationSpeed);

        // Si el objeto tiene Rigidbody fisico, usar MoveRotation para fisica correcta
        if (rb != null && !rb.isKinematic)
        {
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, t);
            rb.MoveRotation(newRotation); // Metodo recomendado para rotar Rigidbodies
        }
        else
        {
            // Sin Rigidbody o kinematic: rotar directamente el transform
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }

        // Visualizar vectores en Scene view
        //if (showDebugRays)
        //{
        //    Debug.DrawRay(transform.position, worldForward * 2f, Color.yellow);   // Amarillo = direccion horizontal
        //    Debug.DrawRay(transform.position, newForward * 2f, Color.magenta);    // Magenta = forward sobre rampa
        //    Debug.DrawRay(transform.position, right * 1f, Color.red);             // Rojo = vector derecha
        //}
    }


    // Mantiene la rotacion del indicador del minimapa sin verse afectada por la inclinacion
    void PreserveMinimapRotation()
    {
        // Mantener completamente vertical y sin rotacion
        minimapIndicator.rotation = Quaternion.Euler(90, 0, 0); // Rotacion (90, 0, 0)
        minimapIndicator.position = bolo.position + new Vector3(0, 15, 0); // Posicion estatica
    }

    // Obtiene el angulo de inclinacion actual del suelo en grados
    public float GetCurrentTiltAngle()
    {
        return Vector3.Angle(Vector3.up, groundNormal);
    }

    // Obtiene la normal del suelo actual
    public Vector3 GetGroundNormal()
    {
        return groundNormal;
    }


    // Activa o desactiva la alineacion al suelo
    public void SetAlignmentEnabled(bool enabled)
    {
        alignToGround = enabled;

        // Si se desactiva, restaurar control de rotacion al NavMeshAgent
        if (!enabled && agent != null)
        {
            agent.updateRotation = true;
            agent.updateUpAxis = true;
        }
    }


    // Dibuja gizmos en el editor cuando el objeto esta seleccionado
    //void OnDrawGizmosSelected()
    //{
    //    // Visualizar punto de origen del raycast
    //    Gizmos.color = Color.yellow;
    //    Vector3 rayOrigin = transform.position + Vector3.up * raycastOffset;
    //    Gizmos.DrawWireSphere(rayOrigin, 0.1f); // Esfera en el punto de origen
    //    Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * raycastDistance); // Linea del raycast

    //    // Visualizar normal del suelo
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawRay(transform.position, groundNormal * 2f);
    //}
}