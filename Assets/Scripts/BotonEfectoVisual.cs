using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class BotonEfectoVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Objetos")]
    public RectTransform objetivo; // Escalar este objeto
    public Image fondo; // Cambiar color si quieres

    [Header("Tamaño")]
    public Vector3 escalaNormal = Vector3.one;
    public Vector3 escalaHover = new Vector3(1.05f, 1.05f, 1f);
    public Vector3 escalaClick = new Vector3(0.95f, 0.95f, 1f);

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorHover = Color.cyan;
    public Color colorClick = Color.gray;

    [Header("Velocidad")]
    public float velocidad = 10f;

    Vector3 escalaObjetivo;
    Color colorObjetivo;
    Coroutine animacion;

    void Awake()
    {
        if (objetivo == null) objetivo = GetComponent<RectTransform>();
        if (fondo == null) fondo = GetComponent<Image>();

        escalaObjetivo = escalaNormal;
        colorObjetivo = colorNormal;
    }

    void Update()
    {
        objetivo.localScale = Vector3.Lerp(objetivo.localScale, escalaObjetivo, Time.deltaTime * velocidad);
        fondo.color = Color.Lerp(fondo.color, colorObjetivo, Time.deltaTime * velocidad);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaObjetivo = escalaHover;
        colorObjetivo = colorHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaObjetivo = escalaNormal;
        colorObjetivo = colorNormal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        escalaObjetivo = escalaClick;
        colorObjetivo = colorClick;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        escalaObjetivo = escalaHover;
        colorObjetivo = colorHover;
    }
}
