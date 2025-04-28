using UnityEngine;

public class PunchMover : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Transform target;
    private bool isMoving = false;
    private bool isReturning = false;

    [Header("Configuración de movimiento")]
    public float speed = 5f;
    public float distanceStop = 0.5f;
    public float rotationSpeed = 10f;

    private Vector3 initialDirection;
    private Quaternion initialLookRotation;

    public Transform rootTransform;

    public Transform objetivo;

    public void StartPunchMovement()
    {
        if (!isMoving && objetivo != null)
        {
            target = objetivo;
            originalPosition = rootTransform.position;
            originalRotation = rootTransform.rotation;
            isMoving = true;

            // ✅ Ejecutar el primer paso de movimiento inmediatamente
            ForceFirstMovement();
        }
    }

    private void ForceFirstMovement()
    {
        if (target == null) return;

        Vector3 direction = (target.position - rootTransform.position).normalized;
        initialDirection = direction;
        initialLookRotation = Quaternion.LookRotation(direction);

        // Avanzamos un pequeño paso inmediatamente
        rootTransform.position += direction * speed * Time.deltaTime;
        rootTransform.rotation = Quaternion.Slerp(rootTransform.rotation, initialLookRotation, rotationSpeed * Time.deltaTime);
    }

    public void ReturnToOriginalPosition()
    {
        if (!isReturning)
        {
            isMoving = false;
            isReturning = true;
        }
    }

    private void LateUpdate()
    {
        if (isMoving && target != null)
        {
            MoveTowards();
        }
        else if (isReturning)
        {
            ReturnToStart();
        }
    }

    private void MoveTowards()
    {
        Vector3 direction = (target.position - rootTransform.position).normalized;
        float distance = Vector3.Distance(rootTransform.position, target.position);

        if (distance > distanceStop)
        {
            rootTransform.position += direction * speed * Time.deltaTime;
            rootTransform.rotation = Quaternion.Slerp(rootTransform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }
    }

    private void ReturnToStart()
    {
        rootTransform.position = Vector3.MoveTowards(rootTransform.position, originalPosition, speed * 1.5f * Time.deltaTime);
        rootTransform.rotation = Quaternion.Slerp(rootTransform.rotation, originalRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(rootTransform.position, originalPosition) <= 0.05f)
        {
            isReturning = false;
            rootTransform.position = originalPosition;
            rootTransform.rotation = originalRotation;
        }
    }
}