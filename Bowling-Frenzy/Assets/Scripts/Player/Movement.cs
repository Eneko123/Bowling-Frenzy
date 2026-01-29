using UnityEngine;
using UnityEngine.InputSystem;
public class Movement_ch : MonoBehaviour
{
    //Para acceder al rigidbody del jugador 
    private Rigidbody rbPlayer;
    //Es la velocidad a la que se mover� el jugador
    [SerializeField] float moveSpeed;
    //Se usar�n para el movimiento
    private float horizontal = 0f;
    private float vertical = 0f;
    //Layer para evitar traspasar muros 
    [SerializeField] LayerMask wallLayer;
    //Define la maxima distancia del raycast detector de paredes
    float maxDistance = 0.85f;

    // El rigidbody que se usa para detectar al jugador se inicializa para que no rote.
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
        rbPlayer.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Stop();
        Move();
        Raycast();
    }
    void Stop()
    {
        //Hace que el jugador no se mueva
        horizontal = 0f;
        vertical = 0f;
    }
    void Move()
    {
        //Dependiendo de a que tecla se pulse se movera hacia delante, hacia atras, hacia la izquierda o hacia la derecha. Tambi�n en las diagonales
        if (Input.GetKey(KeyCode.W))
        {
            vertical += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vertical -= 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontal -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontal += 1f;
        }
    }
    void Raycast()
    {
        //Si alguna de las dos variables es distinta de 0
        if (horizontal != 0 || vertical != 0)
        {
            //Se crea el vector normalizado de la direccion en la que se quiere mover
            Vector3 movement = (transform.forward * vertical + transform.right * horizontal).normalized;
            //Si se esta moviendo el jugador
            if (movement != Vector3.zero)
            {
                //Para recibir la informacion del raycast
                RaycastHit hit;

                // Verifica si hay una pared adelante del jugador
                if (!Physics.Raycast(transform.position, movement, out hit, maxDistance, wallLayer))
                {
                    //Solo se mueve si no hay muro enfrente
                    //Se cambia la posicion del jugador dependiendo de la velocidad 
                    transform.position += movement * moveSpeed * Time.deltaTime;
                    Debug.DrawRay(movement, Vector3.one, Color.red);
                }
                //Si no no se mueve
                else
                {
                    movement = Vector3.zero;
                }
            }
        }
    }
}