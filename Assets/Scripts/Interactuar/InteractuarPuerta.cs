using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider), typeof(Outline))]
public class InteractuarPuerta : MonoBehaviour
{
    [Header("Escena a cargar")]
    public Escena escenaCargar;

    [Header("Interacción con ratón")]
    public bool puedePresionarse;

    [Header("Navegación con flechas ← / →")]
    [Tooltip("False = ←  carga la escena\nTrue  = →  carga la escena")]
    public bool siguienteEscena = false;          // ← por defecto

    // ────────────────────────────────────────────────────────────────────
    Renderer rend;
    Color porDefecto;
    Color blanco;
    Outline outline;

    // ------------------------------------------------------------------
    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = true;          // se enciende para precargar material
    }

    void Start()
    {
        rend = GetComponent<Renderer>();

        ColorUtility.TryParseHtmlString("#6C6A6A", out porDefecto);
        ColorUtility.TryParseHtmlString("#FFFFFF", out blanco);

        outline.enabled = false;         // se apaga hasta que el ratón entre
    }

    // --------------------------- RATÓN --------------------------------
    void OnMouseEnter()
    {
        if (puedePresionarse)
        {
            rend.material.SetColor("_Color", blanco);
            outline.enabled = true;
        }
    }
    void OnMouseExit()
    {
        rend.material.SetColor("_Color", porDefecto);
        outline.enabled = false;
    }
    void OnMouseDown()
    {
        if (puedePresionarse)
            GameManager.instance.CompruebaYCargaEscenas(escenaCargar);
    }

    // --------------------------- TECLADO ------------------------------
    //  Ignora 'puedePresionarse': siempre lanza la transición cuando
    //  la tecla coincide con el valor de 'siguienteEscena'.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !siguienteEscena)
        {
            GameManager.instance.CompruebaYCargaEscenas(escenaCargar);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && siguienteEscena)
        {
            GameManager.instance.CompruebaYCargaEscenas(escenaCargar);
        }
    }
}
