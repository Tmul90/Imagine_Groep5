using UnityEngine;
using UnityEngine.UI;
using Util;

[RequireComponent(typeof(Image))]
public class SpriteAnimation : Singleton<SpriteAnimation>
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float animationFPS = 24;

    private Image _image;
    
    private bool _playing = true;
    private int _direction = 1;
    private int _currentSprite = 0;
    private float _timer = 0f;

    private void Awake()
    {
        _image = GetComponent<Image>();
        if(_image is null) {Debug.LogWarning("_image not attached!");}
    }

    private void Update()
    {
        if (_playing)
        {
            _timer += Time.deltaTime * _direction;
            _timer = Mathf.Clamp(_timer, 0f, (sprites.Length - 1) / animationFPS);
            _currentSprite = (int)Mathf.Floor(_timer * animationFPS);
            _currentSprite = Mathf.Clamp(_currentSprite, 0, sprites.Length - 1);

            _image.sprite = sprites[_currentSprite];
        }
        else
        {
            _timer = 0f;
        }
        
        _image.sprite = sprites[_currentSprite];
    }


    public void PlayAnimation(int direction)
    {
        _playing = true;
        _direction = direction;
        
        if (direction != 1 && direction != -1)
        {
            Debug.LogWarning("Direction should be either 1 or -1");
        }
    }
}
