using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class HUDTextAnimator : MonoBehaviour
{
    [SerializeField] private float bounceScale = 1.2f;
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private Color flashColor = Color.yellow;
    [SerializeField] private float flashDuration = 0.15f;

    private TMP_Text tmpText;
    private string lastText;
    private Vector3 originalScale;
    private Color originalColor;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        originalScale = transform.localScale;
        originalColor = tmpText.color;
        lastText = tmpText.text;
    }

    private void Update()
    {
        if (tmpText.text != lastText)
        {
            Animate();
            lastText = tmpText.text;
        }
    }

    private void Animate()
    {
        // Bounce
        transform.DOKill();
        transform.localScale = originalScale;
        transform.DOScale(originalScale * bounceScale, bounceDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.DOScale(originalScale, bounceDuration).SetEase(Ease.InBack);
        });

        // Flash
        tmpText.DOKill();
        tmpText.color = flashColor;
        tmpText.DOColor(originalColor, flashDuration);
    }
}
