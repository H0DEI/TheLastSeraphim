using UnityEngine;

public class SoyAliado : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameManager.instance != null &&
            GameManager.instance.listaObjetosPersonajesEscena != null &&
            !GameManager.instance.listaObjetosPersonajesEscena.Contains(gameObject))
        {
            GameManager.instance.listaObjetosPersonajesEscena.Add(gameObject);
        }
        GetComponent<LookAtWithMargin>().GetClosestLookAtTarget(transform);
    }
}
