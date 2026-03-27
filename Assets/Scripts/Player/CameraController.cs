using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Rotation")]
    [SerializeField] private float mouseSensitivity = 2f;

    [SerializeField] private Transform _cameraTransform;
    
    private Vector2 _cameraRotation = Vector2.zero;

    private void Update() { RotateCamera(); }

    private void RotateCamera()
    {
        if (_cameraTransform is null)
        {
            Debug.LogWarning("_cameraTransform not attached!");
        }
        var horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        var verticalRotation = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _cameraRotation += new Vector2(horizontalRotation, -verticalRotation);
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, -90f, 90f);

        transform.eulerAngles = new Vector3(0, _cameraRotation.x, 0);
        _cameraTransform.eulerAngles = new Vector3(_cameraRotation.y, _cameraRotation.x, 0);
    }

}
