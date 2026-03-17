using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pickup))]
public class PickupEditor : Editor
{
    private const float LineHeight = 20f;
    private const float VerticalSpacing = 5f;

    public override void OnInspectorGUI()
    {
        // Отрисовка стандартных полей
        DrawDefaultInspector();

        // Получаем ссылку на объект Pickup
        Pickup pickup = (Pickup)target;

        // Отступ
        EditorGUILayout.Space();

        // Кнопка для добавления нового элемента в nestedDict и nestedList
        if (GUILayout.Button("Add Nested Dictionary"))
        {
            // Создаем новый словарь
            var newDict = new SerializableDictionary<string, string>();

            // Добавляем его в nestedList и nestedDict
            pickup.AddNested("NewKey", newDict);
        }

        // Отображаем nestedDict и nestedList
        EditorGUILayout.LabelField("Nested Dictionary", EditorStyles.boldLabel);
        foreach (var pair in pickup.NestedDict)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Отображаем ключ и индекс
            EditorGUILayout.LabelField($"Key: {pair.Key}, Index: {pair.Value}");

            // Отображаем вложенный словарь
            int nestedIndex = pair.Value;
            if (nestedIndex >= 0 && nestedIndex < pickup.NestedList.Count)
            {
                EditorGUI.indentLevel++;
                foreach (var innerPair in pickup.NestedList[nestedIndex])
                {
                    EditorGUILayout.LabelField($"{innerPair.Key}: {innerPair.Value}");
                }
                EditorGUI.indentLevel--;
            }

            // Кнопка для удаления
            if (GUILayout.Button("Remove"))
            {
                pickup.RemoveNested(pair.Key);
                break; // Выходим из цикла, чтобы избежать ошибок
            }

            EditorGUILayout.EndVertical();
        }
    }
}