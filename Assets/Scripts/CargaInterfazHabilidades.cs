using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CargaInterfazHabilidades : MonoBehaviour
{
    private TextMeshProUGUI[] habilidades;

    private void Start()
    {
        ActualizaInterfazHabilidades();
    }

    public void ActualizaInterfazHabilidades()
    {
        GameManager.instance.cargaInterfazHabilidades = this;

        habilidades = GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < GameManager.instance.jugador.habilidades.Count; i++)
        {
            habilidades[i].text = GameManager.instance.jugador.habilidades[i].nombre;
        }
    }
}
