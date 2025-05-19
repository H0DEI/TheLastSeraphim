using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformacionDescripciones : MonoBehaviour
{
    public TextMeshProUGUI muestraNombre;
    public TextMeshProUGUI muestraDescripcion;
    public TextMeshProUGUI SPD;
    public TextMeshProUGUI EN;
    public TextMeshProUGUI PV;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI HE;
    public TextMeshProUGUI F;
    public TextMeshProUGUI R;
    public TextMeshProUGUI INV;

    public bool bloqueoDescripcion;

    [Header("UI")]
    public TMP_SpriteAsset iconosHUD;
    public TextoEnriquecido datosVisuales;

    [Header("Colores")]
    public string colorDaño = "#FF5555"; // Rojo
    public string colorCuracion = "#55FF55"; // Verde
    public string colorDefensa = "#55FFFF"; // Azul

    private string fuerza;
    private string penetracion;

    public void MuestraInformacionHabilidad(Habilidad habilidad)
    {
        if (habilidad.descripcion == "")
        {
            if (habilidad.melee) habilidad.descripcion = "Melee";
            else habilidad.descripcion = "Range";

            habilidad.descripcion += " A " + habilidad.acciones.Count;

            if (habilidad.melee) habilidad.descripcion += " F +" + habilidad.fuerza;
            else habilidad.descripcion += " F " + habilidad.fuerza;

            if (habilidad.penetracion == 0) habilidad.descripcion += " FP 0";
            else habilidad.descripcion += " FP -" + habilidad.penetracion;

            habilidad.descripcion += " D " + habilidad.daño;
        } 
        else
        {
            if (habilidad.penetracion == 0) penetracion = habilidad.penetracion.ToString();
            else penetracion = " -" + habilidad.penetracion.ToString();

            if (!(habilidad.descripcion.IndexOf(" ") < 0))
            {
                if (habilidad.descripcion.Substring(0, habilidad.descripcion.IndexOf(" ")) == "Range") fuerza = habilidad.fuerza.ToString();
                else fuerza = " +" + habilidad.fuerza.ToString();
            }
        }        
        
        muestraNombre.text = habilidad.nombre;
        //muestraDescripcion.text = string.Format(habilidad.descripcion, fuerza, penetracion, " " + habilidad.daño);
        Mostrar(habilidad);
        //muestraValorVelocidad.text = habilidad.velocidad.ToString();
        //if (muestraTextoTier != null) muestraTextoTier.text = habilidad.tier.ToString();
        EN.text = habilidad.coste.ToString();
        SPD.text = habilidad.velocidad.ToString();
    }

    public void Mostrar(Habilidad habilidad)
    {
        muestraDescripcion.text = datosVisuales.Aplicar(habilidad.descripcion);
    }

    public void MuestraInformacionPersonaje(Personaje personaje)
    {
        muestraDescripcion.text = "";

        muestraNombre.text = personaje.nombre;

        foreach (Habilidad hab in personaje.habilidades)
        {
            muestraDescripcion.text += hab.nombre + "\n";
        }

        //if (muestraTextoTier != null) muestraTextoTier.text = "lvl:"+personaje.nivel.ToString();
        SPD.text = "-";
        EN.text = personaje.accionesMaximas.ToString();
        PV.text = personaje.heridasActuales.ToString();
        HP.text = personaje.punteria.ToString();
        HE.text = personaje.habilidadEspecial.ToString();
        F.text = personaje.fuerza.ToString();
        R.text = personaje.resistencia.ToString();
        INV.text = personaje.salvacionInvulnerable.ToString();
    }

    //public void LimpiaInformacion()
    //{
    //    muestraNombre.text = "";
    //    muestraDescripcion.text = "";
    //    muestraTextoVelocidad.text = "";
    //    muestraValorVelocidad.text = "";
    //    if (muestraTextoTier != null) muestraTextoTier.text = "";
    //    muestraTextoCosteAccion.text = "";
    //    muestraValorCosteAccion.text = "";
    //    if (muestraAtributos != null) muestraAtributos.text = "";
    //}
}
