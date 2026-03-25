using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float posStrength;
    private float rotStrength;
    private float time;
    private float endSmooth;

    private bool shakeWasSet;
    private Vector2 shakePos;
    private float shakeRot;
    
    private void Update()
    {
        time -= Time.deltaTime;
        time = Mathf.Max(time, 0f);

        if (time > 0f || shakeWasSet)
        {
            // Reset, otherwise it'll be additive.
            transform.localPosition -= new Vector3(shakePos.x, shakePos.y, 0f);
            transform.localEulerAngles -= new Vector3(0f, 0f, shakeRot);
            
            // Set shake
            shakePos = new Vector2(Random.Range(-posStrength, posStrength), Random.Range(-posStrength, posStrength));
            shakeRot = Random.Range(-rotStrength, rotStrength);
            transform.localPosition += new Vector3(shakePos.x, shakePos.y, 0f);
            transform.localEulerAngles += new Vector3(0f, 0f, shakeRot);
            shakeWasSet = false;
        }
    }
    
    
    public void StartShake(float positionStrength, float rotationStrength, float duration, float endSmoothness)
    {
        posStrength = positionStrength;
        rotStrength = rotationStrength;
        time = duration;
        endSmooth = endSmoothness;
    }

    public void SetShake(float positionStrength, float rotationStrength)
    {
        posStrength = positionStrength;
        rotStrength = rotationStrength;
        shakeWasSet = true;
    }
}
