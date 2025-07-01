using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Widget que muestra el daño total durante la habilidad.
///  • Primera vez: se desliza desde la derecha.
///  • Cada golpe: “pulso ñam”.
///  • Al acabar la habilidad: fade-out.
/// </summary>
public class TotalDamageDisplay : MonoBehaviour
{
    /*────────────────  Ajustes en Inspector  ────────────────*/

    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI txtTotal;

    [Header("Slide-in inicial")]
    [SerializeField] private float slidePx = 320f;
    [SerializeField] private float slideTime = 0.30f;

    [Header("Pulso «ñam»")]
    [SerializeField] private float pulsoEscala = 1.25f;
    [SerializeField] private float pulsoTime = 0.15f;
    [SerializeField] private float pulsoEase = 0.35f;   // 0 – 1 (parte del tiempo para volver)

    [Header("Fade-out final")]
    [Tooltip("Segundos que espera antes de empezar a desvanecerse")]
    [SerializeField] private float fadeOutDelay = 1.1f;   // << alarga aquí
    [SerializeField] private float fadeOutTime = 1f;

    /*────────────────  Campos privados  ─────────────────────*/

    CanvasGroup grupo;
    Vector2 posBase;
    Tween fadeTween;

    int total = 0;
    bool shown = false;

    /*────────────────  Inicialización  ──────────────────────*/

    void Awake()
    {
        grupo = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        grupo.alpha = 0f;

        txtTotal.text = "";
        posBase = ((RectTransform)transform).anchoredPosition;
        gameObject.SetActive(false);
    }

    /*────────────────  API  ────────────────────────────────*/

    public void Resetear()
    {
        DOTween.Kill(txtTotal.transform);
        grupo.DOKill();
        fadeTween?.Kill();

        total = 0;
        shown = false;
        txtTotal.text = "";
        grupo.alpha = 0f;

        ((RectTransform)transform).anchoredPosition = posBase;
        gameObject.SetActive(false);
    }

    public void Añadir(int cantidad)
    {
        // cancela un fade-out pendiente
        fadeTween?.Kill();
        fadeTween = null;

        total += cantidad;
        txtTotal.text = total.ToString();

        var rt = (RectTransform)transform;

        if (!shown)                       // primera vez → slide-in
        {
            shown = true;
            gameObject.SetActive(true);
            grupo.alpha = 1f;

            rt.anchoredPosition = posBase + Vector2.right * slidePx;
            rt.DOAnchorPos(posBase, slideTime).SetEase(Ease.OutQuart);
        }
        else
        {
            // Asegura que nunca se desplace
            rt.anchoredPosition = posBase;
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                grupo.alpha = 1f;
            }
        }

        // pulso
        txtTotal.transform.DOKill();
        DOTween.Sequence()
               .Append(txtTotal.transform.DOScale(pulsoEscala, pulsoTime))
               .Append(txtTotal.transform.DOScale(1f, pulsoTime * pulsoEase))
               .SetEase(Ease.OutQuad);
    }

    /// <summary>Llámalo al terminar la habilidad.</summary>
    public void FadeOut()
    {
        fadeTween?.Kill();
        fadeTween = grupo.DOFade(0f, fadeOutTime)
                         .SetDelay(fadeOutDelay)
                         .OnComplete(() => gameObject.SetActive(false));
    }
}
