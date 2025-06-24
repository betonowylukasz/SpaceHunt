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
    public bool isFadedOut => fadeImage.color.a > 0.0f;

    void Awake()
    {
        if(fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
            Debug.LogError("Fade image is not assigned in the inspector.");
            return;
        }
    }

    public void SetFaded(bool faded)
    {
        if (faded)
        {
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }
        else
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
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
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure the final alpha value is set correctly
        color.a = endAlpha;
        fadeImage.color = color;
        isFading = false;
    }
}
