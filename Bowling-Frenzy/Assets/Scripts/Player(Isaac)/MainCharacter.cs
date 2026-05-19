using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class MainCharacter : MonoBehaviour
{

    float MaxHealth = 100f;
    float playerHealth;
    // Para bajar la vida del enemigo se puede hacer playerHealth = playerHealth - damage + defense 
    float defense = 0;
    float healthRecovery;
    [Space(1)]
    //Sirve para ver la dirección en la que se mueve
    public Vector2 MoveDir = Vector2.zero;
    //Hace referencia al componente CharacterController del objeto al que se le añadira
    private CharacterController controller;
    //Velocidad del jugador para moverse
    [SerializeField] float speed = 4f;
    //Sirve para controlar el salto
    [SerializeField] Vector3 velocity;
    //Fuerza con la que se quiere que el jugador salte
    [SerializeField] float normalJumpForce = 1.6f;      // Salto pequeño
    [SerializeField] float bigJumpForce = 3.2f;         // Salto grande
    [SerializeField] float bigJumpCooldown = 5f;      // Tiempo entre saltos grandes
    private float bigJumpTimer = 0f;                  // Timer interno
    private bool canUseBigJump = true;
    //La gravedad para hacer que el jugador caiga
    [SerializeField] float gravity = -9.8f;
    public CambiaArmasVisualizer changeWeapon;
    public HealthUI saludJugador;
    //Sirve para controlar por si el jugador decide dejar de pulsar al completo porque quiere cancelar el salto
    private float jumpTimeStamp;
    private float jumpTime = 0f;
    public CooldownIndicator tiempoRecarga;
    //Controla el si se puede mover el jugador o no
    private bool _movementInputPressed = false;

    [SerializeField] CameraPlayer cameraPlayer;
    [SerializeField] GameObject pointOfShoot;
    bool isReloadingNormalBullet = false;
    bool isReloadingExplosiveBullet = false;
    bool isReloadingPiercingBullet = false;
    bool isReloadingSlowingBullet = false;

    int typeOfBullet = 0;//Cambiar con el enum de las balas

    [SerializeField] UIGameplay uiGameplay;

    // Singleton para que el enemigo pueda acceder a la posición del jugador
    public static MainCharacter Instance { get; private set; }
    public Transform playerTransform;

    SpecialBullets currentSpecialBullet;

    Animator animator;

    CharacterController characterController;

    // Variables para el disparo continuo
    private bool isShootingPressed = false;
    private bool isShootingLoopActive = false;
    private Coroutine normalBulletReloadCoroutine = null;

    [Header("Special Bullets UI")]
    [SerializeField] private UIGameplay[] specialBulletUIs; // Arrastra en Inspector
    private Dictionary<SpecialBullets, (float cooldownTime, UIGameplay ui)> bulletCooldowns;

    private LayerMask floor;
    private float rayDistance = 0.25f;

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
        playerHealth = MaxHealth;
        healthRecovery = (playerHealth * 15) / 100;
        saludJugador.UpdateHealth(playerHealth, MaxHealth);
        floor = LayerMask.GetMask("Ground");
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        bulletCooldowns = new Dictionary<SpecialBullets, (float, UIGameplay)>
    {
        { SpecialBullets.Explosive, (10f, null) },
        { SpecialBullets.Piercing,  (5.5f, null) },
        { SpecialBullets.Slowing,   (7f, null) }
    };
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
        // Si el jugador realiza la acción y está en el suelo
        if (contextJump.performed && controller.isGrounded)
        {
            jumpTimeStamp = Time.time;

            // Determinar qué tipo de salto usar
            float jumpForceToUse;

            if (canUseBigJump)
            {
                // Usar salto grande
                jumpForceToUse = bigJumpForce;
                canUseBigJump = false;
                bigJumpTimer = bigJumpCooldown; // Iniciar cooldown
            }
            else
            {
                // Usar salto normal
                jumpForceToUse = normalJumpForce;
            }

            // Aplicar la fuerza de salto elegida
            velocity.y = MathF.Sqrt(jumpForceToUse * -3 * gravity);
        }
        else if (contextJump.canceled)
        {
            // Si decide no querer saltar al máximo se frenará el salto
            if (Time.time - jumpTimeStamp < jumpTime)
            {
                velocity.y = 0;
            }
        }
    }
    public void OnShoot(InputAction.CallbackContext contextShoot)
    {

        if (!uiGameplay.isPaused && !uiGameplay.isUpgradeMenuOpen)
        {
            // Cuando se presiona el botón
            if (contextShoot.started)
            {
                isShootingPressed = true;
                if (!isReloadingNormalBullet && !isShootingLoopActive)
                {
                    StartCoroutine(ShootingLoopCoroutine());
                }
            }
            // Cuando se suelta el botón
            else if (contextShoot.canceled)
            {
                isShootingPressed = false;
                StopShootingLoop();
            }
        }
    }

    // Corrutina que maneja el loop de disparo
    private IEnumerator ShootingLoopCoroutine()
    {
        isShootingLoopActive = true;

        while (isShootingPressed)
        {
            // Solo dispara si no está recargando
            if (!isReloadingNormalBullet)
            {
                animator.SetTrigger("isAttacking");
            }

            // Espera el tiempo de recarga antes del siguiente disparo
            yield return new WaitForSeconds(0.5f);
        }

        isShootingLoopActive = false;
    }

    // Detiene el loop de disparo
    private void StopShootingLoop()
    {
        if (isShootingLoopActive)
        {
            StopCoroutine(ShootingLoopCoroutine());
            isShootingLoopActive = false;
        }

        // Detiene la corrutina de recarga de balas normales
        if (normalBulletReloadCoroutine != null)
        {
            StopCoroutine(normalBulletReloadCoroutine);
            normalBulletReloadCoroutine = null;
        }
        isReloadingNormalBullet = false;
    }

    // Función pública para resetear el estado de disparo
    // Evita acomulaciones de disparos
    public void ResetShootingState()
    {
        isShootingPressed = false;
        StopShootingLoop();

        // Detener también las corrutinas de balas especiales si están activas
        StopAllCoroutines();

        isShootingLoopActive = false;
        normalBulletReloadCoroutine = null;
    }

    public void OnSpecial(InputAction.CallbackContext contextSpecial)
    {
        if (contextSpecial.performed && UIGameplay.uI != null && UIGameplay.uI.IsSpecialReady(currentSpecialBullet))
        {
            bool wasShootingPressed = isShootingPressed;
            isShootingPressed = false;
            StopShootingLoop();

            animator.SetTrigger("IsSpecial");

            if (wasShootingPressed)
                StartCoroutine(ResumeShootingAfterSpecial());
        }
    }

    // Corrutina para reanudar el disparo normal después del disparo especial
    private IEnumerator ResumeShootingAfterSpecial()
    {
        // Espera un frame para que la animación especial se active
        yield return null;

        // Espera a que termine la animación especial (ajustar este tiempo según la duración de la animación)
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        // Verifica que el jugador realmente siga presionando el botón (no solo que la variable esté en true)
        if (isShootingPressed && !isReloadingNormalBullet && !isShootingLoopActive)
        {
            StartCoroutine(ShootingLoopCoroutine());
        }
    }

    public void OnChangeSpecial(InputAction.CallbackContext contextSpecial)
    {
        if (contextSpecial.performed)
        {
            int currentSpecial = 0;
            GenerateBullet currentHability = GenerateBullet.instance;
            InputBinding? binding = contextSpecial.action.GetBindingForControl(contextSpecial.control);
            InputBinding K1 = new InputBinding(path: "<Keyboard>/1", action: "ChangeSpecial");
            InputBinding K2 = new InputBinding(path: "<Keyboard>/2", action: "ChangeSpecial");
            InputBinding K3 = new InputBinding(path: "<Keyboard>/3", action: "ChangeSpecial");

            if (binding.Value.path == K1.path)
            {
                currentSpecial = 1;
                currentSpecialBullet = currentHability.ChangeHability(0);
                Debug.Log("1Spec");
            }
            else if (binding.Value.path == K2.path)
            {
                currentSpecial = 2;
                currentSpecialBullet = currentHability.ChangeHability(1);
                Debug.Log("2Spec");
            }
            else if (binding.Value.path == K3.path)
            {
                currentSpecial = 3;
                currentSpecialBullet = currentHability.ChangeHability(2);
                Debug.Log("3Spec");
            }

            changeWeapon.UpdateActive(currentSpecial);

            Debug.Log(binding.Value);
            Debug.Log(currentHability.currentPositionHability);
        }
    }
    IEnumerator DelayForNormalBullet(float delay)
    {
        isReloadingNormalBullet = true;
        yield return new WaitForSeconds(delay);
        isReloadingNormalBullet = false;
        normalBulletReloadCoroutine = null;
    }

    
    private void Update()
    {
        // Si el juego se pausa o se abre el menú de mejoras, detener el disparo
        if ((uiGameplay.isPaused || uiGameplay.isUpgradeMenuOpen) && isShootingPressed)
        {
            isShootingPressed = false;
            StopShootingLoop();
        }
        if (!canUseBigJump)
        {
            bigJumpTimer -= Time.deltaTime;
            if (bigJumpTimer <= 0f)
            {
                canUseBigJump = true;
                Debug.Log("¡SALTO GRANDE DISPONIBLE!");
            }
        }
        if (_movementInputPressed)
        {
            //Se mueve el jugador en la direccion dada a la velocidad dada
            Vector3 move = (this.transform.forward * MoveDir.y + cameraPlayer.transform.right * MoveDir.x);
            controller.Move(move.normalized * speed * Time.deltaTime);
        }
        //Calcula para que el jugador baje segun la gravedad
        if (!IsTouchingFloor())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    bool IsTouchingFloor()
    {
        RaycastHit hit;

        // Lanzar el raycast hacia abajo
        if (Physics.Raycast(transform.position, -transform.up, out hit, rayDistance, floor))
        {
            // Debug para visualizar el ray
            Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.green);
            // velocity.y = 0;
            return true;
        }

        Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.red);
        return false;
    }

    void Dead()
    {
        if (playerHealth <= 0)
        {
            // Condicion
            GameManager.Instance.winornot = false;
            SceneManager.LoadScene("Game_Over");
        }
    }
    public void RestoreHealthByCombo()
    {
        if (playerHealth < MaxHealth)
        {
            playerHealth += Mathf.Round(healthRecovery);
            saludJugador.UpdateHealth(playerHealth, MaxHealth);
        }
        if (playerHealth >= MaxHealth)
        {
            playerHealth = MaxHealth;
            saludJugador.UpdateHealth(playerHealth, MaxHealth);
        }
    }
    public void damageHealthPlayer(float damage)
    {
        playerHealth -= Mathf.Round(damage * (1 - defense));
        Debug.Log("Player health decreased");
        saludJugador.UpdateHealth(playerHealth, MaxHealth);
        StartCoroutine(InvencibilityCoroutine());
        Dead();
    }

    IEnumerator InvencibilityCoroutine()
    {
        characterController.detectCollisions = false;
        yield return new WaitForSeconds(0.5f);
        characterController.detectCollisions = true;
    }

    void ThrowNormalBall()
    {
        // Solo dispara si no está recargando
        if (isReloadingNormalBullet)
            return;

        GameObject b = GenerateBullet.instance.GetBullets();
        b.GetComponentInChildren<NormalBulletBehaviour>().Init(pointOfShoot.transform.position, cameraPlayer.transform.forward);

        // Detener la corrutina anterior si existe
        if (normalBulletReloadCoroutine != null)
        {
            StopCoroutine(normalBulletReloadCoroutine);
        }

        typeOfBullet = 0;
        normalBulletReloadCoroutine = StartCoroutine(DelayForNormalBullet(0.5f));
    }
    void ThrowSpecialBall()
    {
        // Seguridad por si se llama antes de tiempo
        if (UIGameplay.uI == null || !UIGameplay.uI.IsSpecialReady(currentSpecialBullet)) return;

        GameObject b = GenerateBullet.instance.SelectTheSpecial(currentSpecialBullet);
        if (b == null) return;

        b.GetComponentInChildren<NormalBulletBehaviour>().Init(pointOfShoot.transform.position, cameraPlayer.transform.forward);

        // Duraciones centralizadas (puedes moverlas a un ScriptableObject después si quieres)
        float cooldown = currentSpecialBullet switch
        {
            SpecialBullets.Explosive => 10f,
            SpecialBullets.Piercing => 5.5f,
            SpecialBullets.Slowing => 7f,
            _ => 0f
        };

        // Notifica a la UI y elimina lógica duplicada
        UIGameplay.uI.StartSpecialCooldown(currentSpecialBullet, cooldown);
    }
    #region Geters-Seters
    public float GetHealthMax() { return MaxHealth; }
    public float GetCurrentHealth() { return playerHealth; }
    public float GetDefense() { return defense; }
    public float GetSpeed() { return speed; }

    public bool CanUseBigJump() { return canUseBigJump; }
    public float GetBigJumpCooldownRemaining() { return bigJumpTimer; }
    public float GetBigJumpCooldownTotal() { return bigJumpCooldown; }


    public void SetHealthMax(float healtUp) { MaxHealth = healtUp; }
    public void SetCurrentHealth(float currentHealtUp) { playerHealth = currentHealtUp; }
    public void SetDefense(float defenseUp) { defense = defenseUp; }
    public void SetSpeed(float speedUp) { speed = speedUp; }
    #endregion
}
