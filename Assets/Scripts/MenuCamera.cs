using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuCamera : MonoBehaviour
{
    [SerializeField] private GameObject pointsParent;
    [SerializeField] private float switchSpeed = 6f;
    [SerializeField] private float smoothing = 4f;
    [SerializeField] private float addPosSmoothing = 4f;
    [SerializeField] private float addPosAmount = 0.3f;
    [SerializeField] private Image black;

    private GameObject _cam;
    private Camera _camera;

    private float _timer = 0f;
    private Vector3 _targetPosition;
    private Vector3 _targetRotation;
    private float _targetFov;
    private int _prevPosNum = -1;
    private Vector3 _currentPosition = Vector3.zero;
    private Vector3 _currentAddPosition = Vector3.zero;
    private Vector3 _currentRotation = Vector3.zero;
    private Vector3 _currentAddRotation = Vector3.zero;

    private void Awake()
    {
        _cam = transform.GetComponentInChildren<Camera>().gameObject;
        _camera = _cam.GetComponent<Camera>();
        if (_cam is null)
            Debug.LogWarning("No Camera Found");
        Switch(true);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > switchSpeed)
        {
            _timer = 0f;
            Switch();
        }

        Transform cam = _cam.transform;
        SetPosition(cam);
        AddPosition(cam);

        float alpha = _timer > 1f ? (_timer - switchSpeed) + 1f : 1 - _timer;
        alpha = Mathf.Clamp01(alpha);
        
        black.color = new Color(0f, 0f, 0f, alpha);
    }

    private void Switch(bool setPos = false)
    {
        int posNum = -1;
        for (int i = 0; i < 100; i++)
        {
            posNum = Random.Range(0, pointsParent.transform.childCount);
            if (posNum != _prevPosNum)
            {
                _prevPosNum = posNum;
                break;
            }
        }

        Transform point = pointsParent.transform.GetChild(posNum);
        _targetPosition = point.position;
        _currentPosition = point.position;
        _targetRotation = point.rotation.eulerAngles;
        _currentRotation = point.rotation.eulerAngles;
        _targetFov = point.localScale.x;
        
        if (setPos)
        {
            _cam.transform.position = _targetPosition;
            _cam.transform.rotation = Quaternion.Euler(_targetRotation);
            _camera.fieldOfView =  _targetFov;
        }
    }

    private void SetPosition(Transform cam)
    {
        _currentPosition = Vector3.Lerp(_currentPosition, _targetPosition, Time.deltaTime * smoothing);
        _currentRotation = Vector3.Lerp(cam.eulerAngles, _targetRotation, Time.deltaTime * smoothing);
        _currentRotation.z = 0f;
        _camera.fieldOfView =  Mathf.Lerp(_camera.fieldOfView, _targetFov, Time.deltaTime * smoothing);
    }

    private void AddPosition(Transform cam)
    {
        Vector2 resolution = new Vector2(Screen.width, Screen.height);
        Vector2 mousePos = ((Input.mousePosition / resolution) * 2f) - Vector2.one;
        Vector3 targetAddPosition = cam.transform.right * mousePos.x + cam.transform.up * mousePos.y;
        Vector3 targetAddRotation = new Vector3(0f, 0f, mousePos.x);
        _currentAddPosition = Vector3.Lerp(_currentAddPosition, targetAddPosition, Time.deltaTime * addPosSmoothing);
        _currentAddRotation = Vector3.Lerp(_currentAddRotation, targetAddRotation, Time.deltaTime * addPosSmoothing);
        cam.position = _currentPosition + (_currentAddPosition * addPosAmount);
        cam.eulerAngles = _currentRotation + _currentAddRotation;
    }
}
