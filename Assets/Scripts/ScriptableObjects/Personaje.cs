using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Nuevo Personaje")]
public class Personaje : ScriptableObject
{
    public string nombre;

    public int nivel;
    public int punteria;
    public int habilidadCombate;
    public int habilidadEspecial;
    public int fuerza;
    public int probCrit = 5;      // Probabilidad Crítica (%)
    public int dañoCrit = 50;     // Daño Crítico (%, +50 % ⇒ x1 ,5)
    public int resistencia;
    public int agilidad;
    public int heridasMaximas;
    public int heridasActuales;
    public int salvacion;
    public int salvacionInvulnerable;
    public int accionesMaximas;
    public int accionesActuales;
    public int experienciaActual;
    public int requisitoNivel;

    public GameObject gameObject;

    public List<Estados> estados = new List<Estados>();

    public List<Habilidad> habilidades = new List<Habilidad>();

    [Header("Animaciones del personaje")]
    public List<AnimacionDataLookup> animacionDataLookups = new List<AnimacionDataLookup>();
}
