using System.Collections;
using TMPro;
using UnityEngine;

public class GhostMessageUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    private Coroutine currentRoutine;

    public void ShowMessage(string message, float duration = 2f)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message, duration));
    }

    IEnumerator ShowRoutine(string message, float duration)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;

        yield return new WaitForSeconds(duration);

        messageText.gameObject.SetActive(false);
    }
}