using UnityEngine;

public class LimitY : MonoBehaviour {
    [SerializeField] private Transform reference;
    [SerializeField] private float maxY = 45f;

    private RectTransform _rect;

    private void Awake() {
        _rect = GetComponent<RectTransform>();
    }

    private void LateUpdate() {
        Vector2 pos = _rect.anchoredPosition;
        float minY = reference != null ? reference.localPosition.y : 0f;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        _rect.anchoredPosition = pos;
    }
}