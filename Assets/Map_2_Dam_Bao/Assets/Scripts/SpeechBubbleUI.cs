using UnityEngine;
using TMPro;
using System.Collections;

public class SpeechBubbleUI : MonoBehaviour
{
    public GameObject bubbleVisual;
    public TMP_Text bubbleText;

    private Coroutine currentRoutine;

    void Start()
    {
        if (bubbleVisual != null)
            bubbleVisual.SetActive(false);
    }

    public void ShowBubble(string message, float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowBubbleRoutine(message, duration));
    }

    IEnumerator ShowBubbleRoutine(string message, float duration)
    {
        if (bubbleVisual != null)
            bubbleVisual.SetActive(true);

        if (bubbleText != null)
            bubbleText.text = message;

        yield return new WaitForSeconds(duration);

        if (bubbleVisual != null)
            bubbleVisual.SetActive(false);
    }
}