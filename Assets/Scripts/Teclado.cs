using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teclado : MonoBehaviour
{
    public GameObject habilidad1;
    public GameObject habilidad2;
    public GameObject habilidad3;
    public GameObject habilidad4;
    public GameObject botonListo;
   // public GameObject habilidad5;
   // public GameObject habilidad6;

    private InteractuarBotonHabilidad hab1;
    private InteractuarBotonHabilidad hab2;
    private InteractuarBotonHabilidad hab3;
    private InteractuarBotonHabilidad hab4;
    private InteractuarBotonListo btnListo;
    //private InteractuarBotonHabilidad hab5;
    //private InteractuarBotonHabilidad hab6;

    private void Start()
    {
        hab1 = habilidad1.GetComponent<InteractuarBotonHabilidad>();
        hab2 = habilidad2.GetComponent<InteractuarBotonHabilidad>();
        hab3 = habilidad3.GetComponent<InteractuarBotonHabilidad>();
        hab4 = habilidad4.GetComponent<InteractuarBotonHabilidad>();
        btnListo = botonListo.GetComponent<InteractuarBotonListo>();
       // hab5 = habilidad5.GetComponent<InteractuarBotonHabilidad>();
       // hab6 = habilidad6.GetComponent<InteractuarBotonHabilidad>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) hab1.PulsaHabilidad();
        if (Input.GetKeyDown(KeyCode.Alpha2)) hab2.PulsaHabilidad();
        if (Input.GetKeyDown(KeyCode.Alpha3)) hab3.PulsaHabilidad();
        if (Input.GetKeyDown(KeyCode.Alpha4)) hab4.PulsaHabilidad();
        if (Input.GetKeyDown(KeyCode.Space)) btnListo.PulsaListo();
       // if (Input.GetKeyDown(KeyCode.W)) hab5.PulsaHabilidad();
       // if (Input.GetKeyDown(KeyCode.E)) hab6.PulsaHabilidad();
    }
}
