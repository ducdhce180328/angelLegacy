using UnityEngine;

public class PlayerSpeechBubble : MonoBehaviour
{
    private SpeechBubbleUI speechBubbleUI;

    void Start()
    {
        speechBubbleUI = FindFirstObjectByType<SpeechBubbleUI>();
    }

    public void ShowBubble(string message, float duration = 2f)
    {
        if (speechBubbleUI == null)
            speechBubbleUI = FindFirstObjectByType<SpeechBubbleUI>();

        if (speechBubbleUI != null)
        {
            speechBubbleUI.ShowBubble(message, duration);
        }
    }
}