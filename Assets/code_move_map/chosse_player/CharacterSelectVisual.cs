using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelectVisual : MonoBehaviour
{
    [Header("Character Images")]
    public Image knightImage;
    public Image mageImage;
    public Image rogueImage;

    [Header("Blink Settings")]
    public float blinkDuration = 0.15f;
    public int blinkCount = 4;

    private Coroutine blinkCoroutine;

    public void HighlightKnight()
    {
        StartHighlight(knightImage);
    }

    public void HighlightMage()
    {
        StartHighlight(mageImage);
    }

    public void HighlightRogue()
    {
        StartHighlight(rogueImage);
    }

    private void StartHighlight(Image selectedImage)
    {
        ResetAllImages();

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        blinkCoroutine = StartCoroutine(BlinkEffect(selectedImage));
    }

    private IEnumerator BlinkEffect(Image selectedImage)
    {
        Color normalColor = selectedImage.color;
        Color dimColor = normalColor;
        dimColor.a = 0.3f;

        for (int i = 0; i < blinkCount; i++)
        {
            selectedImage.color = dimColor;
            yield return new WaitForSeconds(blinkDuration);

            selectedImage.color = normalColor;
            yield return new WaitForSeconds(blinkDuration);
        }

        // Sau khi nhấp nháy xong thì giữ sáng
        selectedImage.color = normalColor;
    }

    private void ResetAllImages()
    {
        SetImageAlpha(knightImage, 0.5f);
        SetImageAlpha(mageImage, 0.5f);
        SetImageAlpha(rogueImage, 0.5f);
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void SetDefaultSelected(Image selectedImage)
    {
        ResetAllImages();
        SetImageAlpha(selectedImage, 1f);
    }
}