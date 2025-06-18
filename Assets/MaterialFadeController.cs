using System.Collections;
using UnityEngine;
using System.Linq;

public class MaterialFadeController : MonoBehaviour
{
    [Header("Objeto que se vuelve transparente (por materiales)")]
    public Renderer objetoADesvanecer;

    [Header("Transición")]
    public float duracion = 1.5f;
    public float valorFinal = 1f;
    public string nombrePropiedad = "_FaderInOut";

    private Coroutine fadeCoroutine;

    public void IniciarFade()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMateriales());
    }

    private IEnumerator FadeMateriales()
    {
        if (objetoADesvanecer == null)
        {
            Debug.LogWarning("No se asignó 'objetoADesvanecer'.");
            yield break;
        }

        // Buscar hijo Overlay_Ch44 recursivamente
        Transform overlayTransform = GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "Overlay_Ch44");

        if (overlayTransform == null)
        {
            Debug.LogWarning("No se encontró el hijo 'Overlay_Ch44'.");
            yield break;
        }

        Renderer overlayRenderer = overlayTransform.GetComponent<Renderer>();
        if (overlayRenderer == null)
        {
            Debug.LogWarning("'Overlay_Ch44' no tiene Renderer.");
            yield break;
        }

        Material[] overlayMaterials = overlayRenderer.materials;
        float[] valoresIniciales = new float[overlayMaterials.Length];

        for (int i = 0; i < overlayMaterials.Length; i++)
        {
            if (overlayMaterials[i].HasProperty(nombrePropiedad))
                valoresIniciales[i] = overlayMaterials[i].GetFloat(nombrePropiedad);
        }

        Material[] materialesDesvanecer = objetoADesvanecer.materials;
        Color[] coloresOriginales = new Color[materialesDesvanecer.Length];

        foreach (var mat in materialesDesvanecer)
            CambiarMaterialAModoFade(mat);

        for (int i = 0; i < materialesDesvanecer.Length; i++)
            coloresOriginales[i] = materialesDesvanecer[i].color;

        float tiempo = 0f;
        while (tiempo < duracion)
        {
            float t = tiempo / duracion;

            // Interpolar FaderInOut
            for (int i = 0; i < overlayMaterials.Length; i++)
            {
                if (overlayMaterials[i].HasProperty(nombrePropiedad))
                {
                    float nuevoValor = Mathf.Lerp(valoresIniciales[i], valorFinal, t);
                    overlayMaterials[i].SetFloat(nombrePropiedad, nuevoValor);
                }
            }

            // Interpolar alpha de color
            for (int i = 0; i < materialesDesvanecer.Length; i++)
            {
                Color c = coloresOriginales[i];
                c.a = Mathf.Lerp(coloresOriginales[i].a, 100f / 255f, t);
                materialesDesvanecer[i].color = c;
            }

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar valores finales
        for (int i = 0; i < overlayMaterials.Length; i++)
        {
            if (overlayMaterials[i].HasProperty(nombrePropiedad))
            {
                overlayMaterials[i].SetFloat(nombrePropiedad, valorFinal); // Se mantiene
                overlayMaterials[i].SetFloat(nombrePropiedad, 0f); // ← si quieres que vuelva a 0, descomenta esto
            }
        }

        for (int i = 0; i < materialesDesvanecer.Length; i++)
        {
            Color c = materialesDesvanecer[i].color;
            c.a = 100f / 255f;
            materialesDesvanecer[i].color = c;
        }
    }

    private void CambiarMaterialAModoFade(Material mat)
    {
        if (mat == null) return;

        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Refrescar color base por seguridad
        if (mat.HasProperty("_BaseColor"))
        {
            Color c = mat.GetColor("_BaseColor");
            mat.SetColor("_BaseColor", new Color(c.r, c.g, c.b, c.a));
        }

        if (mat.HasProperty("_Color"))
        {
            Color c = mat.GetColor("_Color");
            mat.SetColor("_Color", new Color(c.r, c.g, c.b, c.a));
        }
    }
}
