using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Fox : MonoBehaviour
{
    [SerializeField] private GameObject foxRoot;
    [SerializeField] private Transform headRoot;
    [SerializeField] private float removeDelay = 4f;
    
    private Quaternion _headRestLocalRotation;
    private GameObject _currentFox;

    private void Start()
    {
        var box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        if (headRoot)
            _headRestLocalRotation = headRoot.localRotation;

        SetFoxActive(false);
    }

    private void Update()
    {
        RotateTowardsPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;

        StartCoroutine(DelayRemoval(removeDelay));
    }

    private IEnumerator DelayRemoval(float delay)
    {
        SetFoxActive(true);
        yield return new WaitForSeconds(delay);
        SetFoxActive(false);
    }
    
    private void SetFoxActive(bool active) => foxRoot.SetActive(active);
    
    private void RotateTowardsPlayer()
    {
        if (!headRoot || !PlayerController.Instance) return;

        var playerPos = PlayerController.Instance.transform.position;
        var headPos = headRoot.position;
        var direction = playerPos - headPos;
        if (direction.sqrMagnitude < 0.001f) return;

        var localDirection = headRoot.parent.InverseTransformDirection(direction);
        
        var targetRotation = Quaternion.LookRotation(localDirection, Vector3.up);

        var euler = (targetRotation * Quaternion.Inverse(_headRestLocalRotation)).eulerAngles;
        euler.x = Mathf.Clamp(euler.x > 180 ? euler.x - 360 : euler.x, -45f, 45f);
        euler.y = Mathf.Clamp(euler.y > 180 ? euler.y - 360 : euler.y, -60f, 60f);
        euler.z = 0f;

        targetRotation = _headRestLocalRotation * Quaternion.Euler(euler);

        headRoot.localRotation = Quaternion.Slerp(headRoot.localRotation, targetRotation, Time.deltaTime * 5f);
    }
}