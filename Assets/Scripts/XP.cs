using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class XP : MonoBehaviour
{
    public int xp;

    public List<Habilidad> TierB; 
    public List<Habilidad> TierA; 
    public List<Habilidad> TierS; 
    public List<Habilidad> Upgrades;

    public Habilidad[] TodasHabilidades;

    public GameObject[] descripciones;

    public GameObject InterfazJugable;
    public GameObject LevelUp;

    public List<GameObject> BtnOpts;

    private bool upgrade;

    private int indice;

    private string nombreMejora;

    private GameManager instancia;

    private List<Habilidad> Temp = new List<Habilidad>();
    private List<Habilidad> TempMejoras;

    private Habilidad habilidad;

    private float nivelPrevio;

    private static readonly System.Random random = new System.Random();

    private void Awake()
    {
        instancia = GameManager.instance;

        instancia.XP = this;

        TodasHabilidades = Resources.LoadAll<Habilidad>("");

        foreach(Habilidad habilidad in TodasHabilidades)
        {
            habilidad.nombre = habilidad.name;

            switch (habilidad.tier)
            {
                case TierHabilidad.TierB:

                    TierB.Add(habilidad);

                break;

                case TierHabilidad.TierA:

                    TierA.Add(habilidad);

                break;

                case TierHabilidad.TierS:

                    TierS.Add(habilidad);

                    break;

                case TierHabilidad.Upgrade:

                    Upgrades.Add(habilidad);

                break;
            }
        }

        gameObject.SetActive(false);
    }

    public void ExperienciaEscena()
    {
        xp = instancia.escenaActual.xp;

        foreach(GameObject enemigo in instancia.ObtenerPersonajes(false))
        {
            xp += enemigo.GetComponent<InteractuarPersonajes>().personaje.nivel;
        }
    }
    public void ComprovarNivel()
    {
        if (instancia.jugador.experienciaActual >= instancia.jugador.requisitoNivel)
        {
            nivelPrevio = instancia.jugador.nivel;

            instancia.jugador.experienciaActual -= instancia.jugador.requisitoNivel;

            instancia.jugador.requisitoNivel += 2;

            instancia.jugador.nivel += 1;

            SubidaNivel();
        }
    }


    public void SubidaNivel()
    {
        instancia.informacionDescripciones.bloqueoDescripcion = true;

        InterfazJugable.SetActive(false);
        instancia.AbrirCerrarPuertas(false);
        instancia.objetoJugador.GetComponent<InteractuarPersonajes>().cursorDesactivado = true;
        LevelUp.SetActive(true);
        

        for (int i = 0; i < 3; i++)
        {
            int rnd = random.Next(101);

            if (rnd > 94)
            {
                Temp = new List<Habilidad>(TierS);
            }
            else if(rnd > 69)
            {
                Temp = new List<Habilidad>(TierA);
            }
            else if(rnd > 34)
            {
                Temp = new List<Habilidad>(TierB);
            }
            else if (rnd > -1)
            {
                TempMejoras = new List<Habilidad>(Upgrades);

                upgrade = true;
            }

            if (upgrade)
            {
                foreach (Habilidad habi in instancia.jugador.habilidades)
                {
                    indice = (habi.nombre.LastIndexOf("+") + 1);

                    if (indice == 0) nombreMejora = habi.nombre + " +1";
                    else nombreMejora = habi.nombre.Substring(0, indice) + (int.Parse(habi.nombre.Substring(indice)) + 1).ToString();

                    for (int j = 0; j < TempMejoras.Count; j++)
                    {
                        if (TempMejoras[j].nombre == nombreMejora)
                        {
                            Temp.Add(TempMejoras[j]);
                        }
                    }
                }
            }
            else
            {
                foreach (Habilidad habi in instancia.jugador.habilidades)
                {
                    for (int j = 0; j < Temp.Count; j++)
                    {
                        if (Temp[j].nombre == habi.nombre)
                        {
                            Temp.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            
            habilidad = Instantiate(Temp[random.Next(Temp.Count)]);
                            
            BtnOpts[i].GetComponentInChildren<InteractuarLevelUp>().habilidad = habilidad;

            BtnOpts[i].GetComponentInChildren<InteractuarLevelUp>().esMejora = upgrade;

            if (upgrade)
            {
                descripciones[i].GetComponentInChildren<InformacionDescripciones>().MuestraHabilidadMejorada(ObtenerHabilidadAnterior(habilidad), habilidad, descripciones[i].GetComponentInChildren<InformacionDescripciones>().CompararDescripcionesAvanzado(ObtenerHabilidadAnterior(habilidad).descripcion, habilidad.descripcion));
            }
            else
            {
                descripciones[i].GetComponentInChildren<InformacionDescripciones>().MuestraInformacionHabilidad(habilidad);
            }


            if (Temp != null) Temp.Clear();

            if (TempMejoras != null) TempMejoras.Clear();

            upgrade = false;
        }
    }

    public Habilidad ObtenerHabilidadAnterior(Habilidad habilidadActual)
    {
        string nombre = habilidadActual.name.Replace("(Clone)", "").Trim();

        // Si termina en +n
        Match match = Regex.Match(nombre, @"^(.*)\s\+(\d+)$");

        if (match.Success)
        {
            string baseName = match.Groups[1].Value.Trim();
            int nivel = int.Parse(match.Groups[2].Value);

            string nombreAnterior = nivel == 1 ? baseName : $"{baseName} +{nivel - 1}";

            return TodasHabilidades.FirstOrDefault(h => h.name == nombreAnterior);
        }

        // Si no tiene mejora, no hay anterior
        return null;
    }
}
