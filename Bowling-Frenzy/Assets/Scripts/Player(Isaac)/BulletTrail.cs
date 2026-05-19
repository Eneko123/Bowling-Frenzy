using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    void Awake()
    {
        // Añadir TrailRenderer si no existe
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }

        ConfigureTrail();
    }

    void ConfigureTrail()
    {
        trailRenderer.time = 0.5f; // Duración de la estela
        trailRenderer.startWidth = 0.1f;
        trailRenderer.endWidth = 0.01f;

        // Material y color
        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.startColor = Color.yellow;
        trailRenderer.endColor = new Color(1f, 1f, 0f, 0f); // Transparente al final

        // Suavizado
        trailRenderer.numCornerVertices = 5;
        trailRenderer.numCapVertices = 5;
    }
}