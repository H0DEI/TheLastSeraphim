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
    public Sprite cursorHoverDoorLeft;
    public Sprite cursorHoverDoorRight;
    public Sprite cursorHoverCrosshair;

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

        // Colocar cursor con offset
        Vector3 mousePos = Input.mousePosition;
        Vector3 offset = new Vector3(
            currentOffset.x * cursorUI.localScale.x,
            currentOffset.y * cursorUI.localScale.y,
            0f
        );
        cursorUI.position = mousePos + offset;

        UpdateCursorByHover();
    }

    void UpdateCursorByHover()
    {
        Vector3 mousePos = Input.mousePosition;

        // ========== DETECCIÓN 2D ==========
        Ray ray2D = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray2D);

        if (hit2D.collider != null)
        {
            var personaje = hit2D.collider.GetComponentInParent<InteractuarPersonajes>();
            if (personaje != null)
            {
                if (personaje.cursorDesactivado)
                {
                    SetCursor(cursorDefault);
                }
                else if (personaje.elegible)
                {
                    SetCursor(cursorHoverCrosshair); // cursor para personajes elegibles
                }
                else
                {
                    SetCursor(cursorHoverEnemy); // cursor para personajes no elegibles
                }
                return;
            }
        }

        // ========== DETECCIÓN 3D ==========
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit3D, 100f))
        {
            InteractuarPuerta puerta = hit3D.collider.GetComponentInParent<InteractuarPuerta>();
            if (puerta != null)
            {
                if (puerta.puedePresionarse)
                {
                    bool estaALaDerecha = Input.mousePosition.x > Screen.width / 2f;
                    SetCursor(estaALaDerecha ? cursorHoverDoorRight : cursorHoverDoorLeft);
                }
                else
                {
                    SetCursor(cursorDefault);
                }
                return;
            }
        }

        // Si nada detectado, cursor normal
        SetCursor(cursorDefault);
    }


    void SetCursor(Sprite sprite)
    {
        if (sprite == currentSprite) return;

        currentSprite = sprite;
        cursorImage.sprite = sprite;
        cursorImage.color = hudColor;

        float width = sprite.rect.width;
        float height = sprite.rect.height;

        Vector2 pivot = sprite.pivot / sprite.rect.size; // Usa el pivot tal cual, sin invertir Y

        currentOffset = new Vector2(
            (pivot.x - 0.5f) * -width,  // -width invierte X para que 0 sea izquierda
            (pivot.y - 0.5f) * -height  // -height invierte Y para que 0 sea arriba
        );

        cursorUI.sizeDelta = new Vector2(width, height);
    }

}
