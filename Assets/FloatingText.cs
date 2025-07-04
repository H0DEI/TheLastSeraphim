using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI texto;
    CanvasGroup grupo;
    RectTransform rt;

    /* ───────── 1 · VARIABLES EXTERNAS ───────── */
    [Header("Animación · CURA")]
    [SerializeField] float curaPopScale = 8f;   // tamaño inicial relativo (pop)
    [SerializeField] float curaPopTime = 0.5f;  // s que tarda en pasar de grande → normal
    [SerializeField] float curaRiseDist = 80f;    // px que sube
    [SerializeField] float curaRiseTime = 1f;  // s que tarda en subir
    [SerializeField] float curaFadeTime = 1.5f;  // s del fade-out


    void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
    }

    public void Mostrar(string valor,
                    Vector3 worldPos,
                    Color col,
                    float esc,
                    System.Action onFinish,
                    FloatingTextTipo tipo = FloatingTextTipo.Daño)
    {
        /* ───────────── Configuración base ───────────── */
        texto.text = valor;
        texto.color = col;
        grupo.alpha = 1f;
        rt.localScale = Vector3.one * esc;

        /* ─────── World → coordenada local en Canvas ──── */
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        Vector3 screen = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screen,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 local);

        rt.anchoredPosition = local;   // ⚠️ posición perfecta, como ya funciona

        /* ───────────── Animación por tipo ───────────── */
        DOTween.Kill(rt);              // por si llega del pool con tweens previos
        var seq = DOTween.Sequence();

        switch (tipo)
        {
            case FloatingTextTipo.Daño:
                {
                    /* ─ Configuración básica ───────────────────────── */
                    float speed = 1.8f;          // 1 = normal · >1 = más lento · <1 = más rápido

                    float escBig = esc * 4f;      // aparece enorme
                    float escNormal = esc * 1.0f;    // tamaño objetivo
                    float escShrink = esc * 0.9f;    // ligera contracción final

                    // ▸ Aparece grande y con alpha 0
                    rt.localScale = Vector3.one * escBig;
                    grupo.alpha = 0f;

                    /* ─ Secuencia ──────────────────────────────────── */
                    seq.Append(grupo.DOFade(1f, 0.12f * speed));                    // 1 · fade-in
                    seq.Append(rt.DOScale(escNormal, 0.12f * speed)                 // 2 · encoge a normal
                                   .SetEase(Ease.InQuad));
                    seq.Append(rt.DOScale(escShrink, 0.25f * speed)                 // 3 · blanco + shrink + fade suave
                                   .SetEase(Ease.OutQuad))
                       .Join(texto.DOColor(Color.white, 0.25f * speed))
                       .Join(grupo.DOFade(0.5f, 0.25f * speed));
                    seq.Append(grupo.DOFade(0f, 0.12f * speed));                    // 4 · fade-out rápido

                    break;
                }
            case FloatingTextTipo.Critico:
                seq.Append(rt.DOScale(esc * 1.8f, 0.12f).SetEase(Ease.OutBack))
                   .Join(rt.DOPunchScale(Vector3.one * 0.2f, 0.35f, 4))
                   .Join(rt.DOAnchorPosY(110f, 1.2f).SetRelative().SetEase(Ease.OutQuad))
                   .Join(grupo.DOFade(0, 1.2f).SetDelay(0.1f));
                break;

            case FloatingTextTipo.Fallo:
                {
                    /* ─ Configuración básica ───────────────────────── */
                    float speed = 1.8f;          // 1 = normal · >1 = más lento · <1 = más rápido
                    float escBig = esc * 1.5f;    // tamaño inicial
                    float escSmall = esc * 0.7f;    // tamaño final (ligera contracción)
                    float fallDist = -120f;          // píxeles hacia abajo (negativo = baja)

                    // ▸ Arranca grande y transparente
                    rt.localScale = Vector3.one * escBig;
                    grupo.alpha = 0f;

                    /* ─ Secuencia ──────────────────────────────────── */
                    seq.Append(grupo.DOFade(1f, 0.10f * speed));                      // 1 · fade-in rápido

                    // 2 · encoge, cae y se desvanece
                    seq.Append(rt.DOScale(escSmall, 0.45f * speed)
                                 .SetEase(Ease.OutQuad))
                       .Join(rt.DOAnchorPosY(fallDist, 0.45f * speed)  // ← cae
                                 .SetRelative()
                                 .SetEase(Ease.InQuad))
                       .Join(grupo.DOFade(0f, 0.45f * speed));         // ← fade-out simultáneo

                    break;
                }

            case FloatingTextTipo.Salvacion:
                {
                    /* ─ Configuración básica ───────────────────────── */
                    float speed = 1.8f;          // 1 = normal · >1 = más lento · <1 = más rápido
                    float escBig = esc * 2f;    // tamaño inicial
                    float escSmall = esc * 0.7f;    // tamaño final
                    float riseDist = 80f;           // píxeles hacia arriba (+Y)

                    // ▸ Arranca grande y transparente
                    rt.localScale = Vector3.one * escBig;
                    grupo.alpha = 0f;

                    /* ─ Secuencia ──────────────────────────────────── */
                    seq.Append(grupo.DOFade(1f, 0.10f * speed));                      // 1 · fade-in rápido

                    // 2 · encoge, sube y se desvanece
                    seq.Append(rt.DOScale(escSmall, 0.45f * speed)
                                 .SetEase(Ease.OutQuad))
                       .Join(rt.DOAnchorPosY(riseDist, 0.45f * speed)  // ← sube
                                 .SetRelative()
                                 .SetEase(Ease.OutQuad))
                       .Join(grupo.DOFade(0f, 0.45f * speed));         // ← fade-out simultáneo

                    break;
                }


            case FloatingTextTipo.Resistido:
                {
                    /* ─ Configuración básica ───────────────────────── */
                    float speed = 1.8f;
                    float escBig = esc * 1f;
                    float escShrink = esc * 0.9f;

                    // ▸ Empieza grande y transparente
                    rt.localScale = Vector3.one * escBig;
                    grupo.alpha = 0f;

                    /* ─ Secuencia ──────────────────────────────────── */

                    // 1 · Fade-in + tambaleo inicial
                    seq.Append(grupo.DOFade(1f, 0.15f * speed));
                    seq.Join(rt.DOShakeAnchorPos(0.4f * speed,
                        strength: 25f, vibrato: 15, randomness: 35,
                        snapping: false, fadeOut: true));

                    // 2 · Encoge y fade-out
                    seq.Append(rt.DOScale(escShrink, 0.3f * speed)
                               .SetEase(Ease.InQuad))
                       .Join(grupo.DOFade(0f, 0.3f * speed));

                    break;
                }

            /* ───────── 2 · NUEVO CASE EN EL SWITCH ───────── */
            case FloatingTextTipo.Cura:
                {
                    // ▸ Arranca grande y transparente
                    float escBig = esc * curaPopScale;
                    rt.localScale = Vector3.one * escBig;
                    grupo.alpha = 0f;
                    
                    /* ─ Secuencia ──────────────────────────────── */
                    seq.Append(grupo.DOFade(1f, 0.10f));                       // 1 · fade-in
                    
                    // 2 · pop → normal, sube y luego se desvanece todo a la vez
                    seq.Append(rt.DOScale(esc, curaPopTime)
                                 .SetEase(Ease.OutQuad))
                       .Join(rt.DOAnchorPosY(curaRiseDist, curaRiseTime)
                                 .SetRelative()
                                 .SetEase(Ease.OutQuad))
                       .Join(grupo.DOFade(0f, curaFadeTime));
                    
                    break;



                    ///* ─ Configuración básica ───────────────────────── */
                    //float speed = 2f;          // 1 = normal · >1 = más lento · <1 = más rápido
                    //float escBig = esc * 4f;    // tamaño inicial
                    //float escSmall = esc * 1.7f;    // tamaño final
                    //float riseDist = 80f;           // píxeles hacia arriba (+Y)
                    //
                    //// ▸ Arranca grande y transparente
                    //rt.localScale = Vector3.one * escBig;
                    //grupo.alpha = 0f;
                    //
                    ///* ─ Secuencia ──────────────────────────────────── */
                    //seq.Append(grupo.DOFade(1f, 0.10f * speed));                      // 1 · fade-in rápido
                    //
                    //// 2 · encoge, sube y se desvanece
                    //seq.Append(rt.DOScale(escSmall, 0.45f * speed)
                    //             .SetEase(Ease.OutQuad))
                    //   .Join(rt.DOAnchorPosY(riseDist, 0.45f * speed)  // ← sube
                    //             .SetRelative()
                    //             .SetEase(Ease.OutQuad))
                    //   .Join(grupo.DOFade(0f, 0.45f * speed));         // ← fade-out simultáneo
                    //
                    //break;
                }

            default: // etc.
                seq.Append(rt.DOScale(esc * 1.3f, 0.15f).SetEase(Ease.OutBack))
                   .Join(rt.DOAnchorPosY(90f, 1f).SetRelative().SetEase(Ease.OutQuad))
                   .Join(grupo.DOFade(0, 1f));
                break;
        }

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onFinish?.Invoke();   // vuelve al pool
        });
    }



    public Color ColorPorDefecto => texto.color;
}
