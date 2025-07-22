using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using SHG.AnimatorCoder;
using Unity.VisualScripting;
using static SHG.AnimatorCoder.Animations;
using UnityEngine.VFX;
using System.Runtime.CompilerServices;

[CreateAssetMenu(fileName = "Nueva Habilidad")]
public class Habilidad : ScriptableObject, IComparable
{
    [NonSerialized] private bool _impactoRecibido;

    /// <summary>Aplica todas las acciones de la habilidad exactamente una vez.</summary>
    private void EjecutarTodasAcciones()
    {
        if (_impactoRecibido) return;          // evita dobles llamadas
        foreach (Accion acc in acciones) EjecutarAccion(acc);
        _impactoRecibido = true;
    }


    public string nombre;

    [TextArea(10, 15)]
    public string descripcion;

    public int coste;
    public int velocidad;
    public int fuerza;
    public int penetracion;
    [Header("Tipo de daño")]
    public ElementoDaño tipoDaño = ElementoDaño.Fisico;
    public int daño;
    [Header("Crítico (modificadores)")]
    public bool permiteCritico = true;

    [Tooltip("±% a la probabilidad del personaje")]
    [Range(-100, 100)]
    public int probCritExtra = 0;

    [Tooltip("±% al bonus de daño crítico del personaje")]
    public int dañoCritExtra = 0;

    public List<Accion> acciones = new List<Accion>();

    public List<Animations> animaciones = new List<Animations>();

    public Personaje personaje;

    public List<Personaje> objetivos = new List<Personaje>();

    public TipoSeleccion tipoSeleccion;

    public int cantidad = 2;

    public bool usosLimitados;

    public bool melee;

    public int numeroDeUsos;

    public TierHabilidad tier;

    public AudioClip sonido;

    public GameObject invocacion;
    private FloatingTextManager ftManager =>
     GameManager.instance ? GameManager.instance.floatingTextManager : null;

    [NonSerialized] private float _popupDelay;   // acumulador por habilidad
    [SerializeField] private float delayMin = 0.08f;
    [SerializeField] private float delayMax = 0.12f;

    private int totalDañoInfligido;

    /* ───────── buffers para el “Total Daño” ───────── */
    List<int> _totalesBuffer = new();   // daño de cada golpe
    bool _bufferTotalesActiva;          // true entre EventoImpactoConDuracion y flush
    List<(Personaje, float)> _barraBuffer = new();

    private void Play(string id, AnimationData animationData, int layer = 0)
    {
        GameManager.instance.animationManager.PlayAnimation(id, animationData, layer);
    }

    // Habilidad.cs  (versión resumida)
    public IEnumerator Usar()
    {
        int ventanaFrames = 0;
        float tVentanaInicio = 0f;

        // ── 1 · VentanaImpacto ─────────────────────────────
        System.Action<int> handlerVentana = frames =>
        {
            ventanaFrames = frames;

            ftManager?.BeginBuffer();          // pop-ups
            _totalesBuffer.Clear();            // ← resetea
            _bufferTotalesActiva = true;       // ← ON
        };

        GameManager.OnVentanaImpacto += handlerVentana;

        /// ── 2 · ImpactoHabilidad  (daño + flush) ───────────
        System.Action handlerImpacto = () =>
        {
            EjecutarTodasAcciones();                  // aplica Acciones 1 sola vez

            /* ---------- POP-UPS individuales ---------- */
            if (ftManager && ventanaFrames > 0)
                ftManager.EndBuffer(ventanaFrames / 60f);   // 60 fps

            /* ---------- FLUSH ---------- */
            if (_bufferTotalesActiva)
            {
                float ventanaSegs = ventanaFrames / 60f;

                /* 1) TotalDamage (enemigos) ----------------- */
                int nTot = _totalesBuffer.Count;
                for (int i = 0; i < nTot; i++)
                {
                    float delay = nTot == 1 ? 0f : ventanaSegs * i / (nTot - 1);
                    GameManager.instance.MostrarTotalDaño(
                        _totalesBuffer[i], TipoTotalPopup(), delay);
                }

                /* 2) Barras de vida (todos los objetivos) --- */
                int nBar = _barraBuffer.Count;
                for (int i = 0; i < nBar; i++)
                {
                    float delay = nBar == 1 ? 0f : ventanaSegs * i / (nBar - 1);
                    var (obj, porc) = _barraBuffer[i];
                    GameManager.instance.AnimarBarraVida(obj, porc, delay);
                }

                /* ---------- cleanup ---------- */
                _totalesBuffer.Clear();
                _barraBuffer.Clear();
                _bufferTotalesActiva = false;
            }
        };
        GameManager.OnImpactoHabilidad += handlerImpacto;


        // Antes de la suscripción:
        Action handlerInicio = () =>
        {
            tVentanaInicio = Time.time;
            ftManager?.BeginBuffer();
        };

        Action handlerFin = () =>
        {
            if (ftManager)
            {
                float dur = Time.time - tVentanaInicio;
                ftManager.EndBuffer(dur);
                if (_bufferTotalesActiva)
                {
                    float ventanaSegs = ventanaFrames / 60f;       // 60 fps por defecto
                    int n = _totalesBuffer.Count;
                    if (n > 0)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            float delay = n == 1 ? 0f : ventanaSegs * i / (n - 1);

                            // Total Damage
                            GameManager.instance.MostrarTotalDaño(_totalesBuffer[i], TipoTotalPopup(), delay);

                            // Barra vida
                            var par = _barraBuffer[i];
                            GameManager.instance.AnimarBarraVida(par.Item1, par.Item2, delay);
                        }
                    }
                    _barraBuffer.Clear();

                        _totalesBuffer.Clear();
                    _bufferTotalesActiva = false;                  // ← OFF
                }
            }
        };

        /* ─────────────── RESET ─────────────── */
        _popupDelay = 0f;          // delay para pop-ups
        totalDañoInfligido = 0;           // acumulador lógico

        if (GameManager.instance?.totalDamageDisplay != null)
            GameManager.instance.totalDamageDisplay.Resetear();   // texto = 0, alpha = 0

        bool accionesAplicadas = false;

        /* ───────── Suscripción al impacto ───────── */
        System.Action handler = () =>
        {
            if (!accionesAplicadas)
            {
                EjecutarTodasAcciones();    // aplica Acciones solo 1 vez
                accionesAplicadas = true;
            }
        };
        GameManager.OnImpactoHabilidad += handler;

        /* ───────── Animaciones / SFX ───────── */
        float duracionMax = 0f;

        foreach (var anim in animaciones)
        {
            float len = GetAnimationDuration();          // tu helper
            duracionMax = Mathf.Max(duracionMax, len);

            Play(personaje.gameObject.GetInstanceID().ToString(),
                 new(anim, false, new(), 0.1f));         // sin loop
        }

        if (sonido != null)
            GameManager.instance.soundEffect.PlayOneShot(sonido);

        /* ───────── Espera a que termine lo más largo ───────── */
        yield return new WaitForSeconds(duracionMax + 0.05f);

        /* Fallback por si no llegó el evento de impacto */
        if (!accionesAplicadas)
            EjecutarTodasAcciones();

        /* Volver al Idle */
        Play(personaje.gameObject.GetInstanceID().ToString(),
             new(Animations.IDLE1, true, new(), 0.05f));

        /* Limpieza del handler */
        GameManager.OnImpactoHabilidad -= handler;

        // ── Limpieza ───────────────────────────────────────
        GameManager.OnVentanaImpacto -= handlerVentana;
        GameManager.OnImpactoHabilidad -= handlerImpacto;
    }


    private void EjecutarAccion(Accion accion)
    {
        switch (accion)
        {
            case Accion.Disparo:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.punteria, fuerza, objetivo, daño);
                }

                break;

            case Accion.GolpeMasFuerza:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.habilidadCombate, personaje.fuerza + fuerza, objetivo, daño);
                }

                break;

            case Accion.GolpeMasFuerzaMasHabilidad:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.habilidadCombate, personaje.fuerza + personaje.habilidadEspecial + fuerza, objetivo, daño);
                }

                break;

            case Accion.Golpe:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.habilidadCombate, personaje.fuerza, objetivo, daño);
                }

                break;

            case Accion.DisparoPierdeSaludRestante:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.punteria, fuerza, objetivo, daño);
                }

                personaje.heridasActuales -= personaje.heridasActuales / 8;

                break;

            case Accion.DisparoDañoPorcentualVidaActual:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.punteria, fuerza, objetivo, objetivo.heridasActuales * 15 / 100);
                }

                break;

            case Accion.MejoraFuerzaYAccionesMaximas:

                personaje.fuerza += fuerza;

                personaje.accionesMaximas += 1;

                //Anima(personaje, "Mejora");

                break;

            case Accion.MejoraAgilidad:

                personaje.agilidad += fuerza;

                break;

            case Accion.ReduccionResistencia:

                personaje.resistencia -= 1;

                break;

            case Accion.GolpeUnObjetivoMasFuerzaMasDañoPorcentual:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.habilidadCombate, personaje.fuerza + fuerza, objetivo, daño + objetivo.heridasActuales * 10 / 100);
                }

                break;

            case Accion.GolpeYSiMataCuracion:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.habilidadCombate, personaje.fuerza + fuerza, objetivo, daño);

                    if (objetivo.heridasActuales <= 0) Curar(personaje);
                }

                break;

            case Accion.Curar:

                foreach (Personaje objetivo in objetivos) Curar(objetivo);

                break;

            case Accion.CurarUnoMismo:

                Curar(personaje);

                break;

            case Accion.DisparoDañoMasDañoAleatorio:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.punteria, fuerza, objetivo, daño + Roll(6));
                }

                break;

            case Accion.MejoraPunteria:

                foreach (Personaje objetivo in objetivos)
                {
                    objetivo.punteria -= personaje.habilidadEspecial / 2;
                }

                break;

            case Accion.Disparo1d6:

                foreach (Personaje objetivo in objetivos)
                {
                    for (int i = 0; i < Roll(6); i++) RealizaTiradas(personaje.punteria, fuerza, objetivo, daño);
                }

                break;

            case Accion.Disparo1d3DañoMas1d3:

                foreach (Personaje objetivo in objetivos)
                {
                    for (int i = 0; i < Roll(1, 3); i++) RealizaTiradas(personaje.punteria, fuerza, objetivo, daño + Roll(1, 3));
                }

                break;

            case Accion.DisparoDañoMitadVidaActualObjetivo:

                foreach (Personaje objetivo in objetivos)
                {
                    RealizaTiradas(personaje.punteria, fuerza, objetivo, objetivo.heridasActuales / 2);
                }

                break;
            case Accion.Invoca:

                Transform posicionTransform = GameManager.instance.objetoJugador.transform.Find("posicionInvocacion");

                if (posicionTransform != null) Instantiate(invocacion, posicionTransform.position, posicionTransform.rotation);

                break;
            case Accion.DisparoDirecto:

                foreach (Personaje objetivo in objetivos)
                {
                    AplicaDañoDirecto(objetivo, daño);
                }

                break;
        }
    }


    public int CompareTo(object obj)
    {
        Habilidad hab = (Habilidad)obj;

        return velocidad > hab.velocidad ? 1 : 0;
    }

    /// <summary>
    /// Ejecuta hit ▸ wound ▸ save y aplica daño, pop-ups, contador total
    /// con soporte de crítico (% prob + % bonus).
    /// </summary>
    private void RealizaTiradas(int punteria, int fuerza, Personaje objetivo, int dañoBase)
    {
        float rnd;                       // se reutiliza en los tres caminos

        /* ───────── HIT ROLL ───────── */
        if (HitRoll(punteria - objetivo.agilidad))
        {
            /* ───────── WOUND ROLL ───────── */
            if (WoundRoll(fuerza, objetivo))
            {
                /* ───────── SAVING THROW ───────── */
                if (!SavingThrow(objetivo))
                {
                    /* =======  D A Ñ O  (con crítico)  ======= */
                    bool esCritico = false;
                    int deltaDaño = dañoBase;

                    if (permiteCritico)
                    {
                        int chance = Mathf.Clamp(
                            personaje.probCrit + probCritExtra, 0, 100);   // 0-100 %

                        esCritico = Random.Range(1, 101) <= chance;

                        if (esCritico)
                        {
                            int bonusPct = Mathf.Max(
                                0, personaje.dañoCrit + dañoCritExtra);    // %, p.e. 40
                            float mult = 1f + bonusPct / 100f;           // 1.4
                            deltaDaño = Mathf.RoundToInt(deltaDaño * mult);
                        }
                    }

                    // 1) Aplica el daño
                    objetivo.heridasActuales -= deltaDaño;

                    /* 2) Delay individual para pop-up */
                    rnd = Random.Range(delayMin, delayMax);
                    float delayGolpe = _popupDelay + rnd;
                    _popupDelay += rnd;

                    /* 3) Barra de vida (roja) */
                    float porcentaje = (float)objetivo.heridasActuales / objetivo.heridasMaximas;

                    if (_bufferTotalesActiva)               // ventana ON → se difiere
                    {
                        _barraBuffer.Add((objetivo, porcentaje));   // solo UNA vez aquí
                    }
                    else                                     // modo antiguo → inmediato
                    {
                        GameManager.instance.AnimarBarraVida(objetivo, porcentaje, delayGolpe);
                    }

                    /* 4) Contador de daño total (solo enemigos) */
                    if (objetivo != GameManager.instance.jugador)
                    {
                        totalDañoInfligido += deltaDaño;

                        if (_bufferTotalesActiva)           // ventana ON → se difiere
                        {
                            _totalesBuffer.Add(deltaDaño);  // ← solo el total, sin barra
                        }
                        else                                // modo antiguo
                        {
                            GameManager.instance.StartCoroutine(
                                AddTotalAfterDelay(deltaDaño, delayGolpe));
                        }
                    }

                    /* pop-up de daño / crítico */
                    ftManager.Mostrar(
                        TipoPopup(esCritico),          // ← NUEVO
                        esCritico ? $"¡{deltaDaño}!" : deltaDaño.ToString(),
                        objetivo,
                        null,
                        esCritico ? 1.3f : 1f,
                        delayGolpe);
                }
                else    /* ---------- SALVACIÓN ---------- */
                {
                    rnd = Random.Range(delayMin, delayMax);
                    ftManager.Mostrar(
                        FloatingTextTipo.Salvacion,
                        "SALVACIÓN",
                        objetivo,
                        null,
                        1f,
                        _popupDelay + rnd);
                    _popupDelay += rnd;
                }
            }
            else        /* ---------- RESISTIDO ---------- */
            {
                rnd = Random.Range(delayMin, delayMax);
                ftManager.Mostrar(
                    FloatingTextTipo.Resistido,
                    "RESISTIDO",
                    objetivo,
                    null,
                    1f,
                    _popupDelay + rnd);
                _popupDelay += rnd;
            }
        }
        else            /* ---------- FALLO ---------- */
        {
            rnd = Random.Range(delayMin, delayMax);
            ftManager.Mostrar(
                FloatingTextTipo.Fallo,
                "FALLO",
                objetivo,
                null,
                1f,
                _popupDelay + rnd);
            _popupDelay += rnd;
        }
    }

    FloatingTextTipo TipoPopup(bool critico)
    {
        return tipoDaño switch
        {
            ElementoDaño.Igneo => critico ? FloatingTextTipo.IgneoCritico : FloatingTextTipo.Igneo,
            ElementoDaño.Toxico => critico ? FloatingTextTipo.ToxicoCritico : FloatingTextTipo.Toxico,
            ElementoDaño.Plasma => critico ? FloatingTextTipo.PlasmaCritico : FloatingTextTipo.Plasma,
            _ => critico ? FloatingTextTipo.FisicoCritico : FloatingTextTipo.Fisico
        };
    }

    FloatingTextTipo TipoTotalPopup()
    {
        return tipoDaño switch
        {
            ElementoDaño.Igneo => FloatingTextTipo.IgneoTotal,
            ElementoDaño.Toxico => FloatingTextTipo.ToxicoTotal,
            ElementoDaño.Plasma => FloatingTextTipo.PlasmaTotal,
            _ => FloatingTextTipo.FisicoTotal
        };
    }


    private bool HitRoll(int punteria)
    {
        int resultado = Roll(punteria);

        if (resultado > 5 && resultado != 1 || resultado == punteria) return true;
        else return false;
    }

    private bool WoundRoll(int fuerza, Personaje objetivo)
    {
        int resultado = Roll(6);
        int requisito;

        if (fuerza >= objetivo.resistencia * 2)
        {
            requisito = 2;
        }
        else if (fuerza > objetivo.resistencia)
        {
            requisito = 3;
        }
        else if (fuerza == objetivo.resistencia)
        {
            requisito = 4;
        }
        else if (fuerza < objetivo.resistencia && fuerza > objetivo.resistencia / 2)
        {
            requisito = 5;
        }
        else
        {
            requisito = 6;
        }

        if (resultado >= requisito && resultado != 1) return true;
        else return false;
    }

    private bool SavingThrow(Personaje objetivo)
    {
        int resultado;

        if (objetivo.salvacion - penetracion >= objetivo.salvacionInvulnerable) resultado = Roll(objetivo.salvacion - penetracion);
        else resultado = Roll(objetivo.salvacionInvulnerable);

        if (resultado > 5 && resultado != 1) return true;
        else return false;
    }

    private int Roll(int maxRange)
    {
        return Random.Range(1, maxRange + 1);
    }

    private int Roll(int n1, int n2)
    {
        return Random.Range(n1, n2 + 1);
    }

    private void Curar(Personaje objetivo)
    {
        int curacion = Mathf.RoundToInt(daño * (1 + personaje.habilidadEspecial * 0.01f));

        objetivo.heridasActuales += curacion;
        objetivo.heridasActuales = Mathf.Max(0, objetivo.heridasActuales);

        //Mostrar el Floating Text
        GameManager.instance.floatingTextManager.Mostrar(
            FloatingTextTipo.Cura,
            curacion.ToString(),
            objetivo,
            null,
            1f,
            0f
        );

        // Animar barra de vida
        float pctVida = objetivo.heridasActuales / (float)objetivo.heridasMaximas;
        GameManager.instance.AnimarBarraVida(objetivo, pctVida, 0.5f);
    
    }

    private void AplicaDañoDirecto(Personaje objetivo, int dañoBase)
    {
        /* ───── CÁLCULO DE CRÍTICO ────────────────────── */
        bool esCritico = false;
        int deltaDaño = dañoBase;

        if (permiteCritico)
        {
            int chance = Mathf.Clamp(
                personaje.probCrit + probCritExtra, 0, 100);   // % de crítico

            esCritico = Random.Range(1, 101) <= chance;

            if (esCritico)
            {
                int bonusPct = Mathf.Max(
                    0, personaje.dañoCrit + dañoCritExtra);    // % bonus daño crit
                float mult = 1f + bonusPct / 100f;             // ej. 1.5
                deltaDaño = Mathf.RoundToInt(deltaDaño * mult);
            }
        }

        /* ───── RESTA HERIDAS ─────────────────────────── */
        objetivo.heridasActuales -= deltaDaño;

        /* ───── TIMING PARA POP-UP ─────────────────────── */
        float rnd = Random.Range(delayMin, delayMax);
        float delayGolpe = _popupDelay + rnd;
        _popupDelay += rnd;

        /* ---------- sincr. barra ---------- */
        float porcentaje = (float)objetivo.heridasActuales / objetivo.heridasMaximas;
        if (_bufferTotalesActiva)
            _barraBuffer.Add((objetivo, porcentaje));
        else
            GameManager.instance.AnimarBarraVida(objetivo, porcentaje, delayGolpe);

        /* ───── CONTADOR TOTAL DAÑO ───────────────────── */
        if (objetivo != GameManager.instance.jugador)
        {
            if (_bufferTotalesActiva)              // ventana activa ▸ lo guardamos
                _totalesBuffer.Add(deltaDaño);
            else                                   // flujo antiguo ▸ Coroutine
                GameManager.instance.StartCoroutine(
                    AddTotalAfterDelay(deltaDaño, delayGolpe));
        }

        /* ───── POP-UP INDIVIDUAL ─────────────────────── */
        ftManager.Mostrar(
            TipoPopup(esCritico),                         // mismo helper que RealizaTiradas
            esCritico ? $"¡{deltaDaño}!" : deltaDaño.ToString(),
            objetivo,
            null,
            esCritico ? 1.3f : 1f,                       // escala mayor si es crítico
            delayGolpe);
    }


    public float GetAnimationDuration()
    {
        if (personaje == null)
        {
            Debug.LogWarning("No hay referencia al Personaje.");
            return 0f;
        }

        // Si quieres seguir chequeando el Animator (por seguridad)
        Animator[] animators = personaje.gameObject.GetComponentsInChildren<Animator>();

        Animator animator = null;
        foreach (var a in animators)
        {
            if (a.GetComponentInParent<Canvas>() == null)
            {
                animator = a;
                break;
            }
        }

        if (animator == null)
        {
            Debug.LogWarning("No se encontró un Animator válido.");
            return 0f;
        }

        string nombreAnim = animaciones.Count > 0 ? animaciones[0].ToString() : "";
        return personaje.GetDuracionAnimacion(nombreAnim);
    }



    private IEnumerator AddTotalAfterDelay(int delta, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (GameManager.instance.totalDamageDisplay != null)
            GameManager.instance.MostrarTotalDaño(delta, TipoTotalPopup(), delay);
    }
}
