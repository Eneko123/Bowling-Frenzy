using UnityEngine;

public class uicanmenus : MonoBehaviour
{
    // objetos
    public GameObject pov;
    public GameObject inicio;
    public GameObject pausa;
    public GameObject niveles;
    public GameObject opciones;
    public GameObject Eleccionmejoras;

    //variable
    public bool vieneinicio = true;


    //
    void Start()
    {
        pov.SetActive(false);
        inicio.SetActive(true);//empieza en el menu
        pausa.SetActive(false);   
        niveles.SetActive(false);
        opciones.SetActive(false);
        Eleccionmejoras.SetActive(false);
    }
    void Update()
    {
        if(pov == true)
        {
            //cuando le damos al esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menupausa();
            }
            else { return; }
        }
        
        else { return; }
    
    
    
    }

    //activacion de menus
    public void menuinicio() 
    {
        pov.SetActive(false);
        inicio.SetActive(true);//empieza en el menu
        pausa.SetActive(false);
        niveles.SetActive(false);
        opciones.SetActive(false);
        Eleccionmejoras.SetActive(false);

        vieneinicio = true;
    }
    public void Jugadorpov()
    {
        pov.SetActive(true);//lo que ve el jugador
        inicio.SetActive(false);
        pausa.SetActive(false);
        niveles.SetActive(false);
        opciones.SetActive(false);
        Eleccionmejoras.SetActive(false);
    }
    public void menupausa()
    {
        pov.SetActive(false);
        inicio.SetActive(false);
        pausa.SetActive(true);//saca el menu pausa
        niveles.SetActive(false);
        opciones.SetActive(false);
        Eleccionmejoras.SetActive(false);

        vieneinicio = false;
    }
    public void menuniveles()
    {
        pov.SetActive(false);
        inicio.SetActive(false);
        pausa.SetActive(false);
        niveles.SetActive(true);//se ven los niveles a elegir
        opciones.SetActive(false);
        Eleccionmejoras.SetActive(false);
    }
    public void menuopciones()
    {
        pov.SetActive(false);
        inicio.SetActive(false);
        pausa.SetActive(false);
        niveles.SetActive(false);
        opciones.SetActive(true);//se saca las opciones
        Eleccionmejoras.SetActive(false);
    }
    public void menumejoras()
    {
        pov.SetActive(false);
        inicio.SetActive(true);
        pausa.SetActive(false);
        niveles.SetActive(false);
        opciones.SetActive(false);
        Eleccionmejoras.SetActive(false);//se elije la mejora
    }

    //extra
    public void prueva()
    {
        Debug.Log("funciona");
        Jugadorpov();
    }
    public void Checardondeopciones()
    {
        if (vieneinicio)
        {
            menuinicio();
        }
        else 
        {
            menupausa();
        }
    }
    public void Vueltadepausa()
    {
        
    }
    public void salirjuego()
    {
        Application.Quit();
    }
}
