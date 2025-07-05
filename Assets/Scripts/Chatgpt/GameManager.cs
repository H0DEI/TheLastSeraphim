using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.Rendering;
using System.Threading;
using System;

public class GameManager : MonoBehaviour
{
    public SortedList<string, Escena> copiasEscenas = new SortedList<string, Escena>();

    public Escena escenaActual;

    public static GameManager instance;

    // === Evento global para impactos de habilidad ===
    public static event System.Action OnImpactoHabilidad;

    /// <summary>Llama a este método desde WeaponEffects
    /// cuando llegue el fotograma de impacto.</summary>
    public static void EmitirImpactoHabilidad()
    {
        OnImpactoHabilidad?.Invoke();
    }

    public TotalDamageDisplay totalDamageDisplay;  // Asignar desde Inspector

    public AnimationManager animationManager;

    public FloatingTextManager floatingTextManager;

    public OldFloatingTextManager textManager;

    public CargaEscena cargaEscena;

    public InformacionDescripciones informacionDescripciones;

    public InformacionInterfaz informacionInterfaz;

    public HabilidadesALanzar habilidadesALanzar;

    public XP XP;

    public CargaInterfazHabilidades cargaInterfazHabilidades;

    public InteractuarBotonHabilidad interactuarBotonHabilidad;

    public List<GameObject> listaBotonesInterfaz = new List<GameObject>();

    public List<Personaje> listaScriptPersonajesEscena = new List<Personaje>();

    public List<GameObject> listaObjetosPersonajesEscena = new List<GameObject>();

    public List<GameObject> listaPuertas = new List<GameObject>();

    public Personaje jugador;

    public Personaje tempJugador;

    public GameObject objetoJugador;

    public GameObject Mapa;

    public GameObject mapPlayerPosition;

    public BarraDeVida barraDeVida;

    public bool habilidadSeleccionada;
    public bool mostrarIndicador;

    public Dictionary<TipoSeleccion, bool[]> tipoSelecciones;

    public GameObject btnHasMuerto;

    public GameObject panelAyuda;

    public Habilidad habilidadLevelUp;

    public bool puedeCambiarseHabilidad;

    //Escena 1 carga fea, corregir
    public Escena escena1;

    public Camera camara;

    public DialogueSystem dialogueSystem;

    public GameObject characters;

    public GameObject dialogue;

    public GameObject name;

    public GameObject Icon;

    public GameObject background;

    public AudioSource soundEffect;

    public GameObject Linterna;

    // GameManager.cs  (zona eventos estáticos)

    public void AnimarBarraVida(Personaje p, float porcentajeNuevo, float delay)
    {
        if (p.gameObject.GetComponentInChildren<BarraDeVida>())      // asumiendo que guardas la referencia
            p.gameObject.GetComponentInChildren<BarraDeVida>().AnimarHasta(porcentajeNuevo, delay);
    }


    public static event System.Action<int> OnVentanaImpacto;     // frames

    public static void EmitirVentanaImpacto(int frames) =>
            OnVentanaImpacto?.Invoke(frames);


    [System.Serializable]
    public struct TotalDisplayPorTipo
    {
        public FloatingTextTipo tipo;        // Total, IgneoTotal, etc.
        public TotalDamageDisplay display;   // asigna el prefab
    }

    [Header("Total Damage displays")]
    public TotalDisplayPorTipo[] totalDisplays;

    readonly Dictionary<FloatingTextTipo, TotalDamageDisplay> _mapTotals = new();

    //public GameObject indicadorRaton;

    private Transform interfaz;

    private void Awake()
    {
        instance = this;

        try { 
        DontDestroyOnLoad(instance);
        }
        catch (System.Exception)
        {
            // Silenciado intencionadamente
        }

        interfaz = transform.Find("CanvasInterfaz");

        interfaz.gameObject.SetActive(false);

        foreach (var td in totalDisplays)
            if (td.display) _mapTotals[td.tipo] = td.display;

        CargaJugador();
    }

    private void Start()
    {
        if (tipoSelecciones == null)
        {
            tipoSelecciones = new Dictionary<TipoSeleccion, bool[]>();

            tipoSelecciones.Add(TipoSeleccion.SoloJugador, new bool[] { true, false, false, false, false });
            tipoSelecciones.Add(TipoSeleccion.SoloUnEnemigo, new bool[] { false, true, true, true, true });
            tipoSelecciones.Add(TipoSeleccion.VariosEnemigos, new bool[] { false, true, true, true, true });
            tipoSelecciones.Add(TipoSeleccion.TodosLosEnemigos, new bool[] { false, true, true, true, true });
            tipoSelecciones.Add(TipoSeleccion.CualquierPersonaje, new bool[] { true, true, true, true, true });
            tipoSelecciones.Add(TipoSeleccion.SinSeleccionar, new bool[] { true, true, true, true, true });
        }
    }

    /* llamado por las habilidades */
    public void MostrarTotalDaño(int delta, FloatingTextTipo tipo, float delay)
    {
        if (!_mapTotals.TryGetValue(tipo, out var disp) || disp == null) return;

        StartCoroutine(CoMostrar());

        IEnumerator CoMostrar()
        {
            yield return new WaitForSeconds(delay);
            disp.Añadir(delta);
        }
    }


    public void CompruebaYCargaEscenas(Escena escena)
    {
        if (!copiasEscenas.ContainsKey(escena.idEscena))
        {
            copiasEscenas.Add(escena.idEscena, Instantiate(escena));
        }

        CargaEscenas((escenaActual = copiasEscenas[escena.idEscena]).idEscena);
    }

    private void CargaEscenas(string idEscena)
    {
        SceneManager.LoadScene(idEscena);
    }

    public void ActivaInterfaz()
    {
        interfaz.gameObject.SetActive(true);
    }

    public Habilidad BuscaHabilidadInstanciada(string nombreHabilidad)
    {
        foreach (Habilidad habi in jugador.habilidades)
        {
            if (habi.nombre == nombreHabilidad)
            {
                return Instantiate(habi);
            }
        }
        return null;
    }

    public void ActualizarListaHabilidades()
    {
        habilidadesALanzar.ActualizaLista();
    }

    public void ActualizarBotonesHabilidades()
    {
        for (int i = 0; i < listaBotonesInterfaz[0].transform.childCount; i++)
        {
            listaBotonesInterfaz[0].transform.GetChild(i).GetComponentInChildren<InteractuarBotonHabilidad>().habilidad = jugador.habilidades[i];

            listaBotonesInterfaz[0].transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = jugador.habilidades[i].nombre;
        }
    }

    public void ActivaBotonesInterfaz()
    {       
        foreach (GameObject boton in listaBotonesInterfaz)
        {
            IBoton[] scripts = boton.GetComponentsInChildren<IBoton>();
       
            foreach (IBoton script in scripts) script.Activar();
        }
    }

    public void ActivaBotonesInterfaz(int indice)
    {
        IBoton[] scripts = listaBotonesInterfaz[indice].GetComponentsInChildren<IBoton>();

        foreach (IBoton script in scripts) script.Activar();
    }

    public void DesactivaBotonesInterfaz()
    {
        foreach (GameObject boton in listaBotonesInterfaz)
        {
            IBoton[] scripts = boton.GetComponentsInChildren<IBoton>();
            
            foreach(IBoton script in scripts) script.Desactivar();
        }
    }

    public void ActivaPersonajes()
    {
        foreach (GameObject personaje in listaObjetosPersonajesEscena) personaje.GetComponentInChildren<InteractuarPersonajes>().Activar();
    }

    public void DesactivaPersonajes()
    {
        foreach (GameObject personaje in listaObjetosPersonajesEscena) personaje.GetComponentInChildren<InteractuarPersonajes>().Desactivar();
    }

    public void CargaPersonajesEscena()
    {
        Personaje scriptPersonajeInstanciado;

        string idEscena = SceneManager.GetActiveScene().name;

        for (int i = 0; i < copiasEscenas[idEscena].personajes.Count; i++)
        {
            scriptPersonajeInstanciado = Instantiate(copiasEscenas[idEscena].personajes[i]);

            listaObjetosPersonajesEscena[i].transform.Find("Canvas").GetComponent<RectTransform>().localPosition = new Vector2(listaObjetosPersonajesEscena[i].GetComponent<BoxCollider2D>().offset.x, listaObjetosPersonajesEscena[i].GetComponent<BoxCollider2D>().size.y + 0.05f);

            listaObjetosPersonajesEscena[i].GetComponent<InteractuarPersonajes>().personaje = scriptPersonajeInstanciado;

            listaObjetosPersonajesEscena[i].GetComponentInChildren<BarraDeVida>().personaje = scriptPersonajeInstanciado;

            //listaObjetosPersonajesEscena[i].GetComponentInChildren<BarraDeVida>().barraAncho = listaObjetosPersonajesEscena[i].GetComponent<BoxCollider2D>().size.x;
        }

        listaObjetosPersonajesEscena.Insert(0, objetoJugador);

        ResetearObjetivosSeleccionables();
    }

    public void EscenaCompletada()
    {
        if (escenaActual.completada)
        {
            Destroy(cargaEscena.enemigos);
        }

        AbrirCerrarPuertas(true);

        escenaActual.completada = true;

        ResetearAttributosJugador();

        DesactivaBotonesInterfaz();
    }

    public void MuestraObjetivosSeleccionables(Habilidad habilidad, bool soyJugador)
    {
        switch (habilidad.tipoSeleccion)
        {
            case TipoSeleccion.SoloJugador:
                instance.objetoJugador.GetComponentInChildren<Outline>().SetOutlineVisible(true);
                break;

            case TipoSeleccion.SoloUnEnemigo:
            case TipoSeleccion.CualquierPersonaje:
            case TipoSeleccion.VariosEnemigos:

                for (int i = 0; i < listaObjetosPersonajesEscena.Count; i++)
                {
                    GameObject personaje = listaObjetosPersonajesEscena[i];

                    instance.CambiaColorOutline(personaje.GetComponent<InteractuarPersonajes>().personaje, soyJugador, personaje);

                    personaje.transform.Find("Canvas").transform.Find("Probabilidad").GetComponent<TextMeshProUGUI>().text = MuestraProbabilidades(personaje.GetComponent<InteractuarPersonajes>().personaje, habilidad).ToString() + "%";

                    personaje.transform.Find("Canvas").transform.Find("Probabilidad").gameObject.SetActive(true);

                    personaje.GetComponentInChildren<Outline>().SetOutlineVisible(tipoSelecciones[habilidad.tipoSeleccion][i]);

                    personaje.GetComponent<InteractuarPersonajes>().elegible = tipoSelecciones[habilidad.tipoSeleccion][i];
                }

                break;

            case TipoSeleccion.TodosLosEnemigos:

                foreach (GameObject personaje in listaObjetosPersonajesEscena.Skip(1))
                {
                    instance.CambiaColorOutline(personaje.GetComponent<InteractuarPersonajes>().personaje, soyJugador, personaje);

                    personaje.GetComponentInChildren<Outline>().SetOutlineVisible(true);
                }

                break;
        }
    }

    public int MuestraProbabilidades(Personaje objetivo, Habilidad habilidad)
    {
        float punteria = habilidad.personaje.punteria;
        float agilidad = objetivo.agilidad;
        float fuerza;

        if (habilidad.melee) fuerza = habilidad.personaje.fuerza + habilidad.fuerza;
        else fuerza = habilidad.fuerza;

        float resistencia = objetivo.resistencia;
        float penetracion = habilidad.penetracion;
        float salvacion = objetivo.salvacion;
        float salvacionInvulnerable = objetivo.salvacionInvulnerable;

        bool melee = habilidad.melee;

        return Mathf.RoundToInt((ResultadoProbabilidadPunteria(punteria, agilidad) + ResultadoProbabilidadFuerza(fuerza, resistencia, melee) + ResultadoProbabilidadSalvacion(penetracion, salvacion, salvacionInvulnerable)) / 300 * 100);
    }

    private float ResultadoProbabilidadPunteria(float punteria, float agilidad)
    {
        float probabilidad;

        if (punteria - agilidad > 5) probabilidad = 5 / (punteria - agilidad) * 100;
        else probabilidad = 5 / 6 * 100;

        return probabilidad;
    }

    private float ResultadoProbabilidadFuerza(float fuerza, float resistencia, bool melee)
    {
        float probabildiad;

        if (fuerza >= resistencia * 2)
        {
            probabildiad = 17;
        }
        else if (fuerza > resistencia)
        {
            probabildiad = 33;
        }
        else if (fuerza == resistencia)
        {
            probabildiad = 50;
        }
        else if (fuerza < resistencia && fuerza > resistencia / 2)
        {
            probabildiad = 66;
        }
        else
        {
            probabildiad = 83;
        }

        return probabildiad;
    }

    private float ResultadoProbabilidadSalvacion(float penetracion, float salvacion, float salvacionInvulnerable)
    {
        float probabilidad;

        if (salvacion - penetracion >= salvacionInvulnerable && salvacion - penetracion > 5) probabilidad = 5 / (salvacion - penetracion) * 100;
        else if(salvacion - penetracion < salvacionInvulnerable && salvacionInvulnerable > 5) probabilidad = 5 / salvacionInvulnerable * 100;
        else probabilidad = 5 / 6 * 100;

        return 100 - probabilidad;
    }
    public void SeleccionaJugador(Habilidad habilidad)
    {
        habilidad.objetivos.Add(instance.objetoJugador.GetComponent<InteractuarPersonajes>().personaje);
    }

    public void SeleccionaTodosLosPersonajes(Habilidad habilidad, bool SinJugador)
    {
        for (int i = 0; i < listaObjetosPersonajesEscena.Count; i++)
        {
            habilidad.objetivos.Add(listaObjetosPersonajesEscena[i].GetComponent<InteractuarPersonajes>().personaje);
        }

        if (SinJugador)
        {
            habilidad.objetivos.RemoveAt(0);
        }
    }

    public void ResetearObjetivosSeleccionables()
    {
        foreach (GameObject personaje in listaObjetosPersonajesEscena)
        {
            personaje.GetComponentInChildren<Outline>().SetOutlineVisible(personaje.GetComponent<InteractuarPersonajes>().elegible = false);

            personaje.transform.Find("Canvas").transform.Find("Probabilidad").gameObject.SetActive(false);
        }
    }

    public void AbrirCerrarPuertas(bool puedePresionarse)
    {
        foreach (GameObject puerta in listaPuertas)
        {
            puerta.GetComponent<Outline>().SetOutlineColor(2);
            puerta.GetComponent<Outline>().SetOutlineVisible(puerta.GetComponent<InteractuarPuerta>().puedePresionarse = puedePresionarse);
        }
    }

    public List<GameObject> ObtenerPersonajes(bool aliado)
    {
        List<GameObject> personajesFiltrados = new List<GameObject>();

        foreach (GameObject obj in listaObjetosPersonajesEscena)
        {
            InteractuarPersonajes componente = obj.GetComponentInChildren<InteractuarPersonajes>();

            if (componente != null && componente.aliado == aliado)
            {
                personajesFiltrados.Add(obj);
            }
        }

        if (aliado)
        {
            // Eliminar el objetoJugador si está en la lista
            personajesFiltrados.RemoveAll(obj => obj == objetoJugador);
        }

        return personajesFiltrados;
    }

    public void CargaJugador()
    {
        jugador = Instantiate(jugador);
        
        objetoJugador.transform.Find("Canvas").GetComponent<RectTransform>().localPosition = new Vector2(objetoJugador.GetComponent<BoxCollider2D>().offset.x, objetoJugador.GetComponent<BoxCollider2D>().size.y - 0.2f);

        objetoJugador.GetComponent<InteractuarPersonajes>().personaje = jugador;

        barraDeVida.personaje = jugador;
    }

    public void CargaTurno()
    {
        List<Personaje> objetivosDisponibles = new List<Personaje>();

        List<GameObject> listaEnemigos = ObtenerPersonajes(false);

        List<GameObject> listaAliados = ObtenerPersonajes(true);

        List<Habilidad> lHabilidades = new List<Habilidad>();

        Habilidad habilidad;

        if(listaAliados.Count > 0)
        {
            foreach (GameObject objetoAliado in listaAliados)
            {
                Personaje aliado = objetoAliado.GetComponent<InteractuarPersonajes>().personaje;

                if (aliado.heridasActuales <= 0) return;

                foreach (Habilidad habi in aliado.habilidades)
                {
                    lHabilidades.Add(habi);
                }

                for (int puntosAcciones = aliado.accionesMaximas; puntosAcciones > 0; puntosAcciones -= habilidad.coste)
                {
                    do
                    {
                        habilidad = lHabilidades[UnityEngine.Random.Range(0, aliado.habilidades.Count())];

                        habilidad.nombre = habilidad.name;

                        habilidad.velocidad += aliado.agilidad;

                        habilidad.personaje = aliado;

                    } while (habilidad.coste > puntosAcciones);

                    switch (habilidad.tipoSeleccion)
                    {
                        case TipoSeleccion.SoloJugador:

                            habilidad.objetivos.Add(instance.jugador);

                            break;

                        case TipoSeleccion.SoloUnEnemigo:

                            habilidad.objetivos.Add(listaEnemigos[UnityEngine.Random.Range(0, listaEnemigos.Count())].GetComponent<InteractuarPersonajes>().personaje);

                            break;

                        case TipoSeleccion.CualquierPersonaje:

                            habilidad.objetivos.Add(listaObjetosPersonajesEscena[UnityEngine.Random.Range(0, listaObjetosPersonajesEscena.Count())].GetComponent<InteractuarPersonajes>().personaje);

                            break;

                        case TipoSeleccion.TodosLosEnemigos:

                            foreach (GameObject objetoPersonaje in listaEnemigos)
                            {
                                habilidad.objetivos.Add(objetoPersonaje.GetComponent<InteractuarPersonajes>().personaje);
                            }

                            break;

                        case TipoSeleccion.SinSeleccionar:

                            foreach (GameObject objetoPersonaje in listaObjetosPersonajesEscena)
                            {
                                habilidad.objetivos.Add(objetoPersonaje.GetComponent<InteractuarPersonajes>().personaje);
                            }

                            break;
                    }

                    habilidad.personaje = aliado;

                    habilidadesALanzar.listaHabilidadesALanzar.Add(new KeyValuePair<Habilidad, bool>(Instantiate(habilidad), false));

                    habilidad.objetivos.Clear();
                }
            }
        }

        lHabilidades.Clear();

        foreach (GameObject objetoEnemigo in listaEnemigos)
        {
            Personaje enemigo = objetoEnemigo.GetComponent<InteractuarPersonajes>().personaje;

            if (enemigo.heridasActuales <= 0) return;

            foreach (Habilidad habi in enemigo.habilidades)
            {
                lHabilidades.Add(habi);
            }

            for (int puntosAcciones = enemigo.accionesMaximas; puntosAcciones > 0; puntosAcciones -= habilidad.coste)
            {
                do {
                    habilidad = lHabilidades[UnityEngine.Random.Range(0, enemigo.habilidades.Count())];

                    habilidad.nombre = habilidad.name;

                    habilidad.velocidad += enemigo.agilidad;

                    habilidad.personaje = enemigo;

                } while (habilidad.coste > puntosAcciones);
               
                switch (habilidad.tipoSeleccion)
                {
                    case TipoSeleccion.SoloJugador:

                        habilidad.objetivos.Add(instance.jugador);

                        break;

                    case TipoSeleccion.SoloUnEnemigo:

                        habilidad.objetivos.Add(listaEnemigos[UnityEngine.Random.Range(0, listaEnemigos.Count())].GetComponent<InteractuarPersonajes>().personaje);

                        break;

                    case TipoSeleccion.CualquierPersonaje:

                        habilidad.objetivos.Add(listaObjetosPersonajesEscena[UnityEngine.Random.Range(0, listaObjetosPersonajesEscena.Count())].GetComponent<InteractuarPersonajes>().personaje);

                        break;

                    case TipoSeleccion.TodosLosEnemigos:

                        foreach (GameObject objetoPersonaje in listaEnemigos)
                        {
                            habilidad.objetivos.Add(objetoPersonaje.GetComponent<InteractuarPersonajes>().personaje);
                        }

                        break;

                    case TipoSeleccion.SinSeleccionar:

                        foreach (GameObject objetoPersonaje in listaObjetosPersonajesEscena)
                        {
                            habilidad.objetivos.Add(objetoPersonaje.GetComponent<InteractuarPersonajes>().personaje);
                        }

                        break;
                }

                habilidad.personaje = enemigo;
                
                habilidadesALanzar.listaHabilidadesALanzar.Add(new KeyValuePair<Habilidad, bool>(Instantiate(habilidad), false));
              
                habilidad.objetivos.Clear();
            }
        }

        lHabilidades.Clear();

        ActualizarListaHabilidades();
    }

    public void CambiaColorOutline(Personaje objetivo, bool soyJugador, GameObject personaje)
    {
        if (objetivo == jugador && soyJugador || objetivo != jugador && !soyJugador) personaje.GetComponentInChildren<Outline>().SetOutlineColor(0);
        else personaje.GetComponentInChildren<Outline>().SetOutlineColor(1);
    }

    public void ResetearAttributosJugador()
    {
        jugador.punteria = tempJugador.punteria;
        jugador.habilidadCombate = tempJugador.habilidadCombate;
        jugador.habilidadEspecial = tempJugador.habilidadEspecial;
        jugador.fuerza = tempJugador.fuerza;
        jugador.resistencia = tempJugador.resistencia;
        jugador.salvacion = tempJugador.salvacion;
        jugador.accionesActuales = tempJugador.accionesActuales;
        jugador.accionesMaximas = tempJugador.accionesMaximas;

        GameObject panelHabilidades = listaBotonesInterfaz.ElementAt(0);

        foreach(InteractuarBotonHabilidad scr in panelHabilidades.GetComponentsInChildren<InteractuarBotonHabilidad>())
        {
            //probar con jugador temporal
            scr.ResetearHabilidad();
        }
    }

    public void Reiniciar()
    {
        copiasEscenas.Clear();

        objetoJugador.SetActive(true);

        btnHasMuerto.SetActive(false);

        jugador.heridasActuales = jugador.heridasMaximas;

        CompruebaYCargaEscenas(escena1);

        ActivaBotonesInterfaz();
    }

    public void Salir()
    {
        Application.Quit();
    }
}
