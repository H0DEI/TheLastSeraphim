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
        SetOutlineVisible(false);
    }

    void Start()
    {
        Debug.Log($"[Outline-DEBUG] {name}: Has MeshFilter? {MeshFilter != null}");
        Debug.Log($"[Outline-DEBUG] {name}: Has Renderer? {Renderer != null}");

    }



    void OnBecameVisible() => _visible = true;
    void OnBecameInvisible() => _visible = false;

    // ─────────────────────────────────────────────
    #region Runtime Control API

    /// <summary>Activa/desactiva todos los OutlineRenderer generados.</summary>
    public void SetOutlineVisible(bool on)
    {
        var renderers = GetAllOutlineRenderers();

        if (renderers.Count == 0)
        {
            Debug.LogWarning($"[Outline] No había renderers en {name}, generando ahora...");
            CreateOutlineRenderer();
            renderers = GetAllOutlineRenderers();
        }

        Debug.Log($"[Outline] SetOutlineVisible({on}) llamado en {name}. Encontrados {renderers.Count} renderers.");
        foreach (var r in renderers)
            r.enabled = on;
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
        var meshRenderer = GetComponent<MeshRenderer>();
        var skinnedRenderer = GetComponent<SkinnedMeshRenderer>();

        if (meshRenderer == null && skinnedRenderer == null)
        {
            Debug.LogWarning($"[Outline] {name} NO tiene MeshRenderer ni SkinnedMeshRenderer");
        }
        else
        {
            Debug.Log($"[Outline] {name} TIENE: " +
                      (meshRenderer != null ? "MeshRenderer " : "") +
                      (skinnedRenderer != null ? "SkinnedMeshRenderer" : ""));
        }

        if (transform.Find(CHILD_NAME) != null) return;
        if (!palette) return;

        Material srcMat = palette.Get(Mathf.Clamp(color, 0, 2));
        if (!srcMat)
        {
            Debug.LogWarning($"{name}: falta material #{color}", this);
            return;
        }

        Material unique = new Material(srcMat);
        unique.SetFloat("_OutlineThickness", outlineThickness);

        GameObject g = new(CHILD_NAME) { layer = gameObject.layer };
        g.transform.SetParent(transform, false);

        if (skinnedRenderer) CloneSkinned(unique, g);
        else if (meshRenderer) CloneStatic(unique, g);
        else
        {
            Debug.LogWarning($"{name}: tipo de renderer no soportado", this);
            Destroy(g);
            return;
        }

        if (eraseRenderer && meshRenderer) meshRenderer.enabled = false;
    }


    void CloneSkinned(Material mat, GameObject g)
    {
        var src = SkinnedMeshRenderer;

        if (src.sharedMesh == null || src.rootBone == null)
        {
            Debug.LogWarning($"[{name}] SkinnedMeshRenderer sin mesh o rootBone, cancelando outline.", this);
            Destroy(g);
            return;
        }

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
        var srcMF = MeshFilter;

        if (!srcMF)
        {
            Debug.LogWarning($"[{name}] no tiene MeshFilter y no puede crear outline estático", this);
            Destroy(g);
            return;
        }

        var mf = g.AddComponent<MeshFilter>();
        mf.sharedMesh = srcMF.sharedMesh;

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

    // Outline.cs  ─ añade dentro de la clase
#if UNITY_EDITOR
    void OnValidate()
    {
        // Si ya hay uno asignado, salimos.
        if (palette != null) return;

        // Busca la primera palette que exista en el proyecto (solo en Editor).
        var all = UnityEditor.AssetDatabase.FindAssets("t:OutlinePalette");
        if (all.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(all[0]);
            palette = UnityEditor.AssetDatabase.LoadAssetAtPath<OutlinePalette>(path);
            UnityEditor.EditorUtility.SetDirty(this);   // marca el cambio
        }
    }
#endif

}
