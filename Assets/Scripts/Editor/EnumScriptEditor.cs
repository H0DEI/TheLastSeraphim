using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Habilidad))]
public class EnumScriptEditor : Editor
{
    /* ─── propiedades existentes ─── */
    SerializedProperty propNombre, propDescripcion, propCoste, propVelocidad,
                       propFuerza, propPenetracion, propTipoDaño, propDaño, propAcciones,
                       propAnimaciones, propObjetivos, propTipoSel, propCantidad,
                       propUsosLimitados, propMelee, propNumeroUsos, propTier,
                       propSonido, propInvocacion;

    /* ─── NUEVAS ─── */
    SerializedProperty propPermiteCrit, propProbCritExtra, propDañoCritExtra;

    void OnEnable()
    {
        /* existentes */
        propNombre = serializedObject.FindProperty("nombre");
        propDescripcion = serializedObject.FindProperty("descripcion");
        propCoste = serializedObject.FindProperty("coste");
        propVelocidad = serializedObject.FindProperty("velocidad");
        propFuerza = serializedObject.FindProperty("fuerza");
        propPenetracion = serializedObject.FindProperty("penetracion");
        propTipoDaño = serializedObject.FindProperty("tipoDaño");
        propDaño = serializedObject.FindProperty("daño");
        propAcciones = serializedObject.FindProperty("acciones");
        propAnimaciones = serializedObject.FindProperty("animaciones");
        propObjetivos = serializedObject.FindProperty("objetivos");
        propTipoSel = serializedObject.FindProperty("tipoSeleccion");
        propCantidad = serializedObject.FindProperty("cantidad");
        propUsosLimitados = serializedObject.FindProperty("usosLimitados");
        propMelee = serializedObject.FindProperty("melee");
        propNumeroUsos = serializedObject.FindProperty("numeroDeUsos");
        propTier = serializedObject.FindProperty("tier");
        propSonido = serializedObject.FindProperty("sonido");
        propInvocacion = serializedObject.FindProperty("invocacion");

        /* nuevas */
        propPermiteCrit = serializedObject.FindProperty("permiteCritico");
        propProbCritExtra = serializedObject.FindProperty("probCritExtra");
        propDañoCritExtra = serializedObject.FindProperty("dañoCritExtra");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        /* ─── resto del Inspector tal cual ─── */
        EditorGUILayout.PropertyField(propNombre);
        EditorGUILayout.PropertyField(propDescripcion);
        EditorGUILayout.PropertyField(propCoste);
        EditorGUILayout.PropertyField(propVelocidad);
        EditorGUILayout.PropertyField(propFuerza);
        EditorGUILayout.PropertyField(propPenetracion);
        EditorGUILayout.PropertyField(propTipoDaño);
        EditorGUILayout.PropertyField(propDaño);
        EditorGUILayout.PropertyField(propAcciones);
        EditorGUILayout.PropertyField(propAnimaciones);
        EditorGUILayout.PropertyField(propObjetivos);
        EditorGUILayout.PropertyField(propTipoSel);
        EditorGUILayout.PropertyField(propMelee);

        /* … (tu lógica para cantidad, usos, tier, sonido, etc.) … */

        /* ─── BLOQUE CRÍTICO ─── */
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Crítico", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(propPermiteCrit, new GUIContent("Permite crítico"));
        using (new EditorGUI.DisabledScope(!propPermiteCrit.boolValue))
        {
            EditorGUILayout.PropertyField(propProbCritExtra, new GUIContent("Prob. Crit Extra (%)"));
            EditorGUILayout.PropertyField(propDañoCritExtra, new GUIContent("Daño Crit Extra (%)"));
        }

        /* resto de campos */
        EditorGUILayout.PropertyField(propInvocacion);

        serializedObject.ApplyModifiedProperties();
    }
}
