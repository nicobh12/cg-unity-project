using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreditsRoll : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform creditsContainer;
    [SerializeField] private float scrollSpeed = 60f;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float endYPosition = 2000f;

    [Header("Credits Text")]
    [TextArea(1, 5)]
    [SerializeField] private List<string> creditsLines = new List<string>();

    [Header("Prefab")]
    [SerializeField] private TextMeshProUGUI textPrefab;

    private bool isRunning;

    private void OnEnable()
    {
        StartCoroutine(RunCredits());
    }

    private IEnumerator RunCredits()
    {
        if (isRunning) yield break;
        isRunning = true;

        yield return new WaitForSeconds(startDelay);

        foreach (string line in creditsLines)
        {
            TextMeshProUGUI t = Instantiate(textPrefab, creditsContainer);
            t.text = line;
        }

        while (creditsContainer.anchoredPosition.y < endYPosition)
        {
            creditsContainer.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }

        isRunning = false;
    }
}