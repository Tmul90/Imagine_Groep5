using UnityEngine;

public class PlayerStimulationVisual : MonoBehaviour
{
    [Header("People Settings")]
    public GameObject[] peoplePrefabs;  // 3 people prefabs
    public float spawnRadius = 3f;      // Radius around player
    public int maxPeople = 5;           // Maximum number of people

    private Transform _player;
    private GameObject[] _spawnedPeople;

    private void Awake()
    {
        _player = transform;
        _spawnedPeople = new GameObject[maxPeople];

        StimulationManager.OnStimulationChanged += UpdatePeople;
    }

    private void OnDestroy()
    {
        StimulationManager.OnStimulationChanged -= UpdatePeople;
    }

    private void UpdatePeople(float stimulationPercentage)
    {
        var peopleCount = Mathf.RoundToInt((stimulationPercentage / 100f) * maxPeople);
        
        for (int i = 0; i < maxPeople; i++)
        {
            if (i < peopleCount)
            {
                if (_spawnedPeople[i] == null)
                {
                    var prefab = peoplePrefabs[Random.Range(0, peoplePrefabs.Length)];
                    
                    var angle = i * Mathf.PI * 2 / peopleCount;
                    var spawnPos = _player.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;

                    _spawnedPeople[i] = Instantiate(prefab, spawnPos, Quaternion.identity);
                    
                    _spawnedPeople[i].transform.LookAt(new Vector3(_player.position.x, _spawnedPeople[i].transform.position.y, _player.position.z));
                    
                    var follow = _spawnedPeople[i].AddComponent<FollowPlayer>();
                    follow.player = _player;
                }
                
                //var scale = Mathf.Lerp(minScale, maxScale, stimulationPercentage / 100f);
                _spawnedPeople[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                
                _spawnedPeople[i].transform.LookAt(new Vector3(_player.position.x, _spawnedPeople[i].transform.position.y, _player.position.z));
            }
            else
            {
                if (_spawnedPeople[i] != null)
                {
                    Destroy(_spawnedPeople[i]);
                    _spawnedPeople[i] = null;
                }
            }
        }
    }
}
