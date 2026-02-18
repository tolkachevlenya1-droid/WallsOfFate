using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Data
{
    public static class CollectionSaveLoader
    {        
        public static bool LoadData()
        {
            if (Repository.TryGetData("Pickups", out List<PickupData> savedPickups)/* || savedPickups.Count != 0*/)
            {
                //AssembledPickups.Clear();
                foreach (var pickup in savedPickups)
                {
                    AssembledPickups.AddPickup(pickup.ToPickup());
                }
                Debug.Log("Collection loaded: " + savedPickups.Count + " items.");                
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LoadDefaultData()
        {
            TextAsset textAsset = UnityEngine.Resources.Load<TextAsset>("Data/Conclusions");
            if (textAsset == null)
            {
                Debug.LogError("File not found: Data/Conclusions");
                return;
            }

            try
            {
                // Десериализуем напрямую в список объектов
                List<PickupData> savedPickups = JsonConvert.DeserializeObject<List<PickupData>>(textAsset.text);

                if (savedPickups != null && savedPickups.Count > 0)
                {
                    LoadPickups(savedPickups);
                    Debug.Log($"Successfully loaded {savedPickups.Count} items");
                }
                else
                {
                    Debug.LogWarning("Loaded list is empty or null");               
                }
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON error: {ex.Message}\nJSON content: {textAsset.text}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"General error: {ex.Message}");
            }
        }

        private static void LoadPickups(List<PickupData> savedPickups)
        {
            // Очистка текущих пикапов (если нужно)
            //AssembledPickups.Clear();

            // Добавление загруженных пикапов
            foreach (var pickup in savedPickups)
            {
                AssembledPickups.AddPickup(pickup.ToPickup());
            }

            ////Debug.Log("Collection loaded: " + savedPickups.Count + " items.");
            //foreach (var pickup in savedPickups)
            //{
            //    //Debug.Log($"Pickup {pickup.Name}");
            //}
        }

        public static void SaveData()
        {
            var _pickups = AssembledPickups.GetAllPickups();
            var pickupData = _pickups.Select(PickupData.FromPickup).ToList();
            Repository.SetData("Pickups", pickupData);
            Debug.Log("Collection saved: " + _pickups.Count + " items.");
        }
    }
}
