using Unity.VisualScripting;
using UnityEngine;

public class OldFloatingTextManager : MonoBehaviour
{
    public GameObject textPrefab; // Prefab del texto flotante

    public void ShowFloatingText(GameObject target, string message, Color textColor)
    {
        target = target.GetComponentInChildren<Canvas>().GameObject();

        // Crear una instancia del prefab
        GameObject floatingText = Instantiate(textPrefab, target.transform.position, Quaternion.identity);

        // Ajustar la posición del texto al canvas del personaje
        floatingText.transform.SetParent(target.transform, false);
        floatingText.transform.localPosition = new Vector3(Random.Range(-0.2f, 0.4f), Random.Range(-0.7f, -0.15f), 0); // Ajusta la posición en Y

        // Inicializar el texto flotante
        FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
        //floatingTextScript.Initialize(message, textColor);
    }
}