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
    int currentSpecial = 0;
    int specialSelected = 0;
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
    private Coroutine shootingLoopCoroutine = null;
    private Coroutine normalBulletReloadCoroutine = null;
    private float lastShootTime = -999f;

    [Header("Special Bullets UI")]
    [SerializeField] private UIGameplay[] specialBulletUIs; // Arrastra en Inspector
    private Dictionary<SpecialBullets, (float cooldownTime, UIGameplay ui)> bulletCooldowns;

    private LayerMask floor;
    private float rayDistance = 0.1f;

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

        // Suscripción por código para evitar que la referencia se pierda en runtime
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.actions["CycleWeapon"].performed += OnCycleWeapon;

        bulletCooldowns = new Dictionary<SpecialBullets, (float, UIGameplay)>
    {
        { SpecialBullets.Explosive, (10f, null) },
        { SpecialBullets.Piercing,  (5.5f, null) },
        { SpecialBullets.Slowing,   (7f, null) }
    };
    }

    private void OnDestroy()
    {
        // Siempre desuscribirse para evitar memory leaks
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.actions["CycleWeapon"].performed -= OnCycleWeapon;
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
                if (!isShootingLoopActive)
                {
                    shootingLoopCoroutine = StartCoroutine(ShootingLoopCoroutine());
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

    private const float shootCooldown = 1.2f;

    // Corrutina que maneja el loop de disparo
    private IEnumerator ShootingLoopCoroutine()
    {
        isShootingLoopActive = true;

        // Si se ha disparado recientemente
        // Esperar el tiempo que queda del cooldown antes del primer disparo
        float timeSinceLast = Time.time - lastShootTime;
        if (timeSinceLast < shootCooldown)
            yield return new WaitForSeconds(shootCooldown - timeSinceLast);

        while (isShootingPressed)
        {
            ThrowNormalBall();
            lastShootTime = Time.time;
            yield return new WaitForSeconds(shootCooldown);
        }

        isShootingLoopActive = false;
        shootingLoopCoroutine = null;
    }

    // Detiene el loop de disparo
    private void StopShootingLoop()
    {
        if (shootingLoopCoroutine != null)
        {
            StopCoroutine(shootingLoopCoroutine);
            shootingLoopCoroutine = null;
        }
        isShootingLoopActive = false;

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
        shootingLoopCoroutine = null;
        normalBulletReloadCoroutine = null;
    }

    public void OnSpecial(InputAction.CallbackContext contextSpecial)
    {
        if (contextSpecial.performed && UIGameplay.uI != null && UIGameplay.uI.IsSpecialReady(currentSpecialBullet))
        {
            bool wasShootingPressed = isShootingPressed;
            // Parar el loop normal para que no se solape con el especial
            isShootingPressed = false;
            StopShootingLoop();

            ThrowSpecialBall(); // Lanza la bala especial

            // Si el jugador seguía con el clic pulsado, reanudar el loop después de 1.5s
            if (wasShootingPressed)
            {
                isShootingPressed = true; // Mantener la intención del jugador
                StartCoroutine(ResumeShootingAfterSpecial());
            }
        }
    }

    // Corrutina para reanudar el disparo normal después del disparo especial
    private IEnumerator ResumeShootingAfterSpecial()
    {
        yield return new WaitForSeconds(shootCooldown);

        // Marcar el tiempo como "ahora" para que ShootingLoopCoroutine no añada espera extra
        lastShootTime = Time.time - shootCooldown;

        // Solo reanudar si el jugador sigue con el botón pulsado y no hay ya un loop activo
        if (isShootingPressed && !isShootingLoopActive)
        {
            shootingLoopCoroutine = StartCoroutine(ShootingLoopCoroutine());
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

            //else if (binding.Value.path == K2.path)
            //{
            //    currentSpecial = 2;
            //    currentSpecialBullet = currentHability.ChangeHability(1);
            //    Debug.Log("2Spec");
            //}
            //else if (binding.Value.path == K3.path)
            //{
            //    currentSpecial = 3;
            //    currentSpecialBullet = currentHability.ChangeHability(2);
            //    Debug.Log("3Spec");
            //}

            changeWeapon.UpdateActive(currentSpecial);

            Debug.Log(binding.Value);
            Debug.Log(currentHability.currentPositionHability);

        }
    }

    // Suscrito por código en Start a "CycleWeapon".performed
    public void OnCycleWeapon(InputAction.CallbackContext contextCycle)
    {
        if (uiGameplay.isPaused || uiGameplay.isUpgradeMenuOpen) return;

        GenerateBullet currentHability = GenerateBullet.instance;
        var unlocked = currentHability.specialBullets; // Lista de SpecialBullets desbloqueadas

        // Si no hay ninguna desbloqueada, no hacer nada
        if (unlocked == null || unlocked.Count == 0) return;

        // Buscar en qué posición de la lista está el especial actual
        int currentPos = unlocked.IndexOf(currentSpecialBullet);

        // Avanzar una posición (si no estaba en la lista, empieza en 0)
        int nextPos = (currentPos + 1) % unlocked.Count;

        SpecialBullets next = unlocked[nextPos];

        // Actualizar el estado interno igual que hace OnChangeSpecial
        currentSpecialBullet = currentHability.ChangeHability(nextPos);

        // Actualizar el visual — nextPos + 1 porque UpdateActive espera 1/2/3
        changeWeapon.UpdateActive(nextPos + 1);

        Debug.Log($"Ciclo → {next} (slot {nextPos + 1})");
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
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequenio valor negativo, no 0, para mantener isGrounded estable
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
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
        AudioManager.Instance.PlaySFX("DanoJugador");
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
    public SpecialBullets GetCurrentSpecialBullet() { return currentSpecialBullet; }

    public void SetHealthMax(float healtUp) { MaxHealth = healtUp; }
    public void SetCurrentHealth(float currentHealtUp) { playerHealth = currentHealtUp; }
    public void SetDefense(float defenseUp) { defense = defenseUp; }
    public void SetSpeed(float speedUp) { speed = speedUp; }
    public void SetCurrentSpecialBullet(SpecialBullets sP) { currentSpecialBullet = sP; }
    #endregion
}
