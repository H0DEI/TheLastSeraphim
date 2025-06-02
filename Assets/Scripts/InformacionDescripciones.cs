using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class InformacionDescripciones : MonoBehaviour
{
    public TextMeshProUGUI muestraNombre;
    public TextMeshProUGUI muestraDescripcion;
    public GameObject flechaMejora;
    public GameObject objetomuestraDescripcion2;
    public TextMeshProUGUI muestraDescripcion2;
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

        if (flechaMejora && objetomuestraDescripcion2 is not null)
        {
            flechaMejora.SetActive(false);
            objetomuestraDescripcion2.SetActive(false);
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

    public void MostrarMejora(string descripcionMejora)
    {
        muestraDescripcion2.text = datosVisuales.Aplicar(descripcionMejora);
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

    public string CompararDescripcionesAvanzado(string descripcionAntigua, string descripcionNueva)
    {
        Dictionary<string, int> antiguos = ExtraerAtributos(descripcionAntigua);
        Dictionary<string, int> nuevos = ExtraerAtributos(descripcionNueva);

        string resultado = Regex.Replace(descripcionNueva, @"\{(\w+)\}\s*(\d+)", match =>
        {
            string clave = match.Groups[1].Value;
            int valorNuevo = int.Parse(match.Groups[2].Value);

            // Deja el sprite siempre fuera del color
            string sprite = $"<sprite name=\"{clave}\"> ";

            if (!antiguos.TryGetValue(clave, out int valorViejo))
            {
                // Atributo nuevo: verde
                return $"{sprite}<color=#20BE00>{valorNuevo}</color>";
            }

            if (valorNuevo > valorViejo)
            {
                return $"{sprite}<color=#20BE00>{valorNuevo}</color>";
            }
            else if (valorNuevo < valorViejo)
            {
                return $"{sprite}<color=#BE0000>{valorNuevo}</color>";
            }
            else
            {
                // Sin cambio, resaltar en blanco
                return $"{sprite}<color=#FFFFFF>{valorNuevo}</color>";
            }
        });



        // Extra: asegurar que estamos comparando sin espacios extraños
        string textoPlanoAnterior = QuitarAtributos(descripcionAntigua).Trim();
        string textoPlanoNuevo = QuitarAtributos(descripcionNueva).Trim();

        resultado = ResaltarTextoNuevo(textoPlanoAnterior, resultado);

        return resultado;
    }

    private Dictionary<string, int> ExtraerAtributos(string descripcion)
    {
        var atributos = new Dictionary<string, int>();
        var matches = Regex.Matches(descripcion, @"\{(\w+)\}\s*(\d+)");
        foreach (Match match in matches)
        {
            string clave = match.Groups[1].Value;
            int valor = int.Parse(match.Groups[2].Value);
            atributos[clave] = valor;
        }
        return atributos;
    }

    private string QuitarAtributos(string texto)
    {
        return Regex.Replace(texto, @"\{(\w+)\}\s*\d+", "").Trim();
    }

    private string ResaltarTextoNuevo(string original, string nuevo)
    {
        int i = 0;
        while (i < original.Length && i < nuevo.Length && original[i] == nuevo[i])
            i++;

        if (i >= nuevo.Length)
            return nuevo;

        string inicio = nuevo.Substring(0, i);
        string añadido = nuevo.Substring(i);

        return $"{inicio}<color=#20BE00>{añadido}</color>";
    }

    public void MuestraHabilidadMejorada(Habilidad habilidadAnterior, Habilidad habilidad, string descripcionMejora)
    {
        flechaMejora.SetActive(true);
        objetomuestraDescripcion2.SetActive(true);
        muestraNombre.text = habilidad.nombre;
        Mostrar(habilidadAnterior);
        MostrarMejora(descripcionMejora);
        EN.text = habilidad.coste.ToString();
        SPD.text = habilidad.velocidad.ToString();
    }
}
