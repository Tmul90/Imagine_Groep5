using UnityEngine;

public class GreenZones : MonoBehaviour
{
    public bool inGreenZone = false;
    void Update()
    {
        inGreenZone = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            Area childArea = transform.GetChild(i).GetComponent<Area>();
            if (childArea.collide)
            {
                inGreenZone = true;
                break;
            }
        }
    }
}
