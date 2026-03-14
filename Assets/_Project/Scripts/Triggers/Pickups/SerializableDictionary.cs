using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    // Сохраняем словарь в списки для сериализации
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // Восстанавливаем словарь из списков после десериализации
    public void OnAfterDeserialize()
    {
        this.Clear();

        // Проверяем, что количество ключей и значений совпадает
        if (keys.Count != values.Count)
        {
            //Debug.LogWarning($"Количество ключей ({keys.Count}) не совпадает с количеством значений ({values.Count}). Словарь будет очищен.");
            return; // Прекращаем десериализацию, чтобы избежать ошибок
        }

        // Заполняем словарь
        for (int i = 0; i < keys.Count; i++)
        {
            if (!this.ContainsKey(keys[i])) // Проверяем, чтобы не было дубликатов ключей
            {
                this.Add(keys[i], values[i]);
            }
            else
            {
                //Debug.LogWarning($"Дубликат ключа '{keys[i]}' обнаружен. Значение не будет добавлено в словарь.");
            }
        }
    }
}