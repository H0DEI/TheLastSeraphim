using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargaEscena : MonoBehaviour
{
    public Transform posicionJugador;

    public GameObject puertas;

    [Header("Opcional: Activar o Desactivar Back en el arma")]
    public bool activateBack = false;

    public GameObject enemigos;

    public DialogueGraph dialogueGraph;

    public bool luzJugador = true;

    public Personaje personajeJugador;

    private GameManager instancia;

    private AnimationManager animator;

    [Header("Alineación de cámara opcional")]
    public bool alinearCamara = false;
    public Transform CamaraEscena;

    private void Awake()
    {
        instancia = GameManager.instance;

        instancia.jugador = personajeJugador;

        instancia.CargaJugador();

        instancia.objetoJugador.GetComponentInChildren<InteractuarPersonajes>().personaje = instancia.jugador;

        instancia.objetoJugador.GetComponentInChildren<InteractuarPersonajes>().EstableceGameObjectInteractuarPersonajes();

        instancia.cargaInterfazHabilidades.ActualizaInterfazHabilidades();

        instancia.ActualizarBotonesHabilidades();

        animator = instancia.animationManager;

        instancia.cargaEscena = this;

        animator.ClearCharacters();

        animator.RegisterCharacter(instancia.objetoJugador.GetInstanceID().ToString(), instancia.objetoJugador.AddComponent<CharacterAnimator>().GetComponent<CharacterAnimator>());

        //instancia.Mapa.transform.Find("Sala" + SceneManager.GetActiveScene().name.Substring(3)).Find("Fog").transform.gameObject.SetActive(false);
        
        //instancia.mapPlayerPosition.GetComponent<RectTransform>().position = instancia.Mapa.transform.Find("Sala" + SceneManager.GetActiveScene().name.Substring(3)).GetComponent<RectTransform>().position;

        instancia.tempJugador = Instantiate(instancia.jugador);

        instancia.listaPuertas.Clear();

        instancia.camara = (Camera) FindObjectOfType<Camera>();

        instancia.objetoJugador.transform.position = posicionJugador.position;

        if (alinearCamara && CamaraEscena != null)
        {
            // Intenta encontrar una cámara virtual Cinemachine en cualquier hijo
            var camaraCinemachine = instancia.objetoJugador.GetComponentInChildren<CinemachineCamera>(true);

            if (camaraCinemachine != null)
            {
                Transform camaraTransform = camaraCinemachine.transform;
                camaraTransform.position = CamaraEscena.position;
                camaraTransform.rotation = CamaraEscena.rotation;
            }
            else
            {
                Debug.LogWarning("No se encontró ningún componente CinemachineVirtualCamera dentro de objetoJugador.");
            }
        }

        instancia.objetoJugador.GetComponent<LookAtWithMargin>().GetClosestLookAtTarget(instancia.objetoJugador.transform);

        //instancia.objetoJugador.GetComponent<SpriteRenderer>().flipX = posicionJugador.GetComponent<SpriteRenderer>().flipX;

        Destroy(posicionJugador.gameObject);

        //Corregir esta limpieza, no me gusta +1
        instancia.listaObjetosPersonajesEscena.Clear();

        if (puertas is not null)
        {
            for (int i = 0; i < puertas.transform.childCount; i++)
            {
                instancia.listaPuertas.Add(puertas.transform.GetChild(i).gameObject);
            }
        }

        LinternaSpotLight(instancia.objetoJugador);

        // Activar o desactivar Back si está configurado
        var weapon = instancia.objetoJugador.GetComponentInChildren<WeaponEffects>(true);
        if (weapon != null)
        {
            if (activateBack)
                weapon.ActivateBack2();
            else
                weapon.DeactivateBack2();
        }
        else
        {
            Debug.LogWarning("WeaponEffects no encontrado en el objeto jugador");
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

        CargaDialogo();
    }

    private void Start()
    {
        if (instancia.escenaActual.completada)
        {
            instancia.EscenaCompletada();
        }
    }


    private void CargaDialogo()
    {
        if (dialogueGraph != null)
        {
            instancia.dialogueSystem.InitGraph(dialogueGraph);
        }
    }

    public void LinternaSpotLight(GameObject raiz)
    {
        // Buscar todos los Light en hijos
        Light[] luces = raiz.GetComponentsInChildren<Light>(true); // true incluye los inactivos

        foreach (Light luz in luces)
        {
            if (luz.type == LightType.Spot)
            {
                luz.gameObject.SetActive(luzJugador);
                return; // Sale después de desactivar el primero
            }
        }

        Debug.LogWarning("No se encontró ninguna Spot Light en hijos de " + raiz.name);
    }
}
