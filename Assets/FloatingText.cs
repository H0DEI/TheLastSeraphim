using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI texto;
    CanvasGroup grupo;
    RectTransform rt;

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
                seq.Append(rt.DOShakeAnchorPos(0.4f, strength: 20f, vibrato: 25))
                   .Join(grupo.DOFade(0, 0.6f).SetDelay(0.15f));
                break;

            case FloatingTextTipo.Resistido:
                seq.Append(rt.DOScale(esc * 1.4f, 0.15f).SetEase(Ease.OutBack))
                   .Join(rt.DOAnchorPosY(90f, 1f).SetRelative().SetEase(Ease.OutQuad))
                   .Join(grupo.DOFade(0, 1f));
                break;

            default: // Cura, etc.
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
