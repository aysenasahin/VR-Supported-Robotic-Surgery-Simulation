using UnityEngine;

public class XRPhysicalMovementMultiplier : MonoBehaviour
{
    public Transform hmdCamera;
    public float movementMultiplier = 2.0f;

    private Vector3 lastLocalHmdPosition;
    private CharacterController characterController;

    void Start()
    {
        if (hmdCamera == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
                hmdCamera = cam.transform;
        }

        characterController = GetComponent<CharacterController>();

        if (hmdCamera != null)
            lastLocalHmdPosition = hmdCamera.localPosition;
    }

    void LateUpdate()
    {
        if (hmdCamera == null) return;

        Vector3 currentLocalHmdPosition = hmdCamera.localPosition;

        Vector3 localDelta = currentLocalHmdPosition - lastLocalHmdPosition;

        localDelta.y = 0f;

        Vector3 extraLocalMovement = localDelta * (movementMultiplier - 1f);
        Vector3 extraWorldMovement = transform.TransformVector(extraLocalMovement);

        if (characterController != null)
        {
            characterController.Move(extraWorldMovement);
        }
        else
        {
            transform.position += extraWorldMovement;
        }

        lastLocalHmdPosition = currentLocalHmdPosition;
    }
}