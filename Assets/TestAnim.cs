using SHG.AnimatorCoder;
using UnityEngine;

public class TestAnim : MonoBehaviour
{
    private GameManager instancia;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instancia = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            instancia.animationManager.PlayAnimation(instancia.cargaEscena.enemigos.transform.GetChild(0).gameObject.GetInstanceID().ToString(), new(Animations.SHOOT1, true, new(), 0.2f));
            instancia.animationManager.PlayAnimation(instancia.objetoJugador.gameObject.GetInstanceID().ToString(), new(Animations.SHOOT1, true, new(), 0f));
        }
    }
}
