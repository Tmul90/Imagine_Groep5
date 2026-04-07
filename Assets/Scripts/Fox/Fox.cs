using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Fox : MonoBehaviour
{
    [SerializeField] private GameObject foxObject;                  // Object to move
    [SerializeField] private Transform pointB;                      // Target position
    [SerializeField] private float moveSpeed = 5f;                  // Speed of movement

    private Vector3 _pointA;                                         // Start position
    private bool _playerInside;


    private void Start()
    {
        if (foxObject is null) return;

        _pointA = transform.position;
        foxObject.transform.position = _pointA;
        foxObject.SetActive(false);
        
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (foxObject is null) return;
        
        var target = _playerInside ? pointB.position : _pointA;
        foxObject.transform.position = Vector3.MoveTowards(foxObject.transform.position, target, moveSpeed * Time.deltaTime);
        
        if (!_playerInside && foxObject.transform.position == _pointA && foxObject.activeSelf)
        {
            foxObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() is null) return;
        
        foxObject.SetActive(true);
        _playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() is not null)
        {
            _playerInside = false;
        }
    }
}
