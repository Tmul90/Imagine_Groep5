using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [HideInInspector] internal Transform player;
    private Vector3 _offset;

    private void Start() =>
        _offset = transform.position - player.position;

    private void Update() =>
        transform.position = player.position + _offset;
}
