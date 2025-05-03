using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ErraticLightFlicker : MonoBehaviour
{
    [Header("Tiempo entre ráfagas")]
    public float minPause = 2f;
    public float maxPause = 5f;

    [Header("Duración de una ráfaga")]
    public float flickerDuration = 1.5f;

    [Header("Frecuencia dentro de la ráfaga")]
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;

    private List<Light> lights = new List<Light>();
    private List<Renderer> emissiveMeshes = new List<Renderer>();
    private Coroutine flickerRoutine;

    private void Start()
    {
        // Buscar todas las luces hijas
        lights.AddRange(GetComponentsInChildren<Light>());

        // Buscar emisores de luz visual (opcional)
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            if (renderer.material.HasProperty("_EmissionColor"))
                emissiveMeshes.Add(renderer);
        }

        flickerRoutine = StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            // Espera aleatoria antes de empezar la ráfaga
            yield return new WaitForSeconds(Random.Range(minPause, maxPause));

            float elapsed = 0f;

            while (elapsed < flickerDuration)
            {
                bool on = Random.value > 0.5f;
                SetLights(on);
                yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));
                elapsed += Time.deltaTime;
            }

            // Al final de la ráfaga, apaga o deja encendido aleatoriamente
            SetLights(Random.value > 0.3f);
        }
    }

    private void SetLights(bool on)
    {
        foreach (var l in lights)
        {
            l.enabled = on;
        }

        foreach (var r in emissiveMeshes)
        {
            if (on)
                r.material.EnableKeyword("_EMISSION");
            else
                r.material.DisableKeyword("_EMISSION");
        }
    }
}
