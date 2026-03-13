using UnityEngine;
//Este código permite que los iconos de los enemigos en el minimapa no giren de forma rara con respecto al jugador. Mu sencillo
public class MinimapIconsManager : MonoBehaviour
{
    public Transform player;
    
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, player.eulerAngles.y, transform.eulerAngles.z);
    }
}
