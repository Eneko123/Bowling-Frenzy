using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] float jumpForce = 4f;
    //La gravedad para hacer que el jugador caiga
    [SerializeField] float gravity = -9.8f;
    public CambiaArmasVisualizer changeWeapon;
    public PlayerHealth saludJugador;
    //Sirve para controlar por si el jugador decide dejar de pulsar al completo porque quiere cancelar el salto
    private float jumpTimeStamp;
    private float jumpTime = 0f;

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
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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
        if (!uiGameplay.isPaused && !uiGameplay.isUpgradeMenuOpen)
        {
            if (contextShoot.performed && !isReloadingNormalBullet)
            {
                animator.SetTrigger("isAttacking");
            }
        }
    }
    public void OnSpecial(InputAction.CallbackContext contextSpecial)
    {
        if (contextSpecial.performed && !isReloadingExplosiveBullet && currentSpecialBullet == SpecialBullets.Explosive
            || contextSpecial.performed && !isReloadingPiercingBullet && currentSpecialBullet == SpecialBullets.Piercing
            || contextSpecial.performed && !isReloadingSlowingBullet && currentSpecialBullet == SpecialBullets.Slowing)
        {
            animator.SetTrigger("IsSpecial");
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
    IEnumerator DelayForBullets(float delay)
    {
        switch (typeOfBullet)
        {
            case 0:
                isReloadingNormalBullet = true;
                yield return new WaitForSeconds(delay);
                isReloadingNormalBullet = false;
                break;
            case 1:
                isReloadingExplosiveBullet = true;
                yield return new WaitForSeconds(delay);
                isReloadingExplosiveBullet = false;
                break;
            case 2:
                isReloadingPiercingBullet = true;
                yield return new WaitForSeconds(delay);
                isReloadingPiercingBullet = false;
                break;
            case 3:
                isReloadingSlowingBullet = true;
                yield return new WaitForSeconds(delay);
                isReloadingSlowingBullet = false;
                break;
        }
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
        if (velocity.y > -19.6)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);
    }
    public void RestoreHealthByCombo()
    {
        if (playerHealth < MaxHealth)
        {
            playerHealth *= healthRecovery;
            saludJugador.UpdateHealth(playerHealth, MaxHealth);
        }
    }
    public void damageHealthPlayer(float damage)
    {
        playerHealth -= damage * (1 - defense);
        Debug.Log("Player health decreased");
        saludJugador.UpdateHealth(playerHealth, MaxHealth);
    }
    void ThrowNormalBall()
    {
        GameObject b = GenerateBullet.instance.GetBullets();
        b.GetComponentInChildren<NormalBulletBehaviour>().Init(pointOfShoot.transform.position, cameraPlayer.transform.forward);
        typeOfBullet = 0;
        StartCoroutine(DelayForBullets(0.5f));
    }
    void ThrowSpecialBall()
    {
        GenerateBullet currentHability = GenerateBullet.instance;
        GameObject b = null;
        switch (currentSpecialBullet)
        {
            case SpecialBullets.Explosive:
                if (!isReloadingExplosiveBullet)
                {
                    b = GenerateBullet.instance.SelectTheSpecial(currentSpecialBullet);
                }
                break;
            case SpecialBullets.Piercing:
                if (!isReloadingPiercingBullet)
                {
                    b = GenerateBullet.instance.SelectTheSpecial(currentSpecialBullet);
                }
                break;
            case SpecialBullets.Slowing:
                if (!isReloadingSlowingBullet)
                {
                    b = GenerateBullet.instance.SelectTheSpecial(currentSpecialBullet);
                }
                break;
        }

        if (b != null)
        {
            switch (b.GetComponentInChildren<NormalBulletBehaviour>().GetSpecialBullet())
            {
                case SpecialBullets.Explosive:
                    if (!isReloadingExplosiveBullet)
                    {
                        b.GetComponentInChildren<NormalBulletBehaviour>().Init(pointOfShoot.transform.position, cameraPlayer.transform.forward);
                        typeOfBullet = 1;
                        StartCoroutine(DelayForBullets(10f));
                    }
                    break;
                case SpecialBullets.Piercing:
                    if (!isReloadingPiercingBullet)
                    {
                        b.GetComponentInChildren<NormalBulletBehaviour>().Init(pointOfShoot.transform.position, cameraPlayer.transform.forward);
                        typeOfBullet = 2;
                        StartCoroutine(DelayForBullets(5.5f));
                    }
                    break;
                case SpecialBullets.Slowing:
                    if (!isReloadingSlowingBullet)
                    {
                        b.GetComponentInChildren<NormalBulletBehaviour>().Init(pointOfShoot.transform.position, cameraPlayer.transform.forward);
                        typeOfBullet = 3;
                        StartCoroutine(DelayForBullets(7f));
                    }
                    break;
            }
        }
    }
    #region Geters-Seters
    public float GetHealthMax() { return MaxHealth; }
    public float GetCurrentHealth() { return playerHealth; }
    public float GetDefense() { return defense; }
    public float GetSpeed() { return speed; }

    public void SetHealthMax(float healtUp) { MaxHealth = healtUp; }
    public void SetCurrentHealth(float currentHealtUp) { playerHealth = currentHealtUp; }
    public void SetDefense(float defenseUp) { defense = defenseUp; }
    public void SetSpeed(float speedUp) { speed = speedUp; }
    #endregion
}