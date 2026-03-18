using UnityEngine;

public class Area : MonoBehaviour
{
    public bool collide = false;
    public GameObject otherObject;
    
    private void OnTriggerEnter(Collider other)
    {
        collide = true;
        otherObject = other.gameObject;
        /*
        if(otherObject != null)
            print(otherObject);
        */
    }

    private void OnTriggerExit(Collider other)
    {
        collide = false;
    }
}
