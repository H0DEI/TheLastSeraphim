using UnityEngine;

public class LookAtWithMargin : MonoBehaviour
{
    [SerializeField] private Transform target; // El objeto al que apuntar
    [SerializeField][Range(0, 1)] private float errorMargin = 0.25f; // Margen de error (25% por defecto)

    private void Start()
    {
        if (!GetComponent<InteractuarPersonajes>().aliado) target = GameManager.instance.objetoJugador.transform;

        if(target != null)
        {
            // Excluir que el target sea el mismo objeto que lleva el script
            if (target == transform)
            {
                return;
            }

            LookAt(target);
        }
    }

    public void LookAt(Transform target)
    {
            // Dirección hacia el objetivo
            Vector3 directionToTarget = target.position - transform.position;

            // Introducir margen de error
            Vector3 randomError = new Vector3(
                Random.Range(-errorMargin, errorMargin),
                0f, // ← No queremos error en el eje Y (vertical)
                Random.Range(-errorMargin, errorMargin)
            );

            // Ajustar dirección con margen de error
            Vector3 adjustedDirection = directionToTarget + randomError;

            // Ignorar componente Y (vertical) para mantener la rotación solo en el plano XZ
            adjustedDirection.y = 0f;

            // Solo girar en el plano horizontal
            if (adjustedDirection != Vector3.zero) // evitar error de rotación nula
            {
                transform.rotation = Quaternion.LookRotation(adjustedDirection);
            }
        
    }

    public void GetClosestLookAtTarget(Transform self)
    {
        LookAtWithMargin[] allLookTargets = GameObject.FindObjectsOfType<LookAtWithMargin>();
        Transform closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (LookAtWithMargin target in allLookTargets)
        {
            Transform targetTransform = target.transform;

            // Ignorar a uno mismo
            if (targetTransform == self)
                continue;

            // Verificar si tiene el componente InteractuarPersonajes
            InteractuarPersonajes ip = targetTransform.GetComponent<InteractuarPersonajes>();
            if (ip != null && ip.aliado)
                continue; // Ignorar si es aliado

            float distance = Vector3.Distance(self.position, targetTransform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = targetTransform;
            }
        }

        LookAt(closest);
    }
}