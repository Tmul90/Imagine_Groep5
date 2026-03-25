using UnityEngine;

public class OasisCollisionChecker : MonoBehaviour
{
    public bool collide = false;
    public GameObject otherObject;
    
    private void OnTriggerEnter(Collider other)
    {
        collide = true;
        otherObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        collide = false;
    }
}
