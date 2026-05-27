using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayer : MonoBehaviour
{
    [field: SerializeField] public float sensibilityX; // Se utiliza para fijar la sensibilidad del ratón en el eje X
    [field: SerializeField] public float sensibilityY; // Se utiliza para fijar la sensibilidad del ratón en el eje Y
    //Son variables para la rotacion en los ejes X e Y
    internal float rotationX;
    internal float rotationY;
    //Sirve para rotar al jugador en el eje X
    [SerializeField] Transform player;
    //Sirve para limitar la rotacion en el eje Y
    [SerializeField] float maxY;
    [SerializeField] float minY;

    //Sirve para comprobar si está mirando
    bool isLooking = false;

    // Sirve para congelar la cámara (p.ej. menú de pausa)
    bool isFrozen = false;

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
        if (frozen) isLooking = false; // evita que el último input quede "colgado"
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!BotonTutorial.instance.GetFirstPlayedChecker())
        {
            //Cuando se hace click en la pantalla el ratón pasa la centro de la pantalla
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    public void OnLookInput(InputAction.CallbackContext contextLook)
    {
        if (isFrozen) return;
        if (contextLook.performed)
        {
            isLooking = true;
            rotationY += contextLook.ReadValue<Vector2>().y;
            rotationX += contextLook.ReadValue<Vector2>().x;
        }
    }
    private void LateUpdate()
    {
        if (isLooking)
        {
            //Se limita la rotacion en el angulo Y
            rotationY = AngerOverflow(minY, rotationY, maxY);
            //Se asigna en los vectores el como se quiere hacer la rotacion
            Vector3 rotation_player = new Vector3(0, rotationX * sensibilityX, 0);
            Vector3 rotation_camera = new Vector3(-rotationY * sensibilityY, 0, 0);
            //Se rota al jugador en el eje X y la camara en el eje Y
            player.transform.localEulerAngles = rotation_player;
            this.transform.localEulerAngles = rotation_camera;
        }
    }

    //Sirve para que la camara no pueda hacer giros de más de 60 grados
    float AngerOverflow(float min, float anger, float max)
    {
        //Devolvera el angulo minimo, maximo o el angulo actual del eje Y
        if (anger < min)
        {
            return min;
        }
        if (anger > max)
        {
            return max;
        }
        return anger;
    }
}
