using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Habilidad))]
public class EnumScriptEditor : Editor
{
    private SerializedProperty propiedadNombre;
    private SerializedProperty propiedadDescripcion;
    private SerializedProperty propiedadCoste;
    private SerializedProperty propiedadVelocidad;
    private SerializedProperty propiedadFuerza;
    private SerializedProperty propiedadPenetracion;
    private SerializedProperty propiedadDaño;
    private SerializedProperty propiedadAcciones;
    private SerializedProperty propiedadAnimaciones;
    private SerializedProperty propiedadObjetivos;
    private SerializedProperty propiedadTipoSeleccion;
    private SerializedProperty propiedadCantidad;
    private SerializedProperty propiedadUsosLimitados;
    private SerializedProperty propiedadMelee;
    private SerializedProperty propiedadNumeroDeUsos;
    private SerializedProperty propiedadTier;
    private SerializedProperty propiedadSonido;
    private SerializedProperty propiedadInvocacion;

    private void OnEnable()
    {
        propiedadNombre = serializedObject.FindProperty("nombre");
        propiedadDescripcion = serializedObject.FindProperty("descripcion");
        propiedadCoste = serializedObject.FindProperty("coste");
        propiedadVelocidad = serializedObject.FindProperty("velocidad");
        propiedadFuerza = serializedObject.FindProperty("fuerza");
        propiedadPenetracion = serializedObject.FindProperty("penetracion");
        propiedadDaño = serializedObject.FindProperty("daño");
        propiedadAcciones = serializedObject.FindProperty("acciones");
        propiedadAnimaciones = serializedObject.FindProperty("animaciones");
        propiedadObjetivos = serializedObject.FindProperty("objetivos");
        propiedadTipoSeleccion = serializedObject.FindProperty("tipoSeleccion");
        propiedadCantidad = serializedObject.FindProperty("cantidad");
        propiedadUsosLimitados = serializedObject.FindProperty("usosLimitados");
        propiedadMelee = serializedObject.FindProperty("melee");
        propiedadNumeroDeUsos = serializedObject.FindProperty("numeroDeUsos");
        propiedadTier = serializedObject.FindProperty("tier");
        propiedadSonido = serializedObject.FindProperty("sonido");
        propiedadInvocacion = serializedObject.FindProperty("invocacion");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(propiedadNombre);
        EditorGUILayout.PropertyField(propiedadDescripcion);
        EditorGUILayout.PropertyField(propiedadCoste);
        EditorGUILayout.PropertyField(propiedadVelocidad);
        EditorGUILayout.PropertyField(propiedadFuerza);
        EditorGUILayout.PropertyField(propiedadPenetracion);
        EditorGUILayout.PropertyField(propiedadDaño);
        EditorGUILayout.PropertyField(propiedadAcciones);
        EditorGUILayout.PropertyField(propiedadAnimaciones);
        EditorGUILayout.PropertyField(propiedadObjetivos);
        EditorGUILayout.PropertyField(propiedadTipoSeleccion);
        EditorGUILayout.PropertyField(propiedadMelee);

        TipoSeleccion tipo = (TipoSeleccion)propiedadTipoSeleccion.enumValueIndex;

        EditorGUILayout.PropertyField(propiedadSonido);

        switch (tipo)
        {
            case TipoSeleccion.SoloJugador:
            case TipoSeleccion.SoloUnEnemigo:
            case TipoSeleccion.CualquierPersonaje:

                propiedadCantidad.intValue = 1;

                break;

            case TipoSeleccion.VariosEnemigos:

                propiedadCantidad.intValue = 2;

                EditorGUILayout.PropertyField(propiedadCantidad);

                break;
        }

        EditorGUILayout.PropertyField(propiedadUsosLimitados);

        if (propiedadUsosLimitados.boolValue == true)
        {
            propiedadNumeroDeUsos.intValue = 1;

            EditorGUILayout.PropertyField(propiedadNumeroDeUsos);
        }
        else
        {
            propiedadNumeroDeUsos.intValue = 727;
        }

        EditorGUILayout.PropertyField(propiedadTier);

        EditorGUILayout.PropertyField(propiedadInvocacion);

        serializedObject.ApplyModifiedProperties();
    }
}
