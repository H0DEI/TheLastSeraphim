using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Colócalo en un objeto sin renderer o con renderer.
/// • Busca todos los renderers propios + hijos y genera un contorno.
/// • Copia color/paleta del Outline raíz.
/// • Permite multiplicar o sustituir el grosor con una Override local.
/// </summary>
[DisallowMultipleComponent]
public class OutlineRendererProxy : MonoBehaviour
{
    const string CHILD_NAME = "OutlineRenderer";

    [Header("Thickness Override (optional)")]
    [Tooltip("0  = usa el grosor del personaje\n>0 = sustituye por este valor\n<0 = multiplica (valor absoluto)")]
    public float thicknessOverride = 0.001f;          // 0 = sin override

    void Start()
    {
        var rootOutline = transform.root.GetComponentInChildren<cakeslice.Outline>();
        if (!rootOutline || !rootOutline.palette) { Destroy(this); return; }

        // ───────────────────────── Grosor ─────────────────────────
        float thickness = rootOutline.outlineThickness;
        if (thicknessOverride > 0f) thickness = thicknessOverride;              // sustitución
        else if (thicknessOverride < 0f) thickness *= Mathf.Abs(thicknessOverride);  // multiplicador

        // Material clonado (no toca la paleta)
        Material srcMat = rootOutline.palette.Get(rootOutline.color);
        if (!srcMat) { Destroy(this); return; }
        Material localMat = new Material(srcMat);
        localMat.SetFloat("_OutlineThickness", thickness);

        // ────────────────────── Obtener Renderers ───────────────────
        List<Renderer> renderers = new();
        if (TryGetComponent(out Renderer selfR)) renderers.Add(selfR);
        renderers.AddRange(GetComponentsInChildren<Renderer>(true));

        foreach (Renderer r in renderers)
            CloneRenderer(r, localMat);

        Destroy(this);         // trabajo terminado
    }

    #region helpers
    void CloneRenderer(Renderer src, Material mat)
    {
        if (src.transform.Find(CHILD_NAME)) return;        // evitar duplicados
        GameObject g = new(CHILD_NAME) { layer = src.gameObject.layer };
        g.transform.SetParent(src.transform, false);

        if (src is SkinnedMeshRenderer sk) MakeSkinned(sk, g, mat);
        else if (src is MeshRenderer mr) MakeStatic(mr, g, mat);
        else Destroy(g);
    }

    void Common(Renderer r)
    {
        r.shadowCastingMode = ShadowCastingMode.Off;
        r.receiveShadows = false;
        r.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        foreach (Material m in r.sharedMaterials) if (m) m.renderQueue = 3000;
    }
    void MakeSkinned(SkinnedMeshRenderer src, GameObject g, Material m)
    {
        var r = g.AddComponent<SkinnedMeshRenderer>();
        r.sharedMesh = src.sharedMesh; r.bones = src.bones; r.rootBone = src.rootBone;
        r.updateWhenOffscreen = true; r.sharedMaterial = m; Common(r);
    }
    void MakeStatic(MeshRenderer src, GameObject g, Material m)
    {
        if (!src.TryGetComponent(out MeshFilter mfSrc)) { Destroy(g); return; }
        var mf = g.AddComponent<MeshFilter>(); mf.sharedMesh = mfSrc.sharedMesh;
        var r = g.AddComponent<MeshRenderer>(); r.sharedMaterial = m; Common(r);
    }
    #endregion
}
