using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class PickupData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string HideDescription { get; set; }
        public string Picture { get; set; }
        public bool Rendered { get; set; }
        public bool RenderedOnScreen { get; set; }

        // Обычные словари вместо SerializableDictionary
        public Dictionary<string, string> SimpleDict { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, int> NestedDict { get; set; } = new Dictionary<string, int>();
        public List<Dictionary<string, string>> NestedList { get; set; } = new List<Dictionary<string, string>>();

        // Метод для преобразования Pickup в PickupData
        public static PickupData FromPickup(Pickup pickup)
        {
            return new PickupData
            {
                Name = pickup.Name,
                Type = pickup.Type,
                Description = pickup.Description,
                HideDescription = pickup.HideDescription,
                Picture = pickup.Picture,
                Rendered = pickup.Rendered,
                RenderedOnScreen = pickup.RenderedOnScreen,
                SimpleDict = new Dictionary<string, string>(pickup.SimpleDict),
                NestedDict = new Dictionary<string, int>(pickup.NestedDict),
                NestedList = new List<Dictionary<string, string>>(pickup.NestedList)
            };
        }

        // Метод для преобразования PickupData в Pickup
        public Pickup ToPickup()
        {
            var pickup = new GameObject().AddComponent<Pickup>();
            pickup.Name = Name;
            pickup.Type = Type;
            pickup.Description = Description;
            pickup.HideDescription = HideDescription;
            pickup.Picture = Picture;
            pickup.Rendered = Rendered;
            pickup.RenderedOnScreen = RenderedOnScreen;

            // Преобразуем обычные словари обратно в SerializableDictionary
            foreach (var pair in SimpleDict)
            {
                pickup.SimpleDict[pair.Key] = pair.Value;
            }

            foreach (var pair in NestedDict)
            {
                pickup.NestedDict[pair.Key] = pair.Value;
            }

            pickup.NestedList = new List<SerializableDictionary<string, string>>();
            foreach (var dict in NestedList)
            {
                var serializableDict = new SerializableDictionary<string, string>();
                foreach (var pair in dict)
                {
                    serializableDict[pair.Key] = pair.Value;
                }
                pickup.NestedList.Add(serializableDict);
            }

            return pickup;
        }
    }
}