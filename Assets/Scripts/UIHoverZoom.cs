using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class UIHoverZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float zoomScale = 1.1f;
    public float duration = 0.2f;

    private Vector3 originalScale;

    void Start()
    {
        StartCoroutine(InitScale());
    }

    IEnumerator InitScale()
    {
        yield return null;
        originalScale = transform.localScale;
    }

    public void Exit()
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * zoomScale, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
    }
}