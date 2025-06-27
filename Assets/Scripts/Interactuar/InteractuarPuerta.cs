using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider), typeof(Outline))]
public class InteractuarPuerta : MonoBehaviour
{
    public Escena escenaCargar;

    public bool puedePresionarse;

    private Renderer rend;

    private Color porDefecto;
    private Color blanco;

    private Outline componenteOutline;

    private void Awake()
    {
        componenteOutline = GetComponent<Outline>();

        componenteOutline.enabled = true;
    }

    private void Start()
    {
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString("#6C6A6A", out porDefecto);
        ColorUtility.TryParseHtmlString("#FFFFFF", out blanco);

        componenteOutline.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (puedePresionarse) rend.material.SetColor("_Color", blanco);
    }

    private void OnMouseExit()
    {
        rend.material.SetColor("_Color", porDefecto);
    }
    private void OnMouseDown()
    {
        if(puedePresionarse) GameManager.instance.CompruebaYCargaEscenas(escenaCargar);
    }
}
