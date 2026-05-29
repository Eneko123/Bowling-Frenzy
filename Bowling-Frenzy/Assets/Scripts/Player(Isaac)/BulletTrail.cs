using System.Collections;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private float stelaDuration = 0.5f;
    [SerializeField] private float startWidth = 0.5f;
    [SerializeField] private float endWidth = 0.01f;
    [SerializeField] private Color initialColor;
    [SerializeField] private Color finalColor;

    // Distancia mínima que debe recorrer la bala antes de dibujar estela
    [SerializeField] private float minMoveDistance = 0.5f;

    private TrailRenderer trailRenderer;
    private Vector3 lastPosition;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer == null)
            trailRenderer = gameObject.AddComponent<TrailRenderer>();

        ConfigureTrail();
    }

    void ConfigureTrail()
    {
        trailRenderer.time = stelaDuration;
        trailRenderer.startWidth = startWidth;
        trailRenderer.endWidth = endWidth;

        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.startColor = initialColor;
        trailRenderer.endColor = finalColor;

        trailRenderer.numCornerVertices = 5;
        trailRenderer.numCapVertices = 5;

        trailRenderer.emitting = false; // Empieza siempre apagado
    }

    void OnEnable()
    {
        // Apagar emisión y limpiar puntos viejos
        trailRenderer.emitting = false;
        trailRenderer.Clear();
        lastPosition = transform.position;

        // Activar la estela un frame después, cuando la bala ya está en su posición de disparo
        StartCoroutine(EnableTrailNextFrame());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        trailRenderer.emitting = false;
        trailRenderer.Clear();
    }

    private IEnumerator EnableTrailNextFrame()
    {
        yield return null; // Espera un frame
        trailRenderer.Clear(); // Limpiar de nuevo por si acaso
        lastPosition = transform.position;
        trailRenderer.emitting = true;
    }
}
