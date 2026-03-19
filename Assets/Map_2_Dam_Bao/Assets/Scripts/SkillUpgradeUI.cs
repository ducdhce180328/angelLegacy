using UnityEngine;
using TMPro;
using System.Collections;

public class SkillUpgradeUI : MonoBehaviour
{
    public TMP_Text messageText;
    public float showTime = 2f;

    private Coroutine currentRoutine;

    void Start()
    {
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;

        yield return new WaitForSeconds(showTime);

        messageText.gameObject.SetActive(false);
    }
}