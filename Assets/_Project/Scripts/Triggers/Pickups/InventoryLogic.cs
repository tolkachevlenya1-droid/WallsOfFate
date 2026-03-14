using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.TriggerOjects.Pickups
{
    internal class InventoryLogic : MonoBehaviour
    {
        [SerializeField] private List<GameObject> inventory = new List<GameObject>();
        [SerializeField] private List<ItemSlot> itemSlots = new List<ItemSlot>(); // Список ItemSlot

        [SerializeField] private List<string> inventoryNames = new List<string>();

        private void Start()
        {
            UpdateInventory();
            SubscribeToItemSlots();
        }

        public void UpdateInventory()
        {
            inventory.Clear();

            // Находим все объекты с тегом "MixPannelElement"
            GameObject[] mixPanelElements = GameObject.FindGameObjectsWithTag("MixPannelElement");

            // Добавляем найденные объекты в inventory
            inventory.AddRange(mixPanelElements);
        }


        public void UpdateInventoryNames(PointerEventData eventData, string slotName)
        {
            // Проверяем, что eventData и перетаскиваемый объект существуют
            if (eventData != null && eventData.pointerDrag != null)
            {
                // Получаем копию перетаскиваемого объекта
                GameObject dragDropObject = eventData.pointerDrag.GetComponent<DragDropp>().GetDragCopy();
                if (dragDropObject == null)
                {
                    //Debug.LogWarning("DragCopy is null.");
                    return;
                }

                // Получаем имя перетаскиваемого объекта
                string itemName = dragDropObject.name;

                // Проверяем, содержит ли имя подстроку "(Clone)"
                if (itemName.Contains("(Clone)"))
                {
                    // Находим индекс начала подстроки "(Clone)"
                    int cloneIndex = itemName.IndexOf("(Clone)");

                    // Извлекаем часть имени до "(Clone)"
                    string nameWithoutClone = itemName.Substring(0, cloneIndex);
                    itemName = nameWithoutClone;
                }

                // Ищем индекс объекта в inventory, который соответствует slotName
                int slotIndex = inventory.FindIndex(obj => obj != null && obj.name == slotName);

                if (slotIndex != -1)
                {
                    // Если объект с таким именем найден, обновляем inventoryNames
                    if (slotIndex < inventoryNames.Count)
                    {
                        inventoryNames[slotIndex] = itemName;
                    }
                    else
                    {
                        // Если индекс выходит за пределы списка, добавляем новое имя
                        inventoryNames.Add(itemName);
                    }
                }
                else
                {
                    // Если объект с таким именем не найден, добавляем новое имя в inventoryNames
                    inventoryNames.Add(itemName);
                }
            }
            else
            {
                //Debug.LogWarning("EventData or pointerDrag is null.");
            }
        }

        private void SubscribeToItemSlots()
        {
            // Подписываемся на событие OnDropEvent для каждого ItemSlot
            foreach (var itemSlot in itemSlots)
            {
                itemSlot.OnDropEvent += OnItemSlotDrop;
            }
        }

        private void OnItemSlotDrop(PointerEventData eventData, string slotName)
        {
            UpdateInventory();
            // Обновляем имена в инвентаре при срабатывании события OnDropEvent
            UpdateInventoryNames(eventData, slotName);
        }

        private void OnDestroy()
        {
            // Отписываемся от событий при уничтожении объекта
            UnsubscribeFromItemSlots();
        }

        private void UnsubscribeFromItemSlots()
        {
            // Отписываемся от события OnDropEvent для каждого ItemSlot
            foreach (var itemSlot in itemSlots)
            {
                itemSlot.OnDropEvent -= OnItemSlotDrop;
            }
        }

        private string CheckPathExistence(Pickup pickup, List<string> inventoryNames)
        {
            string path = null;
            if (pickup.NestedDict.TryGetValue(inventoryNames[1], out int nextIndex))
            {
                if (pickup.NestedList[nextIndex].TryGetValue(inventoryNames[2], out string nestedPath))
                {
                    path = nestedPath;
                }
            }
            else
            {
                if (pickup.SimpleDict.TryGetValue(inventoryNames[1], out string simplePath))
                {
                    path = simplePath;
                }
            }
            if (path == null)
            {
                if (pickup.SimpleDict.TryGetValue(inventoryNames[2], out string simplePath))
                {
                    path = simplePath;
                }
            }

            return path;
        }

        // Пример использования
        public void CheckInventoryPath()
        {
            if (inventoryNames.Count == 0)
            {
                //Debug.Log("Inventory is empty.");
                return;
            }

            // Находим Pickup по имени первого объекта
            var pickup = AssembledPickups.FindByName(inventoryNames[0]);
            if (ReferenceEquals(pickup, null))
            {
                //Debug.Log($"Pickup with name '{inventoryNames[0]}' not found.");
                return;
            }

            // Проверяем путь
            string result = CheckPathExistence(pickup, inventoryNames);
            Pickup conclusion = AssembledPickups.FindByName(result);
            if (!ReferenceEquals(conclusion, null))
            {
                conclusion.Rendered = true;
            }
            if (result != null)
            {
                //Debug.Log($"Path exists. Result: {result}");
            }
            else
            {
                //Debug.Log("Path does not exist.");
            }
        }
    }
}