using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    //variable
    public int hp = 10;
    public Transform player;
    public CharacterController controller;
    public float speed = 3f;
    public float trunSpeed = 5f;
    //numero random

    //metodos
    void Start() { }
    void Update() 
    { 
        Movemetn();
    }

    void Movemetn()
    { 
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.forward = Vector3.RotateTowards(transform.forward, direction, trunSpeed + Time.deltaTime, 0f);
        controller.Move(transform.forward * speed * Time.deltaTime);
    }


}
