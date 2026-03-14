using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PickupManagerWindow : EditorWindow
{
    // Сериализованные данные для редактирования
    private SerializedObject serializedObject;
    private SerializedProperty pickupsProperty;

    [MenuItem("Window/Pickup Manager")]
    public static void ShowWindow()
    {
        GetWindow<PickupManagerWindow>("Pickup Manager");
    }

    private void OnEnable()
    {
        // Инициализация сериализованного объекта
        serializedObject = new SerializedObject(this);
        pickupsProperty = serializedObject.FindProperty("pickups");
    }

    private void OnGUI()
    {
        GUILayout.Label("Pickup Management", EditorStyles.boldLabel);

        // Кнопка для создания нового пикапа
        if (GUILayout.Button("Create New Pickup"))
        {
            CreateAndAddPickup("New Pickup", "Type", "Description", "Hide Description", "Picture");
        }

        // Кнопка для очистки всех пикапов
        if (GUILayout.Button("Clear All Pickups"))
        {
            ClearPickups();
        }

        GUILayout.Space(20);

        // Отображение списка пикапов
        if (pickupsProperty != null)
        {
            for (int i = 0; i < AssembledPickups.Count; i++)
            {
                SerializedProperty pickupProperty = pickupsProperty.GetArrayElementAtIndex(i);
                DrawPickup(pickupProperty, i);
            }
        }
    }

    // Метод для отрисовки отдельного пикапа
    private void DrawPickup(SerializedProperty pickupProperty, int index)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Отображение основных свойств пикапа
        EditorGUILayout.PropertyField(pickupProperty.FindPropertyRelative("Name"));
        EditorGUILayout.PropertyField(pickupProperty.FindPropertyRelative("Type"));
        EditorGUILayout.PropertyField(pickupProperty.FindPropertyRelative("Description"));
        EditorGUILayout.PropertyField(pickupProperty.FindPropertyRelative("HideDescription"));
        EditorGUILayout.PropertyField(pickupProperty.FindPropertyRelative("Picture"));

        // Отображение SerializableDictionary
        SerializedProperty simpleDictProperty = pickupProperty.FindPropertyRelative("SimpleDict");
        if (simpleDictProperty != null)
        {
            EditorGUILayout.PropertyField(simpleDictProperty, new GUIContent("Simple Dictionary"), true);
        }

        // Кнопка для удаления пикапа
        if (GUILayout.Button("Remove Pickup"))
        {
            RemovePickupByIndex(index);
        }

        EditorGUILayout.EndVertical();
    }

    // Метод для создания и добавления пикапа
    private void CreateAndAddPickup(string name, string type, string description, string hideDescription, string picture)
    {
        Pickup pickup = new GameObject().AddComponent<Pickup>();
        pickup.Name = name;
        pickup.Type = type;
        pickup.Description = description;
        pickup.HideDescription = hideDescription;
        pickup.Picture = picture;

        AssembledPickups.AddPickup(pickup);

        string allItems = "";
        foreach (Pickup pickup01 in AssembledPickups.GetAllPickups())
        {
            allItems += pickup01.Name;
        }
        Debug.Log(allItems);

        // Обновляем сериализованный объект
        if (serializedObject != null)
        {
            serializedObject.Update();
            pickupsProperty = serializedObject.FindProperty("pickups");
            serializedObject.ApplyModifiedProperties();
        }
    }

    // Метод для удаления пикапа по индексу
    private void RemovePickupByIndex(int index)
    {
        if (index >= 0 && index < AssembledPickups.Count)
        {
            Pickup pickup = AssembledPickups.GetPickupByIndex(index);
            AssembledPickups.RemovePickup(index);
            DestroyImmediate(pickup.gameObject); // Уничтожаем GameObject пикапа

            // Обновляем сериализованный объект
            if (serializedObject != null)
            {
                serializedObject.Update();
                pickupsProperty = serializedObject.FindProperty("pickups");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    // Метод для очистки всех пикапов
    private void ClearPickups()
    {
        foreach (var pickup in AssembledPickups.GetAllPickups())
        {
            DestroyImmediate(pickup.gameObject); // Уничтожаем все GameObject пикапов
        }
        AssembledPickups.Clear();

        // Обновляем сериализованный объект
        if (serializedObject != null)
        {
            serializedObject.Update();
            pickupsProperty = serializedObject.FindProperty("pickups");
            serializedObject.ApplyModifiedProperties();
        }
    }
}