using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class bounce : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ParticleSystem ps;

    [Header("Bounciness by surface")]
    [Range(0f, 1f)] [SerializeField] private float sandBounciness = 0.2f;
    [Range(0f, 1f)] [SerializeField] private float grassBounciness = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float rockBounciness = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float defaultBounciness = 0.3f;

    private void Awake()
    {
        ps = ps != null ? ps : GetComponent<ParticleSystem>();
        var collision = ps.collision;
        collision.enabled = true;
        collision.bounce = defaultBounciness; // base bounce handled by ParticleSystem
    }

    // Single if: react only when tag is one of Sand/Grass/Rock; then set bounce and color.
    private void OnParticleCollision(GameObject other)
    {
        if (!(other.CompareTag("Sand") || other.CompareTag("Grass") || other.CompareTag("Rock"))) return; // only one if

        float bounceValue = other.tag switch
        {
            "Sand" => sandBounciness,
            "Grass" => grassBounciness,
            "Rock" => rockBounciness,
            _ => defaultBounciness
        };

        var collision = ps.collision;
        collision.bounce = bounceValue;



        var rend = other.GetComponent<Renderer>();

        // change the color to yellow, green or gray based on the surface tag
        Color col = other.CompareTag("Sand") ? Color.yellow :
                    other.CompareTag("Grass") ? Color.green :
                    other.CompareTag("Rock") ? Color.gray :
                    (rend != null && rend.sharedMaterial != null) ? rend.sharedMaterial.color : Color.white;

        var main = ps.main;
        main.startColor = col;
    }
}
