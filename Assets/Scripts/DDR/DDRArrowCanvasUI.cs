using System;
using UnityEngine;
using UnityEngine.UI;

public class DDRArrowCanvasUI : MonoBehaviour
{
    [Serializable]
    public class ArrowSpriteMapping
    {
        public string keyName;
        public Sprite sprite;
    }

    [SerializeField] private DDRManager ddrManager;
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private Image arrowImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Vector2 previewStartPosition = new Vector2(0f, 260f);
    [SerializeField] private Vector2 inputHitPosition = new Vector2(0f, 0f);
    [SerializeField] private ArrowSpriteMapping[] arrowSprites;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        SetVisible(false);
    }

    private void Update()
    {
        if (ddrManager == null || arrowImage == null || arrowRect == null || !ddrManager.HasActiveSong)
        {
            SetVisible(false);
            return;
        }

        if (!ddrManager.IsPreviewPhase && !ddrManager.IsInputPhase)
        {
            SetVisible(false);
            return;
        }

        Sprite mappedSprite = GetSpriteForKey(ddrManager.CurrentStepKey);
        if (mappedSprite == null)
        {
            SetVisible(false);
            return;
        }

        arrowImage.sprite = mappedSprite;
        SetVisible(true);

        if (ddrManager.IsPreviewPhase)
        {
            arrowRect.anchoredPosition = Vector2.Lerp(previewStartPosition, inputHitPosition, ddrManager.CurrentStepPreviewProgress);
        }
        else
        {
            arrowRect.anchoredPosition = inputHitPosition;
        }
    }

    private Sprite GetSpriteForKey(string keyName)
    {
        if (arrowSprites == null)
        {
            return null;
        }

        foreach (ArrowSpriteMapping mapping in arrowSprites)
        {
            if (mapping != null && string.Equals(mapping.keyName, keyName, StringComparison.OrdinalIgnoreCase))
            {
                return mapping.sprite;
            }
        }

        return null;
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        if (arrowImage != null)
        {
            arrowImage.enabled = visible;
        }
    }
}
