/*
 * Outline  ─ URP runtime clone + ScriptableObject palette
 * (independiente de cakeslice/OutlineEffect)
 */

using UnityEngine;
using UnityEngine.Rendering;

namespace cakeslice
{
    [RequireComponent(typeof(Renderer)), DisallowMultipleComponent]
    public class Outline : MonoBehaviour
    {
        // ───── ajustes expuestos ──────────────────────────────────────────────
        [Range(0, 2)] public int color = 0;
        public bool eraseRenderer = false;
        [Min(0.00001f)] public float outlineThickness = 0.003f;
        public OutlinePalette palette;             // 3 materiales

        // ─── referencias públicas (compatibilidad con tu código antiguo) ─────
        public Renderer Renderer { get; private set; }
        public SkinnedMeshRenderer SkinnedMeshRenderer { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public MeshFilter MeshFilter { get; private set; }
        public bool IsVisible => _visible;

        bool _visible;
        Material[] _sharedMats;
        Material[] SharedMaterials => _sharedMats ??= Renderer.sharedMaterials;

        // ──────────────────────────────────────────────────────────────────────
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

        // ──────────────────────────────────────────────────────────────────────
        #region builder

        void CreateOutlineRenderer()
        {
            if (transform.Find(CHILD_NAME) != null) return;     // ya existe
            if (!palette) return;

            // -- material local (se instancia para no modificar el asset) --
            Material srcMat = palette.Get(Mathf.Clamp(color, 0, 2));
            if (!srcMat) { Debug.LogWarning($"{name}: falta material #{color}", this); return; }

            Material unique = new Material(srcMat);
            unique.SetFloat("_OutlineThickness", outlineThickness); // nombre exacto del shader

            // ---------- hijo fantasma ----------
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
                if (m) m.renderQueue = 3000;               // geometry + 1
        }

        static Material[] Replicate(Material source, int count)
        {
            Material[] arr = new Material[count];
            for (int i = 0; i < count; i++) arr[i] = source;
            return arr;
        }

        #endregion
    }
}
