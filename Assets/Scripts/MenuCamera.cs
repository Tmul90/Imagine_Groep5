using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MenuCamera : MonoBehaviour
{
    [SerializeField] private GameObject pointsParent;
    [SerializeField] private float switchSpeed = 6f;
    [SerializeField] private float smoothing = 20f;

    private GameObject _cam;
    private Camera _camera;

    private float _timer = 0f;
    private Vector3 _targetPosition;
    private Vector3 _targetRotation;
    private float _targetFov;
    private int _prevPosNum = -1;

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
        cam.position = Vector3.Lerp(cam.position, _targetPosition, Time.deltaTime * smoothing);
        cam.eulerAngles = Vector3.Lerp(cam.eulerAngles, _targetRotation, Time.deltaTime * smoothing);
        _camera.fieldOfView =  Mathf.Lerp(_camera.fieldOfView, _targetFov, Time.deltaTime * smoothing);
        
        
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
        print(posNum);
        print("childCount: " + pointsParent.transform.childCount.ToString());

        Transform point = pointsParent.transform.GetChild(posNum);
        _targetPosition = point.position;
        _targetRotation = point.rotation.eulerAngles;
        _targetFov = point.localScale.x;
        
        if (setPos)
        {
            _cam.transform.position = _targetPosition;
            _cam.transform.rotation = Quaternion.Euler(_targetRotation);
            _camera.fieldOfView =  _targetFov;
        }
    }
}
