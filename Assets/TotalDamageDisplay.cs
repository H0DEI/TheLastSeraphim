using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Muestra un contador de daño total que
///  • se desliza una sola vez desde (180,-264) → (-80,-264)
///  • acumula los nuevos daños mientras está visible
///  • hace un “pulso-ñam” cada vez que sube el número
///  • se desvanece 1 ,5 s después del último golpe
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class TotalDamageDisplay : MonoBehaviour
{
    /* ────── Inspector ────── */
    [Header("Referencias")]
    [SerializeField] private TextMeshProUGUI txtTotal;   // sólo el número

    [Header("Slide-in")]
    [SerializeField] private float slideStartX = 180f;
    [SerializeField] private float slideEndX = -80f;
    [SerializeField] private float slideY = -264f;
    [SerializeField] private float slideTime = 0.35f;
    [SerializeField] private Ease slideEase = Ease.OutQuart;

    [Header("Pulso «ñam»")]
    [SerializeField] private float pulseScale = 1.25f;
    [SerializeField] private float pulseTime = 0.15f;

    [Header("Persistencia / Fade-out")]
    [SerializeField] private float stayTime = 1.5f;
    [SerializeField] private float fadeTime = 0.4f;

    /* ────── internos ────── */
    RectTransform rt;
    CanvasGroup cg;
    Tween slideTween, fadeTween;
    int acumulado;                     // daño acumulado

    /* ------------------------------------------------------------ */
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        ResetDisplay();                // lo deja oculto y colocado
    }

    /* === API pública ============================================ */
    public void Resetear() => ResetDisplay();

    /// <summary>Añade daño y actualiza la UI.</summary>
    public void Añadir(int cantidad)
    {
        if (cantidad <= 0) return;

        bool primeraVez = acumulado == 0;   // estaba oculto

        acumulado += cantidad;
        txtTotal.text = acumulado.ToString();

        /* si era la primera vez mostramos con slide-in */
        if (primeraVez) Mostrar();
        else Pulso();           // si ya estaba, sólo pulso

        /* re-programa el fade cada vez que llega daño */
        ReprogramarFade();
    }

    /* === implementación ========================================= */
    void Mostrar()
    {
        gameObject.SetActive(true);
        cg.alpha = 1f;

        /* Slide: parte fuera (180) y llega a –80 */
        rt.anchoredPosition = new Vector2(slideStartX, slideY);
        slideTween?.Kill();
        slideTween = rt.DOAnchorPosX(slideEndX, slideTime)
                       .SetEase(slideEase);
    }

    void Pulso()
    {
        txtTotal.transform.DOKill();
        DOTween.Sequence()
               .Append(txtTotal.transform.DOScale(pulseScale, pulseTime))
               .Append(txtTotal.transform.DOScale(1f, pulseTime))
               .SetEase(Ease.OutQuad);
    }

    void ReprogramarFade()
    {
        fadeTween?.Kill();
        fadeTween = cg.DOFade(0f, fadeTime)
                     .SetDelay(stayTime)
                     .OnComplete(ResetDisplay);
    }

    void ResetDisplay()
    {
        slideTween?.Kill();
        fadeTween?.Kill();

        acumulado = 0;
        txtTotal.text = "";
        cg.alpha = 0f;

        /* posición fija final (por si se muestra de nuevo) */
        rt.anchoredPosition = new Vector2(slideEndX, slideY);
        gameObject.SetActive(false);
    }
}
