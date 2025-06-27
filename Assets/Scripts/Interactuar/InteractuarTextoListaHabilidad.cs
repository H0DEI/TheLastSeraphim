using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InteractuarTextoListaHabilidad : MonoBehaviour, IBoton
{
    public Habilidad habilidad;

    public bool soyJugador;

    private bool puedePresionarse;

    private TextMeshProUGUI texto;

    private GameManager instancia;

    private Color colorDefault;

    private void Start()
    {
        puedePresionarse = true;

        instancia = GameManager.instance;

        RecogeComponenteTexto();

        if (soyJugador) colorDefault = texto.color;
        else colorDefault = Color.red;

        texto.color = colorDefault;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto.fontStyle = FontStyles.Bold;

        texto.color = Color.gray;

        instancia.informacionDescripciones.MuestraInformacionHabilidad(habilidad);

        for (int i = 0; i < instancia.listaObjetosPersonajesEscena.Count; i++)
        {
            GameObject personaje = instancia.listaObjetosPersonajesEscena[i];

            foreach(Personaje objetivo in habilidad.objetivos)
            {
                if (personaje.GetComponent<InteractuarPersonajes>().personaje.GetInstanceID() == objetivo.GetInstanceID())
                {
                    instancia.CambiaColorOutline(objetivo, soyJugador, personaje);

                    personaje.GetComponent<Outline>().SetOutlineVisible(true);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texto.fontStyle = FontStyles.Normal;

        texto.color = colorDefault;

        //instancia.informacionDescripciones.LimpiaInformacion();

        instancia.ResetearObjetivosSeleccionables();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (puedePresionarse && soyJugador) { 
            instancia.habilidadesALanzar.RemueveIndice(transform.GetSiblingIndex());

            instancia.ResetearObjetivosSeleccionables();
        }
    }

    public void Desactivar()
    {
        texto.color = Color.gray;

        puedePresionarse = false;
    }

    public void Activar()
    {
        if(texto == null)
        {
            RecogeComponenteTexto();
        }

        texto.color = colorDefault;

        puedePresionarse = true;
    }

    private void RecogeComponenteTexto()
    {
        texto = GetComponent<TextMeshProUGUI>();
    }
}
