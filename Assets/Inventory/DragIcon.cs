using UnityEngine;
using UnityEngine.UI;

public class DragIcon : MonoBehaviour
{
    public static DragIcon Instance { get; private set; }

    [SerializeField] private Image image;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void Show(Sprite sprite)
    {
        image.sprite = sprite;
        image.enabled = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}