using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Image bar;      // barra roja
    [SerializeField] Image loss;     // barra blanca

    [Header("Tiempos")]
    [Tooltip("Duración del mordisco rojo")]
    [SerializeField] float durPasoRojo = 0.12f;

    [Tooltip("Cuánto tarda la barra blanca en alcanzar a la roja")]
    [SerializeField] float durFlash = 0.50f;

    [Tooltip("Pausa antes de que la barra blanca empiece a caer")]
    [SerializeField] float pausaPreBlanca = 0.5f;      // ← NUEVO

    public Personaje personaje;

    /* — interna — */
    Coroutine lossRoutine;

    /* -------------------------------------------------------------- */
    public void AnimarHasta(float pctObjetivo, float delay)
    {
        // rojo
        StartCoroutine(RojoTween(pctObjetivo, delay));

        // blanco
        if (lossRoutine != null) StopCoroutine(lossRoutine);
        lossRoutine = StartCoroutine(BlancoTween(pctObjetivo, delay));
    }

    /* ----------------  coroutines privadas  ----------------------- */
    IEnumerator RojoTween(float target, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        float ini = bar.fillAmount;
        float t = 0f;

        while (t < durPasoRojo)
        {
            t += Time.deltaTime;
            bar.fillAmount = Mathf.Lerp(ini, target, t / durPasoRojo);
            yield return null;
        }
        bar.fillAmount = target;
    }

    IEnumerator BlancoTween(float target, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        /* ── NUEVA pausa antes de mover la blanca ── */
        if (pausaPreBlanca > 0f)
            yield return new WaitForSeconds(pausaPreBlanca);

        float ini = loss.fillAmount;
        float t = 0f;

        while (t < durFlash)
        {
            t += Time.deltaTime;
            loss.fillAmount = Mathf.Lerp(ini, target, t / durFlash);
            yield return null;
        }
        loss.fillAmount = target;
    }
}
