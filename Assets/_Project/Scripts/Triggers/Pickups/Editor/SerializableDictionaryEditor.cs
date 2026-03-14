using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryEditor : PropertyDrawer
{
    private const float IndentWidth = 15f; // Отступ для вложенных элементов
    private float LineHeight = EditorGUIUtility.singleLineHeight; // Высота одной строки
    private const float VerticalSpacing = 2f; // Вертикальный отступ между элементами

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Начинаем отрисовку
        EditorGUI.BeginProperty(position, label, property);

        // Отображаем заголовок
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Получаем списки ключей и значений
        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        // Отступ для элементов
        position.y += LineHeight + VerticalSpacing;

        // Кнопка для добавления новой пары ключ-значение
        if (GUI.Button(new Rect(position.x, position.y, position.width, LineHeight), "Add Key-Value Pair"))
        {
            // Добавляем новый элемент с дефолтными значениями
            keys.arraySize++;
            values.arraySize++;

            // Устанавливаем дефолтные значения
            SerializedProperty newKey = keys.GetArrayElementAtIndex(keys.arraySize - 1);
            SerializedProperty newValue = values.GetArrayElementAtIndex(values.arraySize - 1);

            // Дефолтный ключ: "NewKey_X", где X — индекс
            newKey.stringValue = $"NewKey_{keys.arraySize}";

            // Дефолтное значение: "NewValue_X", где X — индекс
            if (newValue.propertyType == SerializedPropertyType.String)
            {
                newValue.stringValue = $"NewValue_{values.arraySize}";
            }
            else if (newValue.propertyType == SerializedPropertyType.Integer)
            {
                newValue.intValue = 0; // Дефолтное значение для int
            }
            else if (newValue.propertyType == SerializedPropertyType.Generic)
            {
                // Если значение — это вложенный словарь, инициализируем его
                var nestedDict = new SerializableDictionary<string, string>();
                newValue.managedReferenceValue = nestedDict;
            }
        }

        position.y += LineHeight + VerticalSpacing;

        // Отображаем существующие пары ключ-значение
        for (int i = 0; i < keys.arraySize; i++)
        {
            Rect lineRect = new Rect(position.x, position.y, position.width, LineHeight);

            // Прямоугольник для кнопки удаления
            Rect removeButtonRect = new Rect(position.x + position.width - 20, position.y, 20, LineHeight);

            // Прямоугольник для поля ввода ключа
            Rect keyRect = new Rect(position.x, position.y, position.width / 2 - 5, LineHeight);

            // Прямоугольник для поля ввода значения
            Rect valueRect = new Rect(position.x + position.width / 2 + 5, position.y, position.width / 2 - 25, LineHeight);

            // Поля для редактирования ключа и значения
            EditorGUI.PropertyField(keyRect, keys.GetArrayElementAtIndex(i), GUIContent.none);

            // Если значение — это вложенный словарь, отображаем его рекурсивно
            SerializedProperty valueProperty = values.GetArrayElementAtIndex(i);
            if (valueProperty.propertyType == SerializedPropertyType.Generic)
            {
                // Увеличиваем отступ для вложенного словаря
                Rect nestedRect = new Rect(position.x + IndentWidth, position.y + LineHeight + VerticalSpacing, position.width - IndentWidth, LineHeight);
                EditorGUI.PropertyField(nestedRect, valueProperty, GUIContent.none, true);
                position.y += EditorGUI.GetPropertyHeight(valueProperty, true) + VerticalSpacing;
            }
            else
            {
                EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
                position.y += LineHeight + VerticalSpacing;
            }

            // Кнопка для удаления пары
            if (GUI.Button(removeButtonRect, "x"))
            {
                keys.DeleteArrayElementAtIndex(i);
                values.DeleteArrayElementAtIndex(i);
                break; // Выходим из цикла, чтобы избежать ошибок
            }
        }

        // Завершаем отрисовку
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        float height = LineHeight * 2 + VerticalSpacing * 2; // Заголовок и кнопка добавления

        for (int i = 0; i < keys.arraySize; i++)
        {
            height += LineHeight + VerticalSpacing; // Ключ и значение
            SerializedProperty valueProperty = values.GetArrayElementAtIndex(i);

            // Если значение — это вложенный словарь, добавляем его высоту
            if (valueProperty.propertyType == SerializedPropertyType.Generic)
            {
                height += EditorGUI.GetPropertyHeight(valueProperty, true) + VerticalSpacing;
            }
        }

        return height;
    }
}