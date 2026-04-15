using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIFadeEffect : MonoBehaviour
{
    public Animator animator; 
    public UIHoverZoom uIHoverZoom;

    public enum FadeType
    {
        FadeIn,
        Blink,
        SlideFromTop,
        ZoomIn,
        ColorPingPong
    }

    public FadeType fadeType = FadeType.FadeIn;
    public int delay = 0;

    [Header("Fade In")]
    public float fadeDuration = 0.5f;

    [Header("Blink")]
    public float blinkDuration = 0.5f;
    public int blinkLoop = -1;

    [Header("Slide From Top")]
    public float slideDistance = 200f;
    public float slideDuration = 0.5f;

    [Header("Zoom In")]
    public float zoomDuration = 0.4f;
    public float startScale = 0.5f;

    [Header("Color PingPong")]
    public Color colorA = Color.white;
    public Color colorB = Color.red;
    public float colorDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPos;
    private Vector3 originalScale;

    private Graphic uiGraphic;
    private TMP_Text tmpText;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalPos = rectTransform != null ? rectTransform.anchoredPosition : transform.position;
        originalScale = transform.localScale;

        // Lấy component màu
        uiGraphic = GetComponent<Graphic>();
        tmpText = GetComponent<TMP_Text>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        StartCoroutine(WaitPlay());
    }

    IEnumerator WaitPlay()
    {
        yield return new WaitForSeconds(delay);
        PlayEffect();
    }

    public void PlayEffect()
    {
        canvasGroup.DOKill();
        if (rectTransform != null) rectTransform.DOKill();
        transform.DOKill();

        if (fadeType == FadeType.FadeIn)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, fadeDuration);
        }
        else if (fadeType == FadeType.Blink)
        {
            canvasGroup.alpha = 1;
            canvasGroup
                .DOFade(0, blinkDuration)
                .SetLoops(blinkLoop, LoopType.Yoyo);
        }
        else if (fadeType == FadeType.SlideFromTop)
        {
            rectTransform.anchoredPosition = originalPos + Vector3.up * slideDistance;
            canvasGroup.alpha = 0;

            rectTransform.DOAnchorPos(originalPos, slideDuration).SetEase(Ease.OutBack);
            canvasGroup.DOFade(1, slideDuration);
        }
        else if (fadeType == FadeType.ZoomIn)
        {
            if (animator != null)
            {
                animator.enabled = false;
            }

            transform.localScale = originalScale * startScale;
            canvasGroup.alpha = 0;

            transform.DOScale(originalScale, zoomDuration).SetEase(Ease.OutBack);
            canvasGroup.DOFade(1, zoomDuration);

            if (animator != null)
                StartCoroutine(WaitAnimate()); 
            
            if (uIHoverZoom != null)
                StartCoroutine(WaitEnable());
        }
        else if (fadeType == FadeType.ColorPingPong)
        {
            PlayColorPingPong();
        }
    }

    void PlayColorPingPong()
    {
        // UI Image / Text
        if (uiGraphic != null)
        {
            uiGraphic.color = colorA;
            uiGraphic.DOColor(colorB, colorDuration)
                     .SetLoops(-1, LoopType.Yoyo);
        }
        // TextMeshPro
        else if (tmpText != null)
        {
            tmpText.color = colorA;
            tmpText.DOColor(colorB, colorDuration)
                   .SetLoops(-1, LoopType.Yoyo);
        }
        // Sprite
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = colorA;
            spriteRenderer.DOColor(colorB, colorDuration)
                          .SetLoops(-1, LoopType.Yoyo);
        }
    }

    IEnumerator WaitAnimate()
    {
        yield return new WaitForSeconds(zoomDuration + 0.3f);

        if (animator != null)
        {
            animator.enabled = true;
        }
    }
    IEnumerator WaitEnable()
    {
        yield return new WaitForSeconds(zoomDuration);
        if (uIHoverZoom != null)
        {
            uIHoverZoom.enabled = true;
        }
    }
}