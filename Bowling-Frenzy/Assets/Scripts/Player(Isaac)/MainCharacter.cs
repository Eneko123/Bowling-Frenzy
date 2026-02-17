using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class MainCharacter : MonoBehaviour
{
    //Sirve para ver la dirección en la que se mueve
    public Vector2 MoveDir = Vector2.zero;
    //Hace referencia al componente CharacterController del objeto al que se le añadira
    private CharacterController controller;
    //Velocidad del jugador para moverse
    [SerializeField] float speed = 4f;
    //Sirve para controlar el salto
    [SerializeField] Vector3 velocity;
    //Fuerza con la que se quiere que el jugador salte
    [SerializeField] float jumpForce = 4f;
    //La gravedad para hacer que el jugador caiga
    [SerializeField] float gravity = -9.8f;

    //Sirve para controlar por si el jugador decide dejar de pulsar al completo porque quiere cancelar el salto
    private float jumpTimeStamp;
    private float jumpTime = 0.2f;
    
    //Controla el si se puede mover el jugador o no
    private bool _movementInputPressed = false;

    [SerializeField] CameraPlayer cameraPlayer;

    [SerializeField]GameObject pointOfShoot;

    bool isReloading = false;

    // Singleton para que el enemigo pueda acceder a la posición del jugador
    public static MainCharacter Instance { get; private set; }
    public Transform playerTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerTransform = transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    //Se llamara al evento en Unity asociado con la accion de moverse
    public void OnMoveInput(InputAction.CallbackContext contextMove)
    {
        //Si ha empezado se le dara permiso para que avance
        if (contextMove.started)
        {
            _movementInputPressed = true;
        }
        //Cuando termina de moverse deja de moverse
        else if (contextMove.canceled)
        {
            _movementInputPressed = false;
        }
        //Controla la dirección en la que se mueve
        MoveDir = contextMove.ReadValue<Vector2>();
    }
    //Se llamara al evento en Unity asociado con la accion de saltar
    public void OnJumpInput(InputAction.CallbackContext contextJump)
    {
        //Si el jugador ha realizado la accion y se encuentra en el suelo
        if (contextJump.performed && controller.isGrounded)
        {
            //Empezara una cuenta para saber si el jugador quiere saltar más o menos
            jumpTimeStamp = Time.time;
            //Ayuda a establecer la máxima altura a la que el jugador quiere llegar
            velocity.y = MathF.Sqrt(jumpForce * -3 * gravity);
        }
        else if (contextJump.canceled)
        {
            //Si decide no querer saltar al maximo se frenara el salto y bajara el jugador
            if (Time.time - jumpTimeStamp < jumpTime)
            {
                velocity.y = 0;
            }
        }
    }
    public void OnShoot(InputAction.CallbackContext contextShoot)
    {
        if (contextShoot.performed && !isReloading)
        {
            NormalBulletBehaviour b = GenerateBullet.instance.GetBullets();
            b.Init(pointOfShoot.transform.position, cameraPlayer.transform.forward);
            StartCoroutine(DelayForBullets());
        }
    }
    IEnumerator DelayForBullets()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.5f);
        isReloading = false;
    }
    private void Update()
    {
        if (_movementInputPressed)
        {
            //Se mueve el jugador en la direccion dada a la velocidad dada
            Vector3 move = (this.transform.forward * MoveDir.y + cameraPlayer.transform.right * MoveDir.x);
            controller.Move(move.normalized * speed * Time.deltaTime);
        }
        //Calcula para que el jugador baje segun la gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
