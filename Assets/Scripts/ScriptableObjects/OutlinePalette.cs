using UnityEngine;

[CreateAssetMenu(fileName = "New OutlinePalette", menuName = "Outline/Palette", order = 1)]
public class OutlinePalette : ScriptableObject
{
    [Tooltip("Material para cada valor del Outline.color (índices: 0, 1, 2)")]
    public Material[] materials = new Material[3];

    public Material Get(int index)
    {
        if (materials == null || materials.Length == 0)
        {
            Debug.LogWarning("OutlinePalette no tiene materiales asignados");
            return null;
        }

        return materials[Mathf.Clamp(index, 0, materials.Length - 1)];
    }
}
