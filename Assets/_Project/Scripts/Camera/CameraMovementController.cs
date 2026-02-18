using UnityEngine;
using Zenject;

namespace Game
{

    public class CameraMovementController : MonoBehaviour
    {
        // Смещение относительно смещённой точки цели
        [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
        // Коэффициент плавности движения камеры
        [SerializeField] private float smoothSpeed = 0.125f;

        // Углы для установки изометрического обзора
        [SerializeField] private float angleX = 30f;
        [SerializeField] private float angleY = 45f;

        // Смещение цели по направлению вверх относительно камеры, чтобы персонаж находился ниже центра кадра
        [SerializeField] private float verticalBias = 1f;

        // Параметры зума камеры (для перспективной камеры – изменение поля зрения)
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minFOV = 15f;
        [SerializeField] private float maxFOV = 90f;

        // Ссылка на компонент камеры
        private Camera cam;
        // Цель, за которой будет следовать камера (например, персонаж)
        private Transform _player;

        [Inject]
        private void Construct(PlayerMoveController player)
        {
            ////Debug.Log("Player injected: " + player);
            _player = player.gameObject.transform;
        }

        void Awake()
        {
            if (_player == null)
            {
                //Debug.LogError("Player is not assigned!");
                enabled = false;
                return;
            }
            cam = GetComponent<Camera>();
            if (cam != null)
            {
                // Переводим камеру в перспективный режим
                cam.orthographic = false;
                // Устанавливаем начальное значение поля зрения (Field of View)
                cam.fieldOfView = 30f;
            }

            // Устанавливаем изометрическое направление обзора
            transform.rotation = Quaternion.Euler(angleX, angleY, 0f);
        }

        void Update()
        {
            // Обработка зума с помощью колёсика мыши для перспективной камеры (изменение Field of View)
            if (cam != null)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    cam.fieldOfView -= scroll * zoomSpeed;
                    cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
                }
            }
        }

        void LateUpdate()
        {
            if (_player != null)
            {
                // Смещаем точку следования на величину verticalBias вдоль направления "вверх" камеры
                Vector3 biasedTargetPosition = _player.position + transform.up * verticalBias;
                // Рассчитываем желаемую позицию камеры с учётом смещения
                Vector3 desiredPosition = biasedTargetPosition + offset;
                // Плавное перемещение камеры
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
        }
    }

}
