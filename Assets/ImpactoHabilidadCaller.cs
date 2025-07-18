using UnityEngine;

public class ImpactoHabilidadCaller : MonoBehaviour
{
    public void EmitirImpacto()
    {
        GameManager.EmitirImpactoHabilidad();
    }

    public void EmitirImpactoConVentana(int frames)
    {
        GameManager.EmitirVentanaImpacto(frames);
        GameManager.EmitirImpactoHabilidad();
    }
}