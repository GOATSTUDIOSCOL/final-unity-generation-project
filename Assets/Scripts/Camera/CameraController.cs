using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class CameraController : NetworkBehaviour
{
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    public CinemachineVirtualCamera cvc;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            cvc.Priority = -1;
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y + mouseX, 0f);
    }
}

