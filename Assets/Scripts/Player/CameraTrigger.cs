using UnityEngine;
using Unity.Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    public CinemachineCamera targetCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetCamera.Priority = 20;
        }
    }
}