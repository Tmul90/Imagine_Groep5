using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerStimulationVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] peoplePrefabs;
    [SerializeField] private float minSpawnRadius = 1f;
    [SerializeField] private float maxSpawnRadius = 3f;
    [SerializeField] private int maxPeople = 5;

    [SerializeField] private GameObject featherPrefab;
    [SerializeField] private float featherMinRadius = 0.5f;
    [SerializeField] private float featherMaxRadius = 3f;
    [SerializeField] private float featherFallSpeed = 1f;
    [SerializeField] private float featherLifetime = 3f;
    [SerializeField] private int maxActiveFeathers = 20;
    [SerializeField] private float featherSpawnInterval = 0.5f;

    private Transform _player;
    private GameObject[] _spawnedPeople;
    private readonly List<GameObject> _activeFeathers = new();
    private float _featherTimer = 0f;
    private float _currentStimulationPercent = 0f;

    private void Awake()
    {
        _player = transform;
        _spawnedPeople = new GameObject[maxPeople];
        StimulationManager.OnStimulationChanged += UpdateStimulation;
    }

    private void OnDestroy()
    {
        StimulationManager.OnStimulationChanged -= UpdateStimulation;
    }

    private void UpdateStimulation(float stimulationPercentage)
    {
        _currentStimulationPercent = stimulationPercentage;
        UpdatePeople(stimulationPercentage);
    }

    private void Update()
    {
        _featherTimer += Time.deltaTime;

        if (featherPrefab && _featherTimer >= featherSpawnInterval)
        {
            _featherTimer = 0f;
            var featherCount = Mathf.RoundToInt((_currentStimulationPercent / 100f) * 2);

            for (var i = 0; i < featherCount && _activeFeathers.Count < maxActiveFeathers; i++)
            {
                var angle = Random.Range(0f, Mathf.PI * 2f);
                var distance = Random.Range(featherMinRadius, featherMaxRadius);
                var spawnPos = _player.position + new Vector3(Mathf.Cos(angle), 1, Mathf.Sin(angle)) * distance;

                var feather = Instantiate(featherPrefab, spawnPos, Quaternion.identity);
                feather.transform.LookAt(_player);
                feather.AddComponent<FeatherFallFade>().Initialize(featherFallSpeed, featherLifetime);

                _activeFeathers.Add(feather);
            }
        }

        _activeFeathers.RemoveAll(f => !f);
    }

    private void UpdatePeople(float stimulationPercentage)
    {
        var peopleCount = Mathf.RoundToInt((stimulationPercentage / 100f) * maxPeople);

        for (var i = 0; i < maxPeople; i++)
        {
            if (i < peopleCount)
            {
                if (!_spawnedPeople[i])
                {
                    var prefab = peoplePrefabs[Random.Range(0, peoplePrefabs.Length)];
                    var angle = Random.Range(0f, Mathf.PI * 2f);
                    var distance = Random.Range(minSpawnRadius, maxSpawnRadius);
                    var spawnPos = _player.position + new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * distance;

                    _spawnedPeople[i] = Instantiate(prefab, spawnPos, Quaternion.identity);
                    _spawnedPeople[i].transform.localScale = Vector3.one * 0.2f;
                    _spawnedPeople[i].transform.LookAt(new Vector3(_player.position.x, _spawnedPeople[i].transform.position.y, _player.position.z));

                    var follow = _spawnedPeople[i].AddComponent<FollowPlayer>();
                    follow.player = _player;
                }
                else
                {
                    _spawnedPeople[i].transform.LookAt(new Vector3(_player.position.x, _spawnedPeople[i].transform.position.y, _player.position.z));
                }
            }
            else
            {
                if (!_spawnedPeople[i]) continue;
                Destroy(_spawnedPeople[i]);
                _spawnedPeople[i] = null;
            }
        }
    }
}

public class FeatherFallFade : MonoBehaviour
{
    private float _fallSpeed;
    private float _lifetime;

    private float _elapsed = 0f;
    private SpriteRenderer _spriteRenderer;

    public void Initialize(float speed, float life)
    {
        _fallSpeed = speed;
        _lifetime = life;
        _spriteRenderer = GetComponent<SpriteRenderer>() ?? gameObject.AddComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var delta = Time.deltaTime;
        _elapsed += delta;

        transform.position += Vector3.down * _fallSpeed * delta;

        if (!_spriteRenderer)
        {
            var halfLife = _lifetime / 2f;
            var alpha = _elapsed < halfLife ? Mathf.Lerp(0f, 1f, _elapsed / halfLife) : Mathf.Lerp(1f, 0f, (_elapsed - halfLife) / halfLife);
            var c = _spriteRenderer.color;
            c.a = alpha;
            _spriteRenderer.color = c;
        }

        if (_elapsed >= _lifetime)
        {
            Destroy(gameObject);
        }
    }
}