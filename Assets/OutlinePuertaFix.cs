using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OutlinePuertaFix : MonoBehaviour
{
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
        {
            // Forzamos la generación del outline y su visibilidad inicial en false
            outline.SetOutlineColor(outline.color);
            outline.SetOutlineVisible(false);

            Debug.Log($"[OutlinePuertaFix] Outline inicializado en {name}");
        }
        else
        {
            Debug.LogWarning($"[OutlinePuertaFix] No se encontró componente Outline en {name}");
        }
    }

    // Llamar a esta función desde GameManager en vez de acceder directo al Outline
    public void ActivarOutline(bool estado, int color = 2)
    {
        if (outline == null) return;

        outline.SetOutlineColor(color);
        outline.SetOutlineVisible(estado);

        Debug.Log($"[OutlinePuertaFix] Outline {(estado ? "activado" : "desactivado")} en {name} con color {color}");
    }
}
