using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string Type { get; set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public string HideDescription { get; set; }
    [field: SerializeField] public string Picture { get; set; }
    [field: SerializeField] public bool Rendered { get; set; }
    [field: SerializeField] public bool RenderedOnScreen { get; set; }


    // Первый словарь: string : string
    [SerializeField] private SerializableDictionary<string, string> simpleDict = new SerializableDictionary<string, string>();

    // Второй словарь: string : int (индекс в nestedList)
    [SerializeField] private SerializableDictionary<string, int> nestedDict = new SerializableDictionary<string, int>();

    // Список словарей: string : string
    [SerializeField] private List<SerializableDictionary<string, string>> nestedList = new List<SerializableDictionary<string, string>>();

    // Свойства для доступа к словарям
    public Dictionary<string, string> SimpleDict => simpleDict;
    public SerializableDictionary<string, int> NestedDict
    {
        get => nestedDict;
        set => nestedDict = value; 
    }
    public List<SerializableDictionary<string, string>> NestedList
    {
        get => nestedList;
        set => nestedList = value; 
    }

    // Добавляем новую пару в nestedDict и новый словарь в nestedList
    public void AddNested(string key, SerializableDictionary<string, string> value)
    {
        if (nestedDict.ContainsKey(key))
        {
            //Debug.LogWarning($"Ключ '{key}' уже существует в nestedDict.");
            return;
        }

        // Добавляем новый словарь в nestedList
        nestedList.Add(value);

        // Сохраняем индекс нового словаря в nestedDict
        nestedDict[key] = nestedList.Count - 1;
    }

    // Удаляем пару из nestedDict и соответствующий словарь из nestedList
    public void RemoveNested(string key)
    {
        if (!nestedDict.ContainsKey(key))
        {
            //Debug.LogWarning($"Ключ '{key}' не найден в nestedDict.");
            return;
        }

        int index = nestedDict[key];
        nestedDict.Remove(key);

        // Удаляем словарь из nestedList
        if (index >= 0 && index < nestedList.Count)
        {
            nestedList.RemoveAt(index);
        }

        // Обновляем индексы в nestedDict
        foreach (var k in nestedDict.Keys)
        {
            if (nestedDict[k] > index)
            {
                nestedDict[k]--;
            }
        }
    }

    public override string ToString()
    {
        return $"{Description} {Name} ({HideDescription})";
    }

    public void Display()
    {
        //Debug.Log(this.ToString());
    }
}