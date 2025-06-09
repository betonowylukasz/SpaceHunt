using UnityEngine;
using TMPro;

public class ScrollingEndText : MonoBehaviour
{
    [Header("Ustawienia przewijania")]
    public float scrollSpeed = 50f;

    [Header("Obiekt do aktywacji po zakoñczeniu scrolla")]
    public GameObject objectToActivate;

    private RectTransform textRect;
    private RectTransform canvasRect;

    private bool isFinished = false;

    void Start()
    {
        textRect = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Debug.Log($"Text Rect: {textRect.rect}, {textRect.anchoredPosition}, Canvas Rect: {canvasRect.rect}");
    }

    void Update()
    {
        if (isFinished) return;

        textRect.anchoredPosition += scrollSpeed * Time.deltaTime * Vector2.up;

        if(textRect.anchoredPosition.y > textRect.rect.height)
        {
            isFinished = true;
            Debug.Log("Scrolling finished, activating object.");
            objectToActivate.SetActive(true);
        }
    }
}