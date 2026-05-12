using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public GameObject jump;
    public GameObject atack;

    [SerializeField] float damage;

    // Variables para guardar las posiciones originales
    private Vector3 jumpOriginalLocalPosition;
    private Vector3 atackOriginalLocalPosition;

    // Variables para guardar las posiciones cuando se activan
    private Vector3 jumpWorldPositionOnActivation;
    private Vector3 atackWorldPositionOnActivation;

    // Flags para saber si están activos
    private bool jumpWasActive = false;
    private bool atackWasActive = false;

    private void Awake()
    {
        // Guardar las posiciones locales originales al inicio
        if (jump != null)
        {
            jumpOriginalLocalPosition = jump.transform.localPosition;
        }
        if (atack != null)
        {
            atackOriginalLocalPosition = atack.transform.localPosition;
        }
    }

    private void Update()
    {
        // Detectar cuando jump se activa
        if (jump != null && jump.activeSelf && !jumpWasActive)
        {
            OnJumpActivated();
            jumpWasActive = true;
        }
        else if (jump != null && !jump.activeSelf && jumpWasActive)
        {
            OnJumpDeactivated();
            jumpWasActive = false;
        }

        // Detectar cuando atack se activa
        if (atack != null && atack.activeSelf && !atackWasActive)
        {
            OnAtackActivated();
            atackWasActive = true;
        }
        else if (atack != null && !atack.activeSelf && atackWasActive)
        {
            OnAtackDeactivated();
            atackWasActive = false;
        }

        // Mantener las posiciones fijas mientras están activos
        if (jump != null && jump.activeSelf && jump.transform.parent == null)
        {
            jump.transform.position = jumpWorldPositionOnActivation;
        }

        if (atack != null && atack.activeSelf && atack.transform.parent == null)
        {
            atack.transform.position = atackWorldPositionOnActivation;
        }
    }

    private void OnJumpActivated()
    {
        // Guardar la posición mundial cuando se activa
        jumpWorldPositionOnActivation = jump.transform.position;

        // Desparentar para que no siga al boss
        jump.transform.SetParent(null);

        Debug.Log("Jump activado en posición: " + jumpWorldPositionOnActivation);
    }

    private void OnJumpDeactivated()
    {
        // Volver a ser hijo del boss
        if (GetComponentInParent<BoloEBoos>() != null)
        {
            jump.transform.SetParent(GetComponentInParent<BoloEBoos>().transform);
        }
        else
        {
            jump.transform.SetParent(this.transform);
        }

        // Restaurar posición local original
        jump.transform.localPosition = jumpOriginalLocalPosition;

        Debug.Log("Jump desactivado y restaurado a posición local original");
    }

    private void OnAtackActivated()
    {
        // Guardar la posición mundial cuando se activa
        atackWorldPositionOnActivation = atack.transform.position;

        // Desparentar para que no siga al boss
        atack.transform.SetParent(null);

        Debug.Log("Atack activado en posición: " + atackWorldPositionOnActivation);
    }

    private void OnAtackDeactivated()
    {
        // Volver a ser hijo del boss
        if (GetComponentInParent<BoloEBoos>() != null)
        {
            atack.transform.SetParent(GetComponentInParent<BoloEBoos>().transform);
        }
        else
        {
            atack.transform.SetParent(this.transform);
        }

        // Restaurar posición local original
        atack.transform.localPosition = atackOriginalLocalPosition;

        Debug.Log("Atack desactivado y restaurado a posición local original");
    }

    private void OnDisable()
    {
        // Asegurar que todo vuelve a su estado original cuando se desactiva el script
        if (jump != null && jump.transform.parent == null)
        {
            OnJumpDeactivated();
        }

        if (atack != null && atack.transform.parent == null)
        {
            OnAtackDeactivated();
        }
    }

    void StopAttacks()
    {
        if (jump != null)
        {
            jump.SetActive(false);
            jump.transform.localScale = new Vector3(10f, 0.01f, 10f);
        }

        if (atack != null)
        {
            atack.SetActive(false);
            atack.transform.localScale = new Vector3(10f, 0.01f, 10f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out MainCharacter player))
        {
            player.damageHealthPlayer(damage);
            Debug.Log("Player hit by area attack");
        }
    }
}