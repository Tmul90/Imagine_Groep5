using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Fox : MonoBehaviour
{
    [SerializeField] private GameObject foxPrefab;                  // Object to move
    [SerializeField] private Transform pointA;                      // Target position
    [SerializeField] private Transform pointB;                      // Target position
    [SerializeField] private float moveSpeed = 5f;                  // Speed of movement
    [SerializeField] private float triggerCooldown = 10f;
    
    private bool _isMoving = false;
    private GameObject _currentFox;
    private float _lastTriggerTime = -Mathf.Infinity;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Update()
    {
        if (_isMoving && _currentFox != null)
        {
            MoveFox();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (Time.time - _lastTriggerTime < triggerCooldown) return;

            if (_currentFox == null)
            {
                SpawnFox();
                _lastTriggerTime = Time.time;
            }
        }
    }
    
    private void SpawnFox() 
    {
        _currentFox = Instantiate(foxPrefab, pointA.position, Quaternion.identity);
        
        var direction = pointB.position - _currentFox.transform.position;
        if (direction != Vector3.zero)
            _currentFox.transform.rotation = Quaternion.LookRotation(direction);

        _isMoving = true;
    }
    
    private void MoveFox()
    {
        if (_currentFox == null) return;
        
        _currentFox.transform.position = Vector3.MoveTowards(
            _currentFox.transform.position,
            pointB.position,
            moveSpeed * Time.deltaTime
        );
        
        RotateTowardsPointB();
        
        if (Vector3.Distance(_currentFox.transform.position, pointB.position) < 0.01f)
        {
            Destroy(_currentFox);
            _currentFox = null;
            _isMoving = false;
        }
    }
    
    private void RotateTowardsPointB()
    {
        var direction = pointB.position - _currentFox.transform.position;
        if (direction != Vector3.zero)
        {
            var lookRotation = Quaternion.LookRotation(direction);
            
            lookRotation *= Quaternion.Euler(0f, 180f, 0f);

            _currentFox.transform.rotation = lookRotation;
        }
    }
    
}