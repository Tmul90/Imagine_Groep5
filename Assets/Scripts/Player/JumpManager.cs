using System;
using Unity.VisualScripting;
using UnityEngine;

public class JumpManager : MonoBehaviour
{
    [SerializeField] private Vector3 boxSize = new(0.3f, 0.1f, 0.3f);
    internal LayerMask groundLayer { get; set; }
    
    internal bool canCollide { get; private set; }
    internal GameObject otherObject { get; private set; }

    private Vector3 _feetPosition;
    
    private void FixedUpdate()
    {
        _feetPosition = new Vector3(
            transform.position.x,
            PlayerController.Instance.GetHeight(),
            transform.position.z
        );
        
        //Debug.Log(PlayerController.Instance.GetHeight());
        var hits = Physics.OverlapBox(
            _feetPosition,
            boxSize * 0.5f,
            transform.rotation,
            groundLayer
        );

        canCollide = hits.Length > 0;
        otherObject = canCollide ? hits[0].gameObject : null;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = canCollide ? Color.green : Color.red;
        Gizmos.matrix = Matrix4x4.TRS(_feetPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
