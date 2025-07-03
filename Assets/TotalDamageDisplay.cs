using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Versión sin DOTween.
/// • Coroutines para slide, pulso y fade.
/// • No usa SetActive, sólo CanvasGroup alpha.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class TotalDamageDisplay : MonoBehaviour
{
    /* ────── Inspector ────── */
    [Header("Referencias")]
    [SerializeField] TextMeshProUGUI txtTotal;

    [Header("Slide-in")]
    [SerializeField] float slideStartX = 180f;
    [SerializeField] float slideEndX = -80f;
    [SerializeField] float slideY = -264f;
    [SerializeField] float slideTime = 0.35f;   // segundos

    [Header("Pulso «ñam»")]
    [SerializeField] float pulseScale = 1.25f;
    [SerializeField] float pulseTime = 0.15f;

    [Header("Persistencia / Fade-out")]
    [SerializeField] float stayTime = 1.5f;
    [SerializeField] float fadeTime = 0.4f;

    /* ────── internos ────── */
    RectTransform rt;
    CanvasGroup cg;

    Coroutine slideCR, fadeCR, pulseCR;
    int total;
    bool yaEntrado;          // evita repetir el slide

    /* ───────────────────────────────────────────── */
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        // fijamos anchor/pivot arriba-derecha para coherencia
        rt.anchorMin = rt.anchorMax = Vector2.one;
        rt.pivot = Vector2.one;

        ReiniciarEstado();
    }

    /* === API pública ========================================== */
    public void Resetear() => ReiniciarEstado();

    public void Añadir(int dmg)
    {
        if (dmg <= 0) return;

        total += dmg;
        txtTotal.text = total.ToString();

        if (!yaEntrado)
        {
            if (slideCR != null) StopCoroutine(slideCR);
            slideCR = StartCoroutine(SlideIn());
        }
        else
        {
            if (pulseCR != null) StopCoroutine(pulseCR);
            pulseCR = StartCoroutine(Pulso());
        }

        // re-programa fade cada vez
        if (fadeCR != null) StopCoroutine(fadeCR);
        fadeCR = StartCoroutine(FadeTrasEspera());
    }

    /* === Coroutines =========================================== */

    // Slide-in con easing OutQuart
    IEnumerator SlideIn()
    {
        yaEntrado = true;
        cg.alpha = 1f;
        float t = 0f;
        float start = slideStartX;
        float end = slideEndX;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / slideTime;           // sin afectar por Time.timeScale
            float eased = 1f - Mathf.Pow(1f - t, 4f);          // OutQuart
            float x = Mathf.Lerp(start, end, eased);
            rt.anchoredPosition = new Vector2(x, slideY);
            yield return null;
        }

        // clava posición exacta
        rt.anchoredPosition = new Vector2(end, slideY);
    }

    // Escala ↑ y ↓
    IEnumerator Pulso()
    {
        Transform tr = txtTotal.transform;
        float t = 0f;
        while (t < pulseTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / pulseTime;
            float s = Mathf.Lerp(1f, pulseScale, k);
            tr.localScale = Vector3.one * s;
            yield return null;
        }

        t = 0f;
        while (t < pulseTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / pulseTime;
            float s = Mathf.Lerp(pulseScale, 1f, k);
            tr.localScale = Vector3.one * s;
            yield return null;
        }

        tr.localScale = Vector3.one;
    }

    // Espera, luego desvanece y reinicia
    IEnumerator FadeTrasEspera()
    {
        yield return new WaitForSeconds(stayTime);

        float t = 0f, start = 1f, end = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeTime;
            cg.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        cg.alpha = 0f;
        ReiniciarEstado();
    }

    /* === Helpers ============================================== */
    void ReiniciarEstado()
    {
        // cancela animaciones
        if (slideCR != null) StopCoroutine(slideCR);
        if (fadeCR != null) StopCoroutine(fadeCR);
        if (pulseCR != null) StopCoroutine(pulseCR);

        total = 0;
        yaEntrado = false;
        txtTotal.text = "";
        txtTotal.transform.localScale = Vector3.one;

        // posición lista para la próxima entrada
        rt.anchoredPosition = new Vector2(slideEndX, slideY);
        cg.alpha = 0f;
    }
}
