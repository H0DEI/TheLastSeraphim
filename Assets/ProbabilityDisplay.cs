using System;
using TMPro;
using UnityEngine;
using System.Collections;

#if DOTWEEN
using DG.Tweening;
#endif

/// <summary>
/// Muestra un porcentaje sobre la barra de vida y aplica:
///  • Color dinámico (gradiente + outline) según el valor.
///  • "Punch" de escala cada vez que el objeto se activa.
/// Asigna este script al mismo GameObject que contiene el TextMeshProUGUI.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class ProbabilityDisplay : MonoBehaviour
{
    [Header("Umbrales (%)")]
    [SerializeField] private int midThreshold = 50; // 50‑79%
    [SerializeField] private int highThreshold = 80; // 80‑100%

    [Header("Animación Punch")]
    [SerializeField] private float punchScale = 0.15f;
    [SerializeField] private float punchDuration = 0.25f;

    [Header("Outline")]
    [SerializeField] private float outlineThickness = 0.3f;

    private TextMeshProUGUI _tmp;
    private Vector3 _baseScale;

    #region Gradientes predefinidos
    // — Verde alto (≥ highThreshold) —
    private static readonly VertexGradient GRAD_HIGH = new VertexGradient(
        new Color32(0xC7, 0xFF, 0xE9, 255), // TL
        new Color32(0x9A, 0xFF, 0xD0, 255), // TR
        new Color32(0x36, 0xD9, 0x9F, 255), // BL
        new Color32(0x12, 0xC8, 0x87, 255)  // BR
    );
    private static readonly Color32 OUT_HIGH = new Color32(0x01, 0x5D, 0x42, 255);

    // — Amarillo medio (midThreshold–highThreshold‑1) —
    private static readonly VertexGradient GRAD_MID = new VertexGradient(
        new Color32(0xFF, 0xF3, 0xB0, 255),
        new Color32(0xFF, 0xE2, 0x8A, 255),
        new Color32(0xFF, 0xB5, 0x40, 255),
        new Color32(0xF8, 0x9A, 0x20, 255)
    );
    private static readonly Color32 OUT_MID = new Color32(0x8A, 0x4E, 0x00, 255);

    // — Rojo bajo (< midThreshold) —
    private static readonly VertexGradient GRAD_LOW = new VertexGradient(
        new Color32(0xFF, 0xD6, 0xD6, 255),
        new Color32(0xFF, 0xB8, 0xB8, 255),
        new Color32(0xFF, 0x5E, 0x5E, 255),
        new Color32(0xE8, 0x38, 0x38, 255)
    );
    private static readonly Color32 OUT_LOW = new Color32(0x78, 0x0B, 0x0B, 255);

    #endregion

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
        _baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        // Asegura color base blanco (evita que vertex color herede otro tint)
        _tmp.color = Color.white;
        _tmp.outlineWidth = outlineThickness;

        ApplyGradientByValue(ParseProbability(_tmp.text));
        PlayPunch();
    }

    /// <summary>
    /// Lee un string tipo "74%" y devuelve 74. Devuelve -1 si no puede parsear.
    /// </summary>
    private int ParseProbability(string txt)
    {
        if (string.IsNullOrEmpty(txt)) return -1;
        txt = txt.Replace("%", string.Empty);
        return int.TryParse(txt, out int value) ? value : -1;
    }

    private void ApplyGradientByValue(int value)
    {
        if (value >= highThreshold)
        {
            _tmp.colorGradient = GRAD_HIGH;
            _tmp.outlineColor = OUT_HIGH;
        }
        else if (value >= midThreshold)
        {
            _tmp.colorGradient = GRAD_MID;
            _tmp.outlineColor = OUT_MID;
        }
        else if (value >= 0)
        {
            _tmp.colorGradient = GRAD_LOW;
            _tmp.outlineColor = OUT_LOW;
        }
    }

    private void PlayPunch()
    {
#if DOTWEEN
        transform.DOKill();
        transform.localScale = _baseScale;
        transform.DOPunchScale(Vector3.one * punchScale, punchDuration, 4, 0f);
#else
        StartCoroutine(PunchCoroutine());
#endif
    }

    #region Coroutine punch fallback (sin DOTween)
    private IEnumerator PunchCoroutine()
    {
        float halfDur = punchDuration * 0.5f;
        float t = 0f;
        while (t < halfDur)
        {
            t += Time.deltaTime;
            float k = t / halfDur;
            transform.localScale = Vector3.Lerp(_baseScale, _baseScale * (1f + punchScale), k);
            yield return null;
        }
        t = 0f;
        while (t < halfDur)
        {
            t += Time.deltaTime;
            float k = t / halfDur;
            transform.localScale = Vector3.Lerp(_baseScale * (1f + punchScale), _baseScale, k);
            yield return null;
        }
        transform.localScale = _baseScale;
    }
    #endregion
}
