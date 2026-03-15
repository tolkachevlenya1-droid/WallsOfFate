using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public class CameraObstacleTransparency : MonoBehaviour
    {
        [Header("Настройки слежения")]
        public LayerMask obstacleMask;        // Слои, по которым искать препятствия
        public float fadeSpeed = 3f;          // Скорость изменения прозрачности
        [Range(0f, 1f)]
        public float transparentAlpha = 0.3f; // Желаемая прозрачность (от 0 до 1)

        // Словарь для хранения оригинальных цветов для каждого материала рендера
        private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();
        // Набор объектов, которые в данный момент должны быть затемнены
        private HashSet<Renderer> currentObstacles = new HashSet<Renderer>();

        private Transform _player;

        [Inject]
        private void Construct(PlayerMoveController player)
        {
            //Debug.Log("Player injected: " + player);
            _player = player.gameObject.transform;
        }

        void Update()
        {
            if (_player == null)
                return;

            // Определяем направление и расстояние от камеры до цели
            Vector3 direction = _player.position - transform.position;
            float distance = direction.magnitude;

            // Находим все препятствия между камерой и целью
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, obstacleMask);
            HashSet<Renderer> hitRenderers = new HashSet<Renderer>();

            foreach (RaycastHit hit in hits)
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend == null)
                    continue;

                hitRenderers.Add(rend);

                // Если впервые встречаем этот рендер, сохраняем оригинальные цвета
                if (!originalColors.ContainsKey(rend))
                {
                    int count = rend.materials.Length;
                    Color[] origColors = new Color[count];
                    for (int i = 0; i < count; i++)
                    {
                        // Берём текущий цвет из материала
                        origColors[i] = rend.materials[i].color;
                    }
                    originalColors[rend] = origColors;
                }

                // Применяем плавное затемнение для каждого материала данного рендера
                int matCount = rend.materials.Length;
                for (int i = 0; i < matCount; i++)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    rend.GetPropertyBlock(block, i);

                    // Если цвет не задан в блоке, используем оригинальный цвет
                    Color currentColor = block.GetColor("_Color");
                    if (currentColor == default(Color))
                    {
                        currentColor = originalColors[rend][i];
                    }

                    // Плавно меняем альфа к прозрачному значению
                    float newAlpha = Mathf.MoveTowards(currentColor.a, transparentAlpha, fadeSpeed * Time.deltaTime);
                    Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                    block.SetColor("_Color", newColor);
                    rend.SetPropertyBlock(block, i);
                }

                currentObstacles.Add(rend);
            }

            // Для рендеров, которые ранее были затемнены, но теперь не попали в Raycast,
            // возвращаем оригинальный цвет (альфа = 1 или исходное значение)
            List<Renderer> toRemove = new List<Renderer>();
            foreach (Renderer rend in currentObstacles)
            {
                if (hitRenderers.Contains(rend))
                    continue;

                bool fullyRestored = true;
                int matCount = rend.materials.Length;
                for (int i = 0; i < matCount; i++)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    rend.GetPropertyBlock(block, i);
                    Color currentColor = block.GetColor("_Color");
                    if (currentColor == default(Color))
                    {
                        // Если блок не установлен, берём оригинальный
                        currentColor = originalColors[rend][i];
                    }
                    float targetAlpha = originalColors[rend][i].a; // исходное значение альфа (обычно 1)
                    float newAlpha = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
                    Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                    block.SetColor("_Color", newColor);
                    rend.SetPropertyBlock(block, i);

                    if (!Mathf.Approximately(newAlpha, targetAlpha))
                        fullyRestored = false;
                }

                if (fullyRestored)
                {
                    // Если для всех материалов альфа восстановлена, удаляем настройки
                    rend.SetPropertyBlock(new MaterialPropertyBlock());
                    toRemove.Add(rend);
                }
            }

            // Убираем полностью восстановленные объекты из списка отслеживаемых и удаляем их сохраненные оригиналы
            foreach (Renderer rend in toRemove)
            {
                currentObstacles.Remove(rend);
                originalColors.Remove(rend);
            }
        }
    }

}
