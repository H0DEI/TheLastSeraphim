using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LookAtWithMargin))]
public class InteractuarPersonajes : MonoBehaviour
{
    public bool aliado = false;
    public bool elegible;
    public bool vivo;
    public bool puedePresionarse;
    public bool cursorDesactivado;

    public Personaje personaje;

    private GameManager instancia;

    private void Start()
    {
        instancia = GameManager.instance;
        vivo = true;

        personaje.gameObject = transform.gameObject;

        puedePresionarse = true;
    }

    private void OnMouseEnter()
    {
        instancia.informacionDescripciones.MuestraInformacionPersonaje(personaje);
    }

    private void OnMouseDown()
    {
        if (elegible && vivo && puedePresionarse)
        {
            instancia.ResetearObjetivosSeleccionables();

            instancia.interactuarBotonHabilidad.ObjetivoSeleccionado(personaje);
        }
    }

    public void Desactivar()
    {
        puedePresionarse = false;
    }

    public void Activar()
    {
        puedePresionarse = true;
    }
}
