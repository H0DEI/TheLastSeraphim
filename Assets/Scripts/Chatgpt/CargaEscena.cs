using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargaEscena : MonoBehaviour
{
    public Transform posicionJugador;

    public GameObject puertas;

    public GameObject enemigos;

    public DialogueGraph dialogueGraph;

    private GameManager instancia;

    private AnimationManager animator;

    private void Awake()
    {
        instancia = GameManager.instance;

        animator = instancia.animationManager;

        instancia.cargaEscena = this;

        animator.ClearCharacters();

        animator.RegisterCharacter(instancia.objetoJugador.GetInstanceID().ToString(), instancia.objetoJugador.AddComponent<CharacterAnimator>().GetComponent<CharacterAnimator>());

        instancia.Mapa.transform.Find("Sala" + SceneManager.GetActiveScene().name.Substring(3)).Find("Fog").transform.gameObject.SetActive(false);
        
        instancia.mapPlayerPosition.GetComponent<RectTransform>().position = instancia.Mapa.transform.Find("Sala" + SceneManager.GetActiveScene().name.Substring(3)).GetComponent<RectTransform>().position;

        instancia.tempJugador = Instantiate(instancia.jugador);

        instancia.listaPuertas.Clear();

        instancia.camara = (Camera) FindObjectOfType<Camera>();

        instancia.objetoJugador.transform.position = posicionJugador.position;

        instancia.objetoJugador.GetComponent<LookAtWithMargin>().GetClosestLookAtTarget(instancia.objetoJugador.transform);

        //instancia.objetoJugador.GetComponent<SpriteRenderer>().flipX = posicionJugador.GetComponent<SpriteRenderer>().flipX;

        Destroy(posicionJugador.gameObject);

        //Corregir esta limpieza, no me gusta +1
        instancia.listaObjetosPersonajesEscena.Clear();

        for (int i = 0; i < puertas.transform.childCount; i++)
        {
            instancia.listaPuertas.Add(puertas.transform.GetChild(i).gameObject);
        }

        if (!instancia.escenaActual.completada) 
        { 
            for (int i=0; i<enemigos.transform.childCount; i++)
            {
                instancia.listaObjetosPersonajesEscena.Add(enemigos.transform.GetChild(i).gameObject);

                animator.RegisterCharacter(enemigos.transform.GetChild(i).gameObject.GetInstanceID().ToString(), enemigos.transform.GetChild(i).gameObject.AddComponent<CharacterAnimator>().GetComponent<CharacterAnimator>());
            }

            instancia.informacionInterfaz.ActualizaPuntos();

            instancia.CargaPersonajesEscena();

            instancia.CargaTurno();

            instancia.ActivaBotonesInterfaz();

            instancia.ResetearObjetivosSeleccionables();

            instancia.XP.ExperienciaEscena();
        }
        else
        {
            instancia.EscenaCompletada();
        }

        CargaDialogo();
    }

    private void CargaDialogo()
    {
        if (dialogueGraph != null)
        {
            instancia.dialogueSystem.InitGraph(dialogueGraph);
        }
    }
}
