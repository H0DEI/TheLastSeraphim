using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera ataqueCam;

    public void ActivarCamara()
    {
        ataqueCam.Priority = 20;
    }

    public void DesactivarCamara()
    {
        ataqueCam.Priority = 0;
    }
}
