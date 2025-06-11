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

        GetComponentInChildren<BarraDeVida>().barraAncho = GetComponent<BoxCollider2D>().size.x;

        GameManager.instance.animationManager.RegisterCharacter(gameObject.GetInstanceID().ToString(), gameObject.AddComponent<CharacterAnimator>().GetComponent<CharacterAnimator>());
    }
}
