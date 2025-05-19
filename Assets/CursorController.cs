using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform cursorUI;
    public Image cursorImage;

    [Header("Sprites de Cursor")]
    public Sprite cursorDefault;
    public Sprite cursorHoverEnemy;
    public Sprite cursorHoverDoor;

    [Header("Opciones")]
    public Color hudColor = Color.cyan;

    private Sprite currentSprite;
    private Vector2 currentOffset;

    void Start()
    {
        Cursor.visible = false;
        SetCursor(cursorDefault);
    }

    void Update()
    {
        if (cursorUI == null || cursorImage == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 offset = new Vector3(
            currentOffset.x * cursorUI.localScale.x,
            currentOffset.y * cursorUI.localScale.y,
            0f
        );

        cursorUI.position = mousePos + offset;

        UpdateCursorByOverlapPoint();
    }

    void UpdateCursorByOverlapPoint()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Ajusta según tu escena

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

        Collider2D hit = Physics2D.OverlapPoint(worldPoint);
        if (hit != null)
        {
            // Enemigo
            InteractuarPersonajes personaje = hit.GetComponentInParent<InteractuarPersonajes>();
            if (personaje != null)
            {
                SetCursor(cursorHoverEnemy);
                return;
            }

            // Puerta
            InteractuarPuerta puerta = hit.GetComponentInParent<InteractuarPuerta>();
            if (puerta != null)
            {
                SetCursor(puerta.puedePresionarse ? cursorHoverDoor : cursorDefault);
                return;
            }
        }

        SetCursor(cursorDefault);
    }

    void SetCursor(Sprite sprite)
    {
        if (sprite == currentSprite) return;

        currentSprite = sprite;
        cursorImage.sprite = sprite;
        cursorImage.color = hudColor;

        // Tamaño del sprite
        float width = sprite.rect.width;
        float height = sprite.rect.height;

        // Pivot del sprite
        Vector2 pivot = sprite.pivot / sprite.rect.size;
        pivot.y = 1f - pivot.y;

        currentOffset = new Vector2(
            (pivot.x - 0.5f) * -width,
            (pivot.y - 0.5f) * -height
        );

        cursorUI.sizeDelta = new Vector2(width, height);
    }
}
