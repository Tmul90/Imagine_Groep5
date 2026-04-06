using TMPro;
using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    private float _time = 0f;
    private bool _timing = false;
    private bool _enabled = false;

    private TextMeshProUGUI _timerText;

    private void Awake()
    {
        _timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // Start
        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.D)) {_timing = true; }
        // Reset
        if (Input.GetKeyDown(KeyCode.R)) { Reset(); }
        // Enable
        if (Input.GetKeyDown(KeyCode.Alpha0)){_enabled = !_enabled; }
        _timerText.enabled = _enabled;

        if (!_timing) {return;}
        _time += Time.deltaTime;
        
        SetText();
    }

    private void SetText()
    {
        var minutes = Mathf.Floor(_time / 60);
        var seconds = Mathf.Floor(_time % 60);
        var milliseconds = Mathf.Floor(_time * 1000) % 1000;
        _timerText.text = minutes + ":" + seconds + ":" + milliseconds;
    }

    private void Reset()
    {
        _time = 0f;
        _timing = false;
        SetText();
    }
    
    public void StopTimer()
    {
        _timing = false;
    }
}
