using UnityEditor;
using UnityEngine;
using TMPro;

public class TMP_SetDefaultGlyphMetrics : EditorWindow
{
    TMP_SpriteAsset spriteAsset;
    float bx = 0;
    float by = 0;
    float ad = 128;
    float scale = 1f;

    [MenuItem("Tools/TMP/Establecer métricas por defecto")]
    public static void ShowWindow()
    {
        GetWindow<TMP_SetDefaultGlyphMetrics>("Ajustar Sprites TMP");
    }

    void OnGUI()
    {
        spriteAsset = (TMP_SpriteAsset)EditorGUILayout.ObjectField("Sprite Asset", spriteAsset, typeof(TMP_SpriteAsset), false);

        EditorGUILayout.Space();
        bx = EditorGUILayout.FloatField("BX (horizontal offset)", bx);
        by = EditorGUILayout.FloatField("BY (vertical offset)", by);
        ad = EditorGUILayout.FloatField("AD (advance)", ad);
        scale = EditorGUILayout.FloatField("Scale", scale);

        if (spriteAsset != null && GUILayout.Button("Aplicar a todos los sprites"))
        {
            bool algoCambio = false;

            for (int i = 0; i < spriteAsset.spriteCharacterTable.Count; i++)
            {
                var sc = spriteAsset.spriteCharacterTable[i];

                var glyph = sc.glyph;
                var metrics = glyph.metrics;

                metrics.horizontalBearingX = bx;
                metrics.horizontalBearingY = by;
                metrics.horizontalAdvance = ad;

                glyph.metrics = metrics;

                // ✅ Forzar el cambio de escala de forma segura
                if (Mathf.Abs(sc.scale - scale) > 0.001f)
                {
                    spriteAsset.spriteCharacterTable[i] = new TMP_SpriteCharacter(sc.unicode, (TMP_SpriteGlyph)sc.glyph)
                    {
                        name = sc.name,
                        scale = scale
                    };
                    algoCambio = true;
                }
            }

            if (algoCambio)
            {
                spriteAsset.UpdateLookupTables();
                EditorUtility.SetDirty(spriteAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("✅ Escala aplicada correctamente.");
            }
            else
            {
                Debug.Log("ℹ️ Escala ya estaba aplicada o no hubo cambios.");
            }
        }
    }
}
