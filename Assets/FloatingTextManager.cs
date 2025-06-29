using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [Header("Canvas donde se instanciarán los textos")]
    public Canvas canvasObjetivo;           // ← arrástralo en el Inspector

    public FloatingText prefabTexto;
    readonly Queue<FloatingText> pool = new();

    FloatingText Obtener()
    {
        if (pool.Count > 0) return pool.Dequeue();

        // Instanciar COMO HIJO del Canvas (muy importante el false)
        GameObject go = Instantiate(prefabTexto.gameObject, canvasObjetivo.transform, false);
        return go.GetComponent<FloatingText>();
    }

    public void MostrarTexto(string valor, Vector3 mundo, Color color, float escala = 1f)
    {
        var txt = Obtener();
        txt.gameObject.SetActive(true);
        txt.Mostrar(valor, mundo, color, escala, this);   // paso ‘this’ si usas pooling
    }

    public void Liberar(FloatingText t) => pool.Enqueue(t);
}
