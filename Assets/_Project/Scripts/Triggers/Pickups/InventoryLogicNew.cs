using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryLigicNew : MonoBehaviour
{
    [SerializeField] private List<GameObject> pickupPanels; // Список UI панелей с компонентом Pickup
    [SerializeField] private string _pickupType; // Тип пикапов, которые мы хотим отображать
    [SerializeField] private List<Image> panelImages;

    private List<Pickup> _currentPickupsOfType = new List<Pickup>();
    private int currentPanelIndex = 0;

    private void Update()
    {
        if (string.IsNullOrEmpty(_pickupType)) return;

        // Получаем все пикапы нужного типа
        IEnumerable<Pickup> pickupsEnumerable = AssembledPickups.GetPickupsByType(_pickupType);
        List<Pickup> newPickups = pickupsEnumerable.ToList();

        // Если список пикапов изменился
        if (!PickupListsEqual(_currentPickupsOfType, newPickups))
        {
            _currentPickupsOfType = newPickups;
            UpdateAllPanels();
            currentPanelIndex = 0;
        }
    }

    private bool PickupListsEqual(List<Pickup> list1, List<Pickup> list2)
    {
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i]) return false;
        }

        return true;
    }

    private void UpdateAllPanels()
    {
        for (int i = 0; i < pickupPanels.Count; i++)
        {
            if (i < _currentPickupsOfType.Count)
            {
                // Активируем панель и обновляем ее данные
                UpdateActivePanelFromPickup(_currentPickupsOfType[i]);
                NextPanel();
            }
        }
    }

    // Переключение на следующую панель
    public void NextPanel()
    {
        if (pickupPanels.Count == 0) return;

        currentPanelIndex = (currentPanelIndex + 1) % pickupPanels.Count;
        //SetActivePanel(currentPanelIndex);
    }

    // Переключение на предыдущую панель
    public void PreviousPanel()
    {
        if (pickupPanels.Count == 0) return;

        currentPanelIndex = (currentPanelIndex - 1 + pickupPanels.Count) % pickupPanels.Count;
        //SetActivePanel(currentPanelIndex);
    }

    // Активация конкретной панели и деактивация остальных
    private void SetActivePanel(int index)
    {
        for (int i = 0; i < pickupPanels.Count; i++)
        {
            pickupPanels[i].SetActive(i == index);
        }
    }

    // Обновление данных текущей активной панели из переданного Pickup
    public void UpdateActivePanelFromPickup(Pickup pickup)
    {
        if (pickupPanels.Count == 0 || currentPanelIndex >= pickupPanels.Count) return;

        var panelPickup = pickupPanels[currentPanelIndex].GetComponent<Pickup>();
        var pannelImage = pickupPanels[currentPanelIndex].gameObject.transform.Find("Image");
        var panelPickupImage = pannelImage?.GetComponent<Image>();
        if (panelPickup == null) return;

        // Копируем все данные из переданного Pickup
        panelPickup.Name = pickup.Name;
        panelPickup.Type = pickup.Type;
        panelPickup.Description = pickup.Description;
        panelPickup.HideDescription = pickup.HideDescription;
        panelPickup.Picture = pickup.Picture;
        panelPickup.Rendered = pickup.Rendered;
        panelPickup.RenderedOnScreen = pickup.RenderedOnScreen;

        // Копируем словари
        panelPickup.SimpleDict.Clear();
        foreach (var pair in pickup.SimpleDict)
        {
            panelPickup.SimpleDict.Add(pair.Key, pair.Value);
        }

        // Копируем вложенные структуры
        panelPickup.NestedDict.Clear();
        foreach (var pair in pickup.NestedDict)
        {
            panelPickup.NestedDict.Add(pair.Key, pair.Value);
        }

        panelPickup.NestedList.Clear();
        foreach (var dict in pickup.NestedList)
        {
            var newDict = new SerializableDictionary<string, string>();
            foreach (var pair in dict)
            {
                newDict.Add(pair.Key, pair.Value);
            }
            panelPickup.NestedList.Add(newDict);
        }

        // Загружаем и устанавливаем изображение, если путь указан
        if (!string.IsNullOrEmpty(pickup.Picture))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(pickup.Picture);
            if (loadedSprite != null && panelImages[currentPanelIndex] != null)
            {
                panelPickupImage.sprite = loadedSprite;
                if (!pannelImage.gameObject.activeSelf) pannelImage.gameObject.SetActive(true);
            }
            else
            {
                pickupPanels[currentPanelIndex].SetActive(false);
                //Debug.LogWarning($"Не удалось загрузить изображение по пути: {pickup.Picture}");
            }
        }

        // Обновляем отображение панели
        panelPickup.Display();
    }

    // Получение текущей активной панели
    public Pickup GetCurrentPickup()
    {
        if (pickupPanels.Count == 0 || currentPanelIndex >= pickupPanels.Count) return null;
        return pickupPanels[currentPanelIndex].GetComponent<Pickup>();
    }
}