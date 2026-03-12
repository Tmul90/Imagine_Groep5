using UnityEngine;

public class GreenZone : MonoBehaviour
{
    [SerializeField]  Transform spawnPoint;
    private Area area;
    
    private void Awake()
    {
        area = GetComponent<Area>();
    }

    private void Update()
    {
        if (area.collide && area.otherObject != null)
        {
            PlayerController player = area.otherObject.GetComponent<PlayerController>();
            if(player != null){}
                player.spawnPoint = spawnPoint.position;
        }
    }
}
