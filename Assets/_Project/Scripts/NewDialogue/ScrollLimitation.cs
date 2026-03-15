using System.Collections;
using UnityEngine;

public class LimitY : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;  
    [SerializeField] private float topPadding = 0f;   
    [SerializeField] private float bottomPadding = 0f; 
    [SerializeField] private float delayBeforeScroll = 0.05f;

    private RectTransform _content;
    private int _lastChildCount = 0;

    private void Awake()
    {
        _content = GetComponent<RectTransform>();
        _lastChildCount = transform.childCount;

        if (viewport == null)
        {
            viewport = transform.parent?.GetComponent<RectTransform>();
        }
    }

    private void LateUpdate()
    {
        if (viewport == null) return;

        if (transform.childCount > _lastChildCount)
        {
            StartCoroutine(ScrollToBottomAfterLayout());
            _lastChildCount = transform.childCount;
        }

        float contentHeight = _content.rect.height;
        float viewportHeight = viewport.rect.height;

        if (contentHeight <= viewportHeight)
        {
            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, 0);
            return;
        }

        Vector2 pos = _content.anchoredPosition;

        
        float minY = (contentHeight - viewportHeight) + bottomPadding;  // Верхняя граница
        float maxY = -(contentHeight - viewportHeight) - bottomPadding;  // Нижняя граница

        pos.y = Mathf.Clamp(pos.y, maxY, minY);
        _content.anchoredPosition = pos;
    }

    private IEnumerator ScrollToBottomAfterLayout()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        if (viewport == null) yield break;

        float contentHeight = _content.rect.height;
        float viewportHeight = viewport.rect.height;

        if (contentHeight <= viewportHeight)
        {
            _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, 0);
            yield break;
        }

        Vector2 newPos = _content.anchoredPosition;
        newPos.y = topPadding;  
        _content.anchoredPosition = newPos;
    }    
}