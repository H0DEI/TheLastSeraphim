using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using SHG.AnimatorCoder;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "Nueva Habilidad")]
public class Habilidad : ScriptableObject, IComparable
{
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

    private void Play(string id, AnimationData animationData, int layer = 0)
    {
        GameManager.instance.animationManager.PlayAnimation(id, animationData, layer);
    }

    private void PlayCanvas(string id, AnimationData animationData, int layer = 0)
    {
        GameManager.instance.animationManager.PlayCanvas(id, animationData, layer);
    }

    public void Usar()
    {
       foreach (Animations animacion in animaciones)
       {
           //if(melee) GameManager.instance.objetoJugador.GetComponent<PunchMover>().objetivo = objetivos[0].GetComponent<Transform>();

           Play(personaje.gameObject.GetInstanceID().ToString(), new(animacion, true, new(), 0.2f));
       }

        
           // AnimationData previousAnimation = null;
           //
           // // Crear la cadena de animaciones desde la lista `animaciones`.
           // foreach (Animations animacion in animaciones)
           // {
           //     var currentAnimation = new AnimationData(animacion, true, new(), 0.2f);
           //     if (previousAnimation != null)
           //     {
           //         // Encadenar la animación actual con la anterior.
           //         previousAnimation.nextAnimation = currentAnimation;
           //     }
           //     else
           //     {
           //         // Iniciar la primera animación directamente.
           //         Play(personaje.gameObject.GetInstanceID().ToString(), currentAnimation);
           //     }
           //     previousAnimation = currentAnimation;
           // }
           //
           // // Asegurar que la última animación regrese a la predeterminada.
           // if (previousAnimation != null)
           // {
           //     //Debug.Log($"Agregando IDLE1 al final de la cadena");
           //     previousAnimation.nextAnimation = new AnimationData(Animations.IDLE1, false, null, 0.2f);
           // }

            // Reproducir sonido si aplica.
            if (sonido != null) GameManager.instance.soundEffect.PlayOneShot(sonido);

            // Ejecutar las acciones asociadas a la habilidad.
            foreach (Accion accion in acciones)
            {
                EjecutarAccion(accion);
            }
        
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

                    //Anima(personaje, "Mejora");

                    break;

                case Accion.ReduccionResistencia:

                personaje.resistencia -= 1;

                //Anima(personaje, "Desmejora");

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

    private void Anima(Personaje objetivo, String animacion, Color color)
    {
        GameManager.instance.textManager.ShowFloatingText(objetivo.gameObject, animacion, color);

        //objetivo.gameObject.GetComponent<Animator>().SetTrigger(animacion);

        //PlayCanvas(objetivo.gameObject.GetInstanceID().ToString(), new(Animations.MISS, true, new(), 0.2f));
    }

    private void AnimaValue(Personaje objetivo, String text, String value)
    {
        //objetivo.gameObject.transform.Find("Canvas").transform.Find("wounded").GetComponent<TextMeshProUGUI>().text = value;
        //!!!!!!!!!!!!!!!!!!!!



        //objetivo.gameObject.GetComponent<Animator>().SetTrigger(text);
    }
}
