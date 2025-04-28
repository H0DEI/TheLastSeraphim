using UnityEngine;
using System.Collections;

public class MeleeMovement : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isMoving = false;
    private bool isReturning = false;

    [Header("Configuración de movimiento")]
    public float speed = 5f;
    public float distanceStop = 0.2f;
    public float rotationSpeed = 10f;

    private Transform target;

    public Transform objetivo;

    public Transform rootTransform;

    // Llamado desde Animation Event: empieza a moverse hacia el objetivo
    public void StartPunchMovement()
    {
        if (!isMoving && objetivo != null)
        {
            target = objetivo;
            originalPosition = rootTransform.position;
            originalRotation = rootTransform.rotation;
            StartCoroutine(MoveTowardsTarget());
        }
    }

    // Llamado desde otro Animation Event: empieza a volver
    public void ReturnToOriginalPosition()
    {
        if (!isReturning)
        {
            StartCoroutine(ReturnToStart());
        }
    }

    private IEnumerator MoveTowardsTarget()
    {
        isMoving = true;

        // Mover hacia el objetivo hasta estar cerca
        while (Vector3.Distance(rootTransform.position, target.position) > distanceStop)
        {
            Vector3 direction = (target.position - rootTransform.position).normalized;

            // Movimiento hacia delante
            Vector3 move = direction * speed * Time.deltaTime;
            if (move.magnitude > Vector3.Distance(rootTransform.position, target.position) - distanceStop)
                move = direction * (Vector3.Distance(rootTransform.position, target.position) - distanceStop);

            rootTransform.position += move;

            // Rotación hacia el objetivo
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rootTransform.rotation = Quaternion.Slerp(rootTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        isMoving = false;
    }

    private IEnumerator ReturnToStart()
    {
        isReturning = true;

        float returnSpeed = speed * 1.5f;

        while (Vector3.Distance(rootTransform.position, originalPosition) > 0.05f)
        {
            rootTransform.position = Vector3.MoveTowards(rootTransform.position, originalPosition, returnSpeed * Time.deltaTime);
            rootTransform.rotation = Quaternion.Slerp(rootTransform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Asegurar valores exactos
        rootTransform.position = originalPosition;
        rootTransform.rotation = originalRotation;

        isReturning = false;
    }
}
