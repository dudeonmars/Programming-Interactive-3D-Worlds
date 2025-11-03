using UnityEngine;
using System.Collections;

public class NPC_movement : MonoBehaviour
{
    public float speed = 0.5f;
    public GameObject enemyPrefab; // Slot for the enemy prefab
    private bool isIlluminated = false;
    private float illuminatedTime = 0f;
    private Light playerLight;
    private bool isDying = false;
    private float spawnTimer = 0f;
    private Vector3 baseSpawnPosition = new Vector3(-0.75f, 0.64f, 0f);

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerLight = player.GetComponentInChildren<Light>();
    }

    void Update()
    {
        if (isDying) return;

        // Handle enemy spawning
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 2f && enemyPrefab != null)
        {
            Vector3 spawnPos = baseSpawnPosition;
            spawnPos.z = Random.Range(-2f, 2f);
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            spawnTimer = 0f;
        }

        CheckIllumination();

        if (!isIlluminated)
        {
            // Move slowly in the x direction
            transform.position += Vector3.right * speed * Time.deltaTime;
            illuminatedTime = 0f;
        }
        else
        {
            illuminatedTime += Time.deltaTime;
            if (illuminatedTime >= 1f)
            {
                StartCoroutine(DeathSequence());
            }
        }
    }

    void CheckIllumination()
    {
        isIlluminated = false;

        if (playerLight == null || !playerLight.enabled || playerLight.type != LightType.Spot)
            return;

        Vector3 lightPos = playerLight.transform.position;
        Vector3 dirToNPC = (transform.position - lightPos).normalized;
        float distance = Vector3.Distance(lightPos, transform.position);

        if (distance > playerLight.range) return;

        float angle = Vector3.Angle(playerLight.transform.forward, dirToNPC);
        if (angle > playerLight.spotAngle * 0.5f) return;

        if (Physics.Raycast(lightPos, dirToNPC, out RaycastHit hit, distance))
        {
            if (hit.transform == transform)
            {
                isIlluminated = true;
            }
        }
    }

    IEnumerator DeathSequence()
    {
        isDying = true;

        // Create flash light effect
        GameObject flashLight = new GameObject("FlashLight");
        flashLight.transform.position = transform.position;
        Light flash = flashLight.AddComponent<Light>();
        flash.type = LightType.Point;
        flash.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Eerie green
        flash.intensity = 8f;
        flash.range = 5f;

        // Create particle system
        GameObject particles = new GameObject("DeathParticles");
        particles.transform.position = transform.position;
        ParticleSystem ps = particles.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = 2f;
        main.startSize = 0.05f;
        main.maxParticles = 30;
        main.startColor = new Color(0.2f, 0.8f, 0.2f, 1f); // Eerie green
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // Set renderer to make particles round and properly colored
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.material.color = Color.green;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = 1f; // Move in +x direction

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 30)
        });

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;

        // Hide the NPC mesh
        GetComponent<Renderer>().enabled = false;

        // Flash effect - quickly fade the light
        float flashDuration = 0.1f;
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            flash.intensity = Mathf.Lerp(8f, 0f, elapsed / flashDuration);
            yield return null;
        }

        Destroy(flashLight);

        // Wait for particles to fade
        yield return new WaitForSeconds(1f);

        // Clean up
        Destroy(particles);
        Destroy(gameObject);
    }
}