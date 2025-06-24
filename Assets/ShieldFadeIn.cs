using System.Collections;
using UnityEngine;

/// <summary>
/// Anima la propiedad _FaderInOut de TODOS los materiales del Renderer
/// desde 0 → 1 en el tiempo indicado.
/// Basta con añadir este componente al objeto que ya tiene el material.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class ShieldFadeIn : MonoBehaviour
{
    [Header("Tiempo de la transición (segundos)")]
    [Min(0.1f)]
    public float duracion = 3f;

    [Tooltip("Nombre del parámetro float del shader")]
    public string nombrePropiedad = "_FaderInOut";

    Renderer _rend;
    Coroutine _fx;

    void Awake() => _rend = GetComponent<Renderer>();

    /// <summary>
    /// Lanza el efecto automáticamente cuando el objeto se activa.
    /// Si prefieres llamarlo desde otro script, desactiva StartFadeIn()
    /// y llama públicamente a Iniciar().
    /// </summary>
    void OnEnable() => Iniciar();

    public void Iniciar()
    {
        if (_fx != null) StopCoroutine(_fx);
        _fx = StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        // Guarda valor inicial de cada material (por si el shader no parte de cero)
        var mats = _rend.materials;           // instancia independientes
        var inicio = new float[mats.Length];

        for (int i = 0; i < mats.Length; i++)
            if (mats[i].HasProperty(nombrePropiedad))
                inicio[i] = mats[i].GetFloat(nombrePropiedad);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            float valor = Mathf.Lerp(0f, 1f, t);

            for (int i = 0; i < mats.Length; i++)
                if (mats[i].HasProperty(nombrePropiedad))
                    mats[i].SetFloat(nombrePropiedad, valor);

            yield return null;
        }

        // Asegura valor final exacto
        foreach (var m in mats)
            if (m.HasProperty(nombrePropiedad))
                m.SetFloat(nombrePropiedad, 1f);
    }

    /// <summary> Detiene la animación y reinicia el valor a 0. </summary>
    public void Reiniciar()
    {
        if (_fx != null) StopCoroutine(_fx);

        foreach (var m in _rend.materials)
            if (m.HasProperty(nombrePropiedad))
                m.SetFloat(nombrePropiedad, 0f);
    }
}
