using UnityEngine;

public class AñadirMaterialExtra : MonoBehaviour
{
    [Tooltip("Material que se superpondrá a todos los submeshes.")]
    public Material overlayMaterial;

    private GameObject overlayObject;

    void Start()
    {
        if (overlayMaterial == null)
        {
            Debug.LogWarning("No se asignó un material de overlay.");
            return;
        }

        Renderer originalRenderer = GetComponent<Renderer>();
        if (originalRenderer == null)
        {
            Debug.LogWarning("Este objeto no tiene un Renderer compatible.");
            return;
        }

        // Duplicar objeto visual sin lógica
        overlayObject = new GameObject("Overlay_" + gameObject.name);
        overlayObject.transform.SetParent(transform);
        overlayObject.transform.localPosition = Vector3.zero;
        overlayObject.transform.localRotation = Quaternion.identity;
        overlayObject.transform.localScale = Vector3.one;

        Material[] overlayMaterials;

        if (originalRenderer is SkinnedMeshRenderer skinned)
        {
            SkinnedMeshRenderer newRenderer = overlayObject.AddComponent<SkinnedMeshRenderer>();
            newRenderer.sharedMesh = skinned.sharedMesh;
            newRenderer.bones = skinned.bones;
            newRenderer.rootBone = skinned.rootBone;

            int submeshCount = skinned.sharedMesh.subMeshCount;
            overlayMaterials = new Material[submeshCount];
            for (int i = 0; i < submeshCount; i++)
                overlayMaterials[i] = overlayMaterial;

            newRenderer.materials = overlayMaterials;
        }
        else if (originalRenderer is MeshRenderer meshRenderer && TryGetComponent<MeshFilter>(out MeshFilter filter))
        {
            overlayObject.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
            MeshRenderer newRenderer = overlayObject.AddComponent<MeshRenderer>();

            int submeshCount = filter.sharedMesh.subMeshCount;
            overlayMaterials = new Material[submeshCount];
            for (int i = 0; i < submeshCount; i++)
                overlayMaterials[i] = overlayMaterial;

            newRenderer.materials = overlayMaterials;
        }
    }
}
