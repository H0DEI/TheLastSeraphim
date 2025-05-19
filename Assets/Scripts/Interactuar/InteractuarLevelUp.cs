using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InteractuarLevelUp : MonoBehaviour, IBoton
{
    public TextMeshProUGUI upgrade;

    public bool esMejora;

    public Habilidad habilidad;

    public GameObject InterfazJugable;
    public GameObject LevelUp;

    private TextMeshProUGUI texto;

    private GameManager instancia;

    private int indice;

    private string habiNormal;

    private void Start()
    {
        instancia = GameManager.instance;

        texto = GetComponentInChildren<TextMeshProUGUI>();

        if (esMejora) {
            upgrade.SetText("Mejorar");
        } else { 
            upgrade.SetText("Desbloquear"); 
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        instancia.informacionDescripciones.bloqueoDescripcion = false;

        instancia.DesactivaBotonesInterfaz();
        instancia.DesactivaPersonajes();

        InterfazJugable.SetActive(true);

        if (!esMejora)
        {
            instancia.ActivaBotonesInterfaz(0);
       
            instancia.habilidadLevelUp = habilidad;

            instancia.puedeCambiarseHabilidad = true;

            LevelUp.SetActive(false);
        }
        else
        {
            indice = habilidad.nombre.LastIndexOf("+") + 1;

            int coletilla = int.Parse(habilidad.nombre.Substring(indice)) - 1;

            if (coletilla == 0) habiNormal = habilidad.nombre.Substring(0, indice - 2);
            else habiNormal = habilidad.nombre.Substring(0, indice) + coletilla;

            for (int i = 0; i < instancia.jugador.habilidades.Count; i++)
            {
                if (instancia.jugador.habilidades[i].nombre == habiNormal)
                {
                    instancia.jugador.habilidades[i] = habilidad;
                }                
            }

            instancia.ActualizarBotonesHabilidades();

            instancia.AbrirCerrarPuertas(true);

            LevelUp.SetActive(false);

            instancia.XP.ComprovarNivel();
        }

        instancia.objetoJugador.GetComponent<InteractuarPersonajes>().cursorDesactivado = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto.fontStyle = FontStyles.Bold;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texto.fontStyle = FontStyles.Normal;
    }

    public void Activar()
    {
        throw new System.NotImplementedException();
    }

    public void Desactivar()
    {
        throw new System.NotImplementedException();
    }
}
