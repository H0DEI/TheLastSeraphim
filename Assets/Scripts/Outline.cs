using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer)), DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    [Range(0, 2)] public int color = 0;
    public bool eraseRenderer = false;
    [Min(0.00001f)] public float outlineThickness = 0.003f;
    public OutlinePalette palette;

    public Renderer Renderer { get; private set; }
    public SkinnedMeshRenderer SkinnedMeshRenderer { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public MeshFilter MeshFilter { get; private set; }
    public bool IsVisible => _visible;

    bool _visible;
    Material[] _sharedMats;
    Material[] SharedMaterials => _sharedMats ??= Renderer.sharedMaterials;

    const string CHILD_NAME = "OutlineRenderer";

    void Awake()
    {
        Renderer = GetComponent<Renderer>();
        SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        MeshFilter = GetComponent<MeshFilter>();

        CreateOutlineRenderer();
    }

    void OnBecameVisible() => _visible = true;
    void OnBecameInvisible() => _visible = false;

    // ─────────────────────────────────────────────
    #region Runtime Control API

    /// <summary>Activa/desactiva todos los OutlineRenderer generados.</summary>
    public void SetOutlineVisible(bool on)
    {
        foreach (var r in GetAllOutlineRenderers()) r.enabled = on;
    }

    /// <summary>Reemplaza el color del outline y actualiza los materiales.</summary>
    public void SetOutlineColor(int newColor)
    {
        color = Mathf.Clamp(newColor, 0, 2);
        Material newMat = palette?.Get(color);
        if (!newMat) return;

        foreach (var r in GetAllOutlineRenderers())
        {
            var uniqueMat = new Material(newMat);
            uniqueMat.SetFloat("_OutlineThickness", outlineThickness);
            r.sharedMaterials = Replicate(uniqueMat, r.sharedMaterials.Length);
        }
    }

    List<Renderer> GetAllOutlineRenderers()
    {
        List<Renderer> results = new();

        // 1. El hijo local
        var local = transform.Find(CHILD_NAME);
        if (local && local.TryGetComponent(out Renderer r)) results.Add(r);

        // 2. Todos los OutlineRenderer que estén en hijos de vecinos
        var siblings = transform.parent ? transform.parent.GetComponentsInChildren<Transform>(true) : null;
        if (siblings != null)
        {
            foreach (var t in siblings)
            {
                if (t == transform) continue;
                var children = t.GetComponentsInChildren<Transform>(true);
                foreach (var c in children)
                {
                    if (c.name == CHILD_NAME && c.TryGetComponent(out Renderer rx))
                        results.Add(rx);
                }
            }
        }

        return results;
    }

    #endregion

    // ─────────────────────────────────────────────
    #region builder

    void CreateOutlineRenderer()
    {
        if (transform.Find(CHILD_NAME) != null) return;
        if (!palette) return;

        Material srcMat = palette.Get(Mathf.Clamp(color, 0, 2));
        if (!srcMat) { Debug.LogWarning($"{name}: falta material #{color}", this); return; }

        Material unique = new Material(srcMat);
        unique.SetFloat("_OutlineThickness", outlineThickness);

        GameObject g = new(CHILD_NAME) { layer = gameObject.layer };
        g.transform.SetParent(transform, false);

        if (SkinnedMeshRenderer) CloneSkinned(unique, g);
        else if (Renderer is MeshRenderer) CloneStatic(unique, g);
        else
        {
            Debug.LogWarning($"{name}: tipo de renderer no soportado", this);
            Destroy(g);
            return;
        }

        if (eraseRenderer) Renderer.enabled = false;
    }

    void CloneSkinned(Material mat, GameObject g)
    {
        var src = SkinnedMeshRenderer;
        var clone = g.AddComponent<SkinnedMeshRenderer>();

        clone.sharedMesh = src.sharedMesh;
        clone.bones = src.bones;
        clone.rootBone = src.rootBone;
        clone.updateWhenOffscreen = true;
        clone.sharedMaterials = Replicate(mat, src.sharedMaterials.Length);

        Configure(clone);
    }

    void CloneStatic(Material mat, GameObject g)
    {
        var srcMR = (MeshRenderer)Renderer;

        var mf = g.AddComponent<MeshFilter>();
        mf.sharedMesh = MeshFilter.sharedMesh;

        var mr = g.AddComponent<MeshRenderer>();
        mr.sharedMaterials = Replicate(mat, srcMR.sharedMaterials.Length);

        Configure(mr);
    }

    static void Configure(Renderer r)
    {
        r.shadowCastingMode = ShadowCastingMode.Off;
        r.receiveShadows = false;
        r.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        r.allowOcclusionWhenDynamic = false;

        foreach (Material m in r.sharedMaterials)
            if (m) m.renderQueue = 3000;
    }

    static Material[] Replicate(Material source, int count)
    {
        Material[] arr = new Material[count];
        for (int i = 0; i < count; i++) arr[i] = source;
        return arr;
    }

    #endregion
}
