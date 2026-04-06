using UnityEngine;

public class StopTimerOnEnter : MonoBehaviour
{
    [SerializeField] private SpeedrunTimer speedrunTimer;
    private OasisCollisionChecker collisionChecker;

    private void Awake()
    {
        collisionChecker = GetComponent<OasisCollisionChecker>();
    }

    private void Update()
    {
        if (!collisionChecker.collide) {return;}
        speedrunTimer.StopTimer();
    }
}
