using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    // Settings
    [SerializeField] private float flickerStrength = 0.5f;
    [SerializeField] private float flickerDuration = 0.1f;
    
    // Reference
    private Light _light;
    
    // Variables
    private float _strength;
    private float _flickerTimer = 0f;
    private float _currentStrength;
    
    private void Awake()
    {
        _light = GetComponent<Light>();
        _strength = _light.intensity;
    }

    private void Update()
    {
        _flickerTimer += Time.deltaTime;
        
        if (_flickerTimer >= flickerDuration)
        {
            _currentStrength = _strength * (1f + Random.Range(-flickerStrength, flickerStrength));
            _flickerTimer = 0f;
        }
        
        _light.intensity = Mathf.Lerp(_currentStrength, _light.intensity, 0.9f);
    }
}
