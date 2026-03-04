using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveBulletBehaviour : NormalBulletBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(transform.position, Vector3.zero);
        currentSpecial = SpecialBullets.Explosive;
        for (int i = 0; i < GenerateBullet.instance.listOfHabilities.Length; i++)
        {
            if (GenerateBullet.instance.listOfHabilities[i] == null)
            {
                GenerateBullet.instance.listOfHabilities[i] = this;
                break;
            }
        }
    }

    // Update is called once per frame
    internal SpecialBullets GetSpecialBullet()
    {
        return currentSpecial;
    }

    protected override void OnDeactivate()
    {
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);



        base.OnDeactivate();
    }
}
