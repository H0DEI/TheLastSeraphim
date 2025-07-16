using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimacionDataLookup", menuName = "Config/AnimacionDataLookup")]
public class AnimacionDataLookup : ScriptableObject
{
    [System.Serializable]
    public class EntradaAnimacion
    {
        public string nombreEstado;
        public AnimationClip clip;
    }

    public List<EntradaAnimacion> animaciones = new List<EntradaAnimacion>();

    private Dictionary<string, AnimationClip> lookupTable;

    public void Initialize()
    {
        lookupTable = new Dictionary<string, AnimationClip>();
        foreach (var entrada in animaciones)
        {
            if (!lookupTable.ContainsKey(entrada.nombreEstado))
            {
                lookupTable.Add(entrada.nombreEstado, entrada.clip);
            }
        }
    }

    public float GetDuracion(string nombreEstado)
    {
        if (lookupTable == null)
            Initialize();

        if (lookupTable.TryGetValue(nombreEstado, out var clip))
        {
            return clip.length;
        }
        else
        {
            Debug.LogWarning($"Animación '{nombreEstado}' no encontrada en el Lookup");
            return 0f;
        }
    }
}
