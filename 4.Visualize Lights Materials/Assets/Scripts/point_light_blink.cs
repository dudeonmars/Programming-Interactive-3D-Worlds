using UnityEngine;

public class point_light_blink : MonoBehaviour
{
    Light pointLight;
    float timer = 0f;
    float nextBlinkTime = 0f;
    bool isLightOn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointLight = GetComponent<Light>();
        ScheduleNextBlink();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextBlinkTime)
        {
            isLightOn = !isLightOn;
            if (pointLight != null)
                pointLight.enabled = isLightOn;

            // If turning off, stay off for a short, random time (simulate flicker)
            if (!isLightOn)
                nextBlinkTime = Random.Range(0.05f, 0.2f);
            else // If turning on, stay on for a random time (erratic)
                nextBlinkTime = Random.Range(0.1f, 1.2f);

            timer = 0f;
        }
    }

    void ScheduleNextBlink()
    {
        nextBlinkTime = Random.Range(0.1f, 1.2f);
    }
}
