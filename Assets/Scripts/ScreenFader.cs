using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private float fadeDuration = 1.0f;

    public bool isFading { get; private set; }

    void Awake()
    {
        if(fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
            Debug.LogError("Fade image is not assigned in the inspector.");
            return;
        }
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(0, 1);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(1, 0);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        isFading = true;
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while(elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha value is set correctly
        color.a = endAlpha;
        fadeImage.color = color;
        isFading = false;
    }
}
