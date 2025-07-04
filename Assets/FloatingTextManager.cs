using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabPorTipo
    {
        public FloatingTextTipo tipo;
        public FloatingText prefab;
    }

    [Header("Prefabs por tipo")]
    public PrefabPorTipo[] prefabs;          // asigna en Inspector

    /*────────── pools y catálogo ──────────*/
    readonly Dictionary<FloatingTextTipo, Queue<FloatingText>> pool =
        new();
    readonly Dictionary<FloatingTextTipo, FloatingText> catalogo =
        new();

    void Awake()
    {
        foreach (var p in prefabs)
            if (p.prefab) catalogo[p.tipo] = p.prefab;
    }

    /*────────── instanciación / pool ──────────*/
    FloatingText Obtener(FloatingTextTipo tipo)
    {
        if (!pool.TryGetValue(tipo, out var cola))
        {
            cola = new Queue<FloatingText>();
            pool[tipo] = cola;
        }

        if (cola.Count > 0) return cola.Dequeue();

        Canvas canvas = GetComponentInParent<Canvas>();

        if (!catalogo.TryGetValue(tipo, out var prefab))
        {
            Debug.LogError($"No hay prefab asignado para {tipo}", this);
            return null;
        }

        GameObject go = Instantiate(prefab.gameObject, canvas.transform, false);
        return go.GetComponent<FloatingText>();
    }

    public void Liberar(FloatingTextTipo tipo, FloatingText txt) =>
        pool[tipo].Enqueue(txt);


    /* ─────────── API (con objetivo + delay) ─────────── */
    public void Mostrar(FloatingTextTipo tipo,
                        string valor,
                        Personaje objetivo,
                        Color? colorOverride = null,
                        float escalaOverride = 1f,
                        float delay = 0f)          // ← nuevo parámetro
    {
        if (!objetivo)
        {
            Debug.LogWarning("FloatingTextManager: objetivo es null");
            return;
        }

        Vector3 worldPos = CalcularTorso(objetivo.gameObject.transform);
        Color col = colorOverride ?? Color.white;

        // diferimos la llamada real con DOVirtual.DelayedCall
        DOVirtual.DelayedCall(delay, () =>
        {
            // reutilizamos el overload que usa worldPos
            Mostrar(tipo, valor, worldPos, col, escalaOverride);
        });
    }


    /*────────── API (world-pos) ──────────*/
    public void Mostrar(FloatingTextTipo tipo,
                        string valor,
                        Vector3 mundo,
                        Color? colorOverride = null,
                        float escalaOverride = 1f)
    {
        var txt = Obtener(tipo);
        if (txt == null) return;

        txt.gameObject.SetActive(true);

        Color col = colorOverride ?? txt.ColorPorDefecto;
        float esc = escalaOverride;

        txt.Mostrar(valor, mundo, col, esc,
                    () => Liberar(tipo, txt), tipo);
    }

    /*────────── API (con objetivo) ──────────*/
    public void Mostrar(FloatingTextTipo tipo,
                        string valor,
                        Personaje objetivo,
                        Color? colorOverride = null,
                        float escalaOverride = 1f)
    {
        if (!objetivo)
        {
            Debug.LogWarning("FloatingTextManager: objetivo es null");
            return;
        }

        Vector3 worldPos = CalcularTorso(objetivo.gameObject.transform);
        Mostrar(tipo, valor, worldPos,
                colorOverride ?? Color.white,
                escalaOverride);
    }

    /*────────── cálculo torso + dispersión ──────────*/
    [SerializeField, Range(0f, 1f)]
    float torsoFactor = 0.35f;         // % de la altura desde el centro

    [SerializeField, Range(0f, 1f)]
    float dispersionPctX = 0.30f;      // % de la mitad del ancho

    [SerializeField, Range(0f, 1f)]
    float dispersionPctY = 0.20f;      // % de la mitad del alto

    Vector3 CalcularTorso(Transform objetivo)
    {
        // Punto base
        Vector3 basePos = objetivo.position + Vector3.up;

        if (objetivo.TryGetComponent(out BoxCollider2D bc))
        {
            Bounds b = bc.bounds;

            // Pecho = centro + altura * torsoFactor
            basePos = b.center + Vector3.up * (b.size.y * torsoFactor);

            // Dispersión relativa al tamaño del collider
            float dx = b.extents.x * dispersionPctX;
            float dy = b.extents.y * dispersionPctY;

            // Horizontal respecto a la cámara
            Vector3 camRight = Camera.main.transform.right;
            basePos += camRight * Random.Range(-dx, dx);

            // Vertical en eje Y mundial
            basePos += Vector3.up * Random.Range(-dy, dy);
        }

        return basePos;
    }
}

/*────────── Enum de tipos ──────────*/
public enum FloatingTextTipo
{
    Daño, Critico, Total,      // ← por defecto
    Igneo, IgneoCritico, IgneoTotal,
    Toxico, ToxicoCritico, ToxicoTotal,
    Plasma, PlasmaCritico, PlasmaTotal,
    Fallo, Resistido, Salvacion, Cura
}


public enum ElementoDaño
{
    Ninguno,   // ← se mostrarán los pop-ups por defecto
    Igneo,
    Toxico,
    Plasma
}
