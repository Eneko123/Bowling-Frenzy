using UnityEngine;

public class PowerUps : MonoBehaviour
{
    MainCharacter player;
    EnemyBase enemis;



    void UpHealth()
    {
        float newMaxHealth = player.GetHealthMax() + player.GetHealthMax() * 0.1f;
        player.SetHealthMax(newMaxHealth);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
