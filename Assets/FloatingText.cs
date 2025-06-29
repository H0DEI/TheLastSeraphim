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

    public void Mostrar(string valor, Vector3 worldPos, Color color, float escala,
                    FloatingTextManager manager)
    {
        texto.text = valor;
        texto.color = color;
        grupo.alpha = 1f;
        rt.localScale = Vector3.one * escala;

        // Conversion pantalla → posición UI (Canvas Overlay)
        Vector3 screen = Camera.main.WorldToScreenPoint(worldPos);
        rt.position = screen;                    // ya está en el Canvas correcto

        DOTween.Sequence()
            .Append(rt.DOScale(escala * 1.3f, 0.15f).SetEase(Ease.OutBack))
            .Append(rt.DOAnchorPosY(90f, 1f).SetRelative().SetEase(Ease.OutQuad))
            .Join(grupo.DOFade(0, 1f))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                manager.Liberar(this);           // vuelve al pool
            });
    }

}
