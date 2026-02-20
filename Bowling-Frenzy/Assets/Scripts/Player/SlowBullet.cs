using System.Collections;
using UnityEngine;

public class SlowBullet : NormalBulletBehaviour
{
    bool isSlowing = false;
    float originalSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(transform.position, Vector3.zero);
        currentSpecial = SpecialBullets.Slowing;
        //for (int i = 0; i < GenerateBullet.instance.listOfHabilities.Length; i++)
        //{
        //    if (GenerateBullet.instance.listOfHabilities[i] == null)
        //    {
        //        GenerateBullet.instance.listOfHabilities[i] = this;
        //        break;
        //    }
        //}
    }
  

    // Update is called once per frame
    internal override void CheckEnemy(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.ReceiveDamage(damage);
            enemy.SlowEnemy();
            OnDeactivate();
        }
    }
   
}
