using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    [Tooltip("Массив спрайтов для анимации")]
    public Sprite[] frames;

    [Tooltip("Частота смены кадров (кадров в секунду)")]
    public float framesPerSecond = 10f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            //Debug.LogError("UISpriteAnimator: Компонент Image не найден!");
        }
    }

    private void Update()
    {
        if (frames.Length == 0) return;

        // Вычисляем текущий кадр на основе времени
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        image.sprite = frames[index];
    }
}
