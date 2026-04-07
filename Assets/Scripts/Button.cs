using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public bool selected = false;

    [SerializeField] private Sprite selectSprite;
    [SerializeField] private Sprite deselctSprite;
    [SerializeField] private Vector2 characterSpacing = new Vector2(11f, 40f);
    
    private Image _image;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        _image.sprite = selected ? selectSprite : deselctSprite;
        _text.characterSpacing = Mathf.Lerp(_text.characterSpacing, selected ? characterSpacing.y : characterSpacing.x, 3 * Time.deltaTime);
    }

    public void Select()
    {
        selected = true;
        Debug.Log("mouseEnter");
    }

    public void DeSelect()
    {
        selected = false;
        Debug.Log("mouseExit");
    }
}
