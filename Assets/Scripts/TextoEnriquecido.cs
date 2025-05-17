using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextoEnriquecido
{
    [System.Serializable]
    public class Reemplazo
    {
        public string clave;           // {dano}
        public string textoPlano;      // 10
        public Color color = Color.white;
        public string spriteName;      // blind (opcional)
    }

    public List<Reemplazo> reemplazos = new List<Reemplazo>();

    public string Aplicar(string textoBase)
    {
        string resultado = textoBase;

        foreach (var r in reemplazos)
        {
            string reemplazoFinal = "";

            if (!string.IsNullOrEmpty(r.spriteName))
            {
                reemplazoFinal = $"<sprite name=\"{r.spriteName}\">";
            }
            else
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(r.color);
                reemplazoFinal = $"<color=#{colorHex}>{r.textoPlano}</color>";
            }

            resultado = resultado.Replace($"{{{r.clave}}}", reemplazoFinal);
        }

        return resultado;
    }
}