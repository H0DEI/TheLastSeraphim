using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using cakeslice;

public class InteractuarBotonHabilidad : MonoBehaviour, IBoton
{
    public int cantidad;
    public int numUsos;

    public Habilidad habilidad;

    private bool puedePresionarse;

    private TextMeshProUGUI texto;

    private Color colorDefault;

    private Personaje jugador;

    private GameManager instancia;

    private void Start()
    {
        InteractuarBotonHabilidad a = this;

        puedePresionarse = true;

        instancia = GameManager.instance;

        jugador = instancia.jugador;

        texto = GetComponent<TextMeshProUGUI>();

        colorDefault = texto.color;

        ResetearHabilidad();
    }

    public void ResetearHabilidad()
    {
        habilidad = instancia.BuscaHabilidadInstanciada(this.texto.text);

        numUsos = habilidad.numeroDeUsos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto.fontStyle = FontStyles.Bold;

        habilidad.personaje = jugador;

        instancia.informacionDescripciones.MuestraInformacionHabilidad(habilidad);

        if (!instancia.habilidadSeleccionada) instancia.MuestraObjetivosSeleccionables(habilidad, true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        texto.fontStyle = FontStyles.Normal;

        //instancia.informacionDescripciones.LimpiaInformacion();

        if(!instancia.habilidadSeleccionada) instancia.ResetearObjetivosSeleccionables();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (instancia.puedeCambiarseHabilidad)
        {
            for (int i = 0; i < jugador.habilidades.Count; i++)
            {
                if (jugador.habilidades[i].nombre == habilidad.nombre)
                {
                    jugador.habilidades[i] = instancia.habilidadLevelUp;
                }
            }

            instancia.ActualizarBotonesHabilidades();

            instancia.puedeCambiarseHabilidad = false;

            instancia.AbrirCerrarPuertas(true);

            instancia.XP.ComprovarNivel();
        }
        else if (numUsos > 0)
        {
            if (numUsos != 727) numUsos--;

            PulsaHabilidad();
        }
    }

    public void PulsaHabilidad()
    {
        if (puedePresionarse && habilidad.coste <= jugador.accionesActuales)
        {
            habilidad.personaje = jugador;

            instancia.habilidadSeleccionada = true;

            ObjetivosSeleccionables();
        }
    }

    public void ObjetivoSeleccionado(Personaje personaje)
    {
        cantidad--;
        
        habilidad.objetivos.Add(personaje);

        if (cantidad == 0) FinalizaSeleccion();
    }

    public void Desactivar()
    {
        texto.color = Color.gray;

        puedePresionarse = false;

        Image img = GetComponentInParent<Image>();
        if (img != null)
        {
            Color c = img.color;
        c.a = Mathf.Clamp01(0.5f); // asegura que esté entre 0 y 1
        img.color = c;
    }
    }

    public void Activar()
    {
        texto.color = colorDefault;

        puedePresionarse = true;

        Image img = GetComponentInParent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = Mathf.Clamp01(1f); // asegura que esté entre 0 y 1
            img.color = c;
        }
    }

    private void ObjetivosSeleccionables()
    {
        instancia.interactuarBotonHabilidad = this;

        instancia.DesactivaBotonesInterfaz();

        habilidad.objetivos.Clear();

        switch (habilidad.tipoSeleccion)
        {
            case TipoSeleccion.SoloJugador:
            case TipoSeleccion.SoloUnEnemigo:
            case TipoSeleccion.CualquierPersonaje:

                cantidad = habilidad.cantidad;

                instancia.MuestraObjetivosSeleccionables(habilidad, true);

                break;

            case TipoSeleccion.VariosEnemigos:

                instancia.mostrarIndicador = true;

                cantidad = habilidad.cantidad;

                instancia.MuestraObjetivosSeleccionables(habilidad, true);

                break;

            case TipoSeleccion.TodosLosEnemigos:

                instancia.SeleccionaTodosLosPersonajes(habilidad, true);

                FinalizaSeleccion();

                break;
            
            case TipoSeleccion.SinSeleccionar:

                instancia.SeleccionaTodosLosPersonajes(habilidad, false);

                FinalizaSeleccion();

                break;
        }
    }

    private void FinalizaSeleccion()
    {
        Habilidad habilidadInstanciada;

        instancia.habilidadSeleccionada = false;

        instancia.mostrarIndicador = false;

        jugador.accionesActuales -= habilidad.coste;

        habilidad.personaje = jugador;

        habilidadInstanciada = Instantiate(habilidad);

        habilidadInstanciada.velocidad += habilidad.personaje.agilidad;
        
        instancia.habilidadesALanzar.listaHabilidadesALanzar.Add(new KeyValuePair<Habilidad, bool>(habilidadInstanciada, true));
        
        instancia.ActivaBotonesInterfaz();

        instancia.ActualizarListaHabilidades();

        instancia.ResetearObjetivosSeleccionables();

        GameManager.instance.informacionInterfaz.ActualizaPuntos();
    }
}
