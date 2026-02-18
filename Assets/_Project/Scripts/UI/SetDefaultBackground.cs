using UnityEngine;
using UnityEngine.UI; // Для работы с компонентом Image
using TMPro; // Для работы с TextMeshProUGUI

namespace Game.UI
{
    public class SetDefaultBackground : MonoBehaviour
    {
        // Спрайт, который будет установлен как Source Image
        public Sprite background;

        // Метод, который будет вызван при активации объекта
        void OnEnable()
        {
            SetChildrenBackground();
            ClearDescriptionText();
            ProcessMixPanels();
        }

        // Метод для установки фона всем дочерним объектам
        void SetChildrenBackground()
        {
            // Получаем все дочерние объекты
            foreach (Transform child in transform)
            {
                // Получаем компонент Image (если он есть)
                Image image = child.GetComponent<Image>();
                if (image != null)
                {
                    // Устанавливаем Source Image
                    image.sprite = background;
                }
            }
        }

        // Метод для очистки текста в объекте Description, если есть Text (TMP)
        void ClearDescriptionText()
        {
            // Ищем объект с именем "Description" среди дочерних объектов
            Transform description = transform.Find("Description");
            if (description != null)
            {
                // Рекурсивно ищем объект с именем "Text (TMP)" внутри Description
                Transform textTMP = FindDeepChild(description, "Content");
                if (textTMP != null)
                {
                    // Получаем компонент TextMeshProUGUI
                    TextMeshProUGUI tmpText = textTMP.GetComponent<TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        // Устанавливаем текст в пустую строку
                        tmpText.text = "";
                    }
                }
            }
        }

        // Метод для обработки MixPanel0, MixPanel1, MixPanel2
        void ProcessMixPanels()
        {
            // Ищем объект с именем "Items" среди дочерних объектов
            Transform items = transform.Find("Items");
            if (items != null)
            {
                // Ищем объекты MixPanel0, MixPanel1, MixPanel2
                Transform mixPanel0 = items.Find("MixPanel0");
                Transform mixPanel1 = items.Find("MixPanel1");
                Transform mixPanel2 = items.Find("MixPanel2");

                // Обрабатываем MixPanel0
                if (mixPanel0 != null)
                {
                    RemoveChildrenExceptDragObjPlace(mixPanel0);
                }

                // Обрабатываем MixPanel1
                if (mixPanel1 != null)
                {
                    RemoveChildrenExceptDragObjPlace(mixPanel1);
                    mixPanel1.gameObject.SetActive(false); // Выключаем MixPanel1
                }

                // Обрабатываем MixPanel2
                if (mixPanel2 != null)
                {
                    RemoveChildrenExceptDragObjPlace(mixPanel2);
                    mixPanel2.gameObject.SetActive(false); // Выключаем MixPanel2
                }
            }
        }

        // Метод для удаления всех дочерних объектов, кроме объектов с именем DragObjPlace
        void RemoveChildrenExceptDragObjPlace(Transform parent)
        {
            // Создаем временный массив для хранения дочерних объектов
            Transform[] children = new Transform[parent.childCount];
            for (int i = 0; i < parent.childCount; i++)
            {
                children[i] = parent.GetChild(i);
            }

            // Проходим по всем дочерним объектам
            foreach (Transform child in children)
            {
                // Если объект не называется DragObjPlace, удаляем его
                if (child.name != "DragObjPlace")
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        // Рекурсивный метод для поиска дочернего объекта по имени
        Transform FindDeepChild(Transform parent, string name)
        {
            // Проверяем текущий объект
            if (parent.name == name)
            {
                return parent;
            }

            // Проходим по всем дочерним объектам
            foreach (Transform child in parent)
            {
                // Рекурсивно ищем в дочерних объектах
                Transform result = FindDeepChild(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            // Если объект не найден, возвращаем null
            return null;
        }
    }
}
