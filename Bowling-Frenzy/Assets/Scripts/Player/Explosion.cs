using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionTime = 0.5f;
    [SerializeField] private float explosionScale = 8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        float timer = 0f;
        while (timer < explosionTime)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * explosionScale, timer / explosionTime);
            yield return null;
        }
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetExposionScale()
    {
        return explosionScale;
    }
    public void SetExposionScale(float newScale)
    {
        explosionScale = newScale;
    }
}
