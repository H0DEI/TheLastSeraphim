using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using SHG.AnimatorCoder;
using Unity.VisualScripting;
using UnityEditor.Animations;

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

    [TextArea(10,15)]
    public string descripcion;

    public int coste;
    public int velocidad;
    public int fuerza;
    public int penetracion;
    public int daño;

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

    private void Play(string id, AnimationData animationData, int layer = 0)
    {
        GameManager.instance.animationManager.PlayAnimation(id, animationData, layer);
    }

    private void PlayCanvas(string id, AnimationData animationData, int layer = 0)
    {
        GameManager.instance.animationManager.PlayCanvas(id, animationData, layer);
    }

    // Habilidad.cs  (versión resumida)
    public IEnumerator Usar()
    {
        bool accionesAplicadas = false;

        // 1 · suscribirse al impacto
        System.Action handler = () =>
        {
            if (!accionesAplicadas)
            {
                EjecutarTodasAcciones();
                accionesAplicadas = true;
            }
        };
        GameManager.OnImpactoHabilidad += handler;

        /* ───────────────────────────────────────────────
         * 2 · reproducir animaciones/SFX y calcular la más larga
         * ─────────────────────────────────────────────── */
        float duracionMax = 0f;

        foreach (var anim in animaciones)
        {
            // Usa tu helper para obtener la duración del clip concreto
            float len = GetAnimationDuration();      // ← implementa o ya existe
            duracionMax = Mathf.Max(duracionMax, len);

            // Reproduce el clip SIN loop
            Play(personaje.gameObject.GetInstanceID().ToString(),
                 new(anim, false, new(), 0.1f));
        }

        // Sonido opcional
        if (sonido) GameManager.instance.soundEffect.PlayOneShot(sonido);

        /* ───────────────────────────────────────────────
         * 3 · esperar la animación más larga (+ margen)
         * ─────────────────────────────────────────────── */
        yield return new WaitForSeconds(duracionMax + 0.05f);

        // 4 · fallback por si no llegó el evento
        if (!accionesAplicadas)
            EjecutarTodasAcciones();

        // 5 · volver al Idle
        Play(personaje.gameObject.GetInstanceID().ToString(),
             new(Animations.IDLE1, true, new(), 0.05f));

        // 6 · limpieza
        GameManager.OnImpactoHabilidad -= handler;
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
                    RealizaTiradas(personaje.punteria, fuerza, objetivo, objetivo.heridasActuales/2);
                }

                    break;
            case Accion.Invoca:

                Transform posicionTransform = GameManager.instance.objetoJugador.transform.Find("posicionInvocacion");

                if (posicionTransform != null) Instantiate(invocacion, posicionTransform.position, posicionTransform.rotation);

                break;
            }
        }
    

    public int CompareTo(object obj)
    {
        Habilidad hab = (Habilidad)obj;

        return velocidad > hab.velocidad ? 1 : 0;
    }

    private void RealizaTiradas(int punteria, int fuerza, Personaje objetivo, int daño)
    {
        if (HitRoll(punteria - objetivo.agilidad))
        {
            if (WoundRoll(fuerza, objetivo))
            {
                if (!SavingThrow(objetivo))
                {
                    objetivo.heridasActuales -= daño;

                    Anima(objetivo, daño.ToString(), Color.red);
                    Play(objetivo.gameObject.GetInstanceID().ToString(), new(Animations.HIT, true, new(), 0.2f));
                }
                else
                {
                    Anima(objetivo, "Saved", Color.yellow);
                }
            }
            else
            {
                Anima(objetivo, "Resisted", Color.cyan);
            }
        }
        else
        {
            Anima(objetivo, "Miss", Color.grey);
        }
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
        int heal;

        if (objetivo.heridasActuales > 0 && objetivo.heridasActuales < objetivo.heridasMaximas)
        {
            heal = Roll(personaje.habilidadEspecial);

            objetivo.heridasActuales += heal;

            if (objetivo.heridasActuales > objetivo.heridasMaximas) objetivo.heridasActuales = objetivo.heridasMaximas;

            AnimaValue(objetivo, "Curar", heal.ToString());
        }
    }

    public float GetAnimationDuration()
    {
        Animator[] animators = personaje.gameObject.GetComponentsInChildren<Animator>();

        Animator animator = null;

        // Buscar el Animator que NO esté en un Canvas
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
            Debug.LogWarning("No se encontró un Animator válido (excluyendo el Canvas).");
            return 0f;
        }

        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            Debug.LogWarning("El Animator no tiene un AnimatorController válido.");
            return 0f;
        }

        var stateMachine = controller.layers[0].stateMachine;

        foreach (var state in stateMachine.states)
        {
            if (state.state.name == animaciones[0].ToString())
            {
                Motion motion = state.state.motion;
                if (motion is AnimationClip clip)
                {
                    return clip.length;
                }
            }
        }

        Debug.LogWarning("Animación no encontrada en el Animator seleccionado.");
        return 0f;
    }

    private void Anima(Personaje objetivo, String animacion, Color color)
    {
        GameManager.instance.textManager.ShowFloatingText(objetivo.gameObject, animacion, color);
    }

    private void AnimaValue(Personaje objetivo, String text, String value)
    {
        //objetivo.gameObject.transform.Find("Canvas").transform.Find("wounded").GetComponent<TextMeshProUGUI>().text = value;
        //!!!!!!!!!!!!!!!!!!!!



        //objetivo.gameObject.GetComponent<Animator>().SetTrigger(text);
    }
}
