using UnityEngine;

public class Area : MonoBehaviour
{
    public bool collide = false;

    private void OnTriggerEnter(Collider other)
    {
        collide = true;
    }

    private void OnTriggerExit(Collider other)
    {
        collide = false;
    }
}
