using Quest;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryLogicEnd : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] private List<GameObject> pickupPanels;
    [SerializeField] private string _pickupType;

    [Header("Button Settings")]
    [SerializeField] private GameObject _buttonObject; 
    [SerializeField] private GameObject _inventoryObj; 

    private List<Pickup> _currentPickupsOfType = new List<Pickup>();
    private int _displayedImagesCount = 0; 
    private GameObject _ratObject;
    private void Start()
    {
        // Ищем объект с тегом "Rat" при старте
        _ratObject = GameObject.FindWithTag("Rat");
        if (_ratObject == null)
        {
            //Debug.LogWarning("Объект с тегом 'Rat' не найден!");
        }
        else
        {
            // Устанавливаем объект неактивным при старте (если нужно)
            _ratObject.SetActive(false);
        }
    }

    public List<GameObject> FindAllRatObjects()
    {
        List<GameObject> ratObjects = new List<GameObject>();

        // Находим активные объекты с тегом "Rat"
        GameObject[] activeRatObjects = GameObject.FindGameObjectsWithTag("Rat");
        ratObjects.AddRange(activeRatObjects);

        // Находим неактивные объекты с тегом "Rat"
        var inactiveRatObjects = Resources.FindObjectsOfTypeAll<GameObject>()
            .Where(go => go.CompareTag("Rat") &&
                        go.scene.isLoaded &&
                        !go.activeInHierarchy && // Только неактивные
                        !ratObjects.Contains(go)) // Исключаем уже найденные активные
            .ToList();

        ratObjects.AddRange(inactiveRatObjects);

        // Логирование для отладки
        if (ratObjects.Count == 0)
        {
            //Debug.LogWarning("Объекты с тегом 'Rat' не найдены в сцене.");
        }
        else
        {
            //Debug.Log($"Найдено {ratObjects.Count} объектов с тегом 'Rat': {string.Join(", ", ratObjects.Select(o => o.name))}");
        }

        return ratObjects;
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(_pickupType)) return;

        var newPickups = AssembledPickups.GetPickupsByType(_pickupType).ToList();

        UpdatePanelsVisibility();
        CheckAllImagesDisplayed();

        if (!PickupListsEqual(_currentPickupsOfType, newPickups))
        {
            _currentPickupsOfType = newPickups;
        }

        var ratObjs = FindAllRatObjects();
        if (AllPanelsOpened())
        {
            if (_ratObject != null)
            {
                _ratObject.SetActive(true);
            }
            else
            {
                _ratObject = GameObject.FindWithTag("Rat");
                if (_ratObject == null) _ratObject = ratObjs[0];
            }
        }
    }

    private bool AllPanelsOpened()
    {
        bool result = false;
        bool isAllChecked = true;
        foreach (var pannel in pickupPanels)
        {
            if (pannel != null)
            {
                bool imageCheck = pannel.transform.Find("Image").gameObject.activeSelf;
                isAllChecked = isAllChecked && imageCheck;
            }
        }
        result = isAllChecked || result;
        return result;
    }

    private void UpdatePanelsVisibility()
    {
        _displayedImagesCount = 0;

        foreach (var panel in pickupPanels)
        {
            if (panel == null) continue;

            var imageTransform = panel.transform.Find("Image");
            var panelPickup = panel.GetComponent<Pickup>();

            bool shouldDisplay = panelPickup != null &&
                               AssembledPickups.ContainsPrefab(panelPickup);

            if (imageTransform != null)
            {
                imageTransform.gameObject.SetActive(shouldDisplay);
                if (shouldDisplay) _displayedImagesCount++;
            }
        }
    }

    private void CheckAllImagesDisplayed()
    {
        if (_buttonObject == null) return;

        bool allDisplayed = _displayedImagesCount >= 3;
        _buttonObject.SetActive(allDisplayed);
    }

    public Pickup FindFirstNonRenderedPickup(string pickupType)
    {
        return AssembledPickups.GetPickupsByType(pickupType + "Conc")
            .FirstOrDefault(p => !p.Rendered);
    }

    public void RefreshPanel()
    {
        _displayedImagesCount = 0;
        _buttonObject.SetActive(false);
        foreach(var panel in pickupPanels)
        {
            var imageTransform = panel.transform.Find("Image");
            var indicationTransform = panel.transform.Find("Indication");
            imageTransform.gameObject.SetActive(false);
            indicationTransform.gameObject.SetActive(false);
            Pickup pannelPickup = panel.GetComponent<Pickup>();
            AssembledPickups.RemovePickupByName(pannelPickup.Name);
        }
    }

    public void ChangeVisibility(string pickupType)
    {
        var completedEvidenceQuests = QuestCollection.GetAllQuestGroups()
            .Where(q => q.Evidence != null &&
                       !string.IsNullOrEmpty(q.Evidence.EvidenceType))
            .ToList();

        // Проверяем, что такой квест ровно один
        bool hasSingleCompletedEvidenceQuest = completedEvidenceQuests.Count == 1;
        QuestGroup quest = null;
        if (hasSingleCompletedEvidenceQuest) quest = completedEvidenceQuests[0];

        if (quest.Evidence.EvidenceType != "" && hasSingleCompletedEvidenceQuest)
        {
            if(!quest.Evidence.DialogePlayed)
            {
                _inventoryObj.SetActive(false);
                //DialogueManager.GetInstance().EnterDialogueMode(quest.Evidence.Dialoge);
                quest.Evidence.DialogePlayed = true;
            }
            else
            {
                GameObject imagePannel = GameObject.FindWithTag("ImageField");
                GameObject textField = GameObject.FindWithTag("TextField");

                TMPro.TextMeshProUGUI textMeshProComponent = textField.GetComponent<TMPro.TextMeshProUGUI>();
                Sprite loadedSprite = Resources.Load<Sprite>(quest.Evidence.Picture);

                UnityEngine.UI.Image targetImage = imagePannel.GetComponent<UnityEngine.UI.Image>();
                    
                textMeshProComponent.text = quest.Evidence.Description;
                targetImage.sprite = loadedSprite;

            }
        }

    }

    private void UpdatePanelFromPickup(GameObject panel, Pickup pickup)
    {
        var panelPickup = panel.GetComponent<Pickup>();
        var panelImage = panel.transform.Find("Image")?.GetComponent<UnityEngine.UI.Image>();

        if (panelPickup == null) return;

        // Копируем основные данные
        CopyPickupData(panelPickup, pickup);

        // Устанавливаем изображение
        if (!string.IsNullOrEmpty(pickup.Picture) && panelImage != null)
        {
            LoadAndSetSprite(pickup.Picture, panelImage);
        }

        panelPickup.Display();
    }

    private void CopyPickupData(Pickup target, Pickup source)
    {
        target.Name = source.Name;
        target.Type = source.Type;
        target.Description = source.Description;
        target.HideDescription = source.HideDescription;
        target.Picture = source.Picture;
        target.Rendered = source.Rendered;
        target.RenderedOnScreen = source.RenderedOnScreen;

        // Копируем словари
        target.SimpleDict.Clear();
        foreach (var pair in source.SimpleDict)
        {
            target.SimpleDict.Add(pair.Key, pair.Value);
        }
    }

    private void LoadAndSetSprite(string path, UnityEngine.UI.Image targetImage)
    {
        Sprite loadedSprite = Resources.Load<Sprite>(path);
        if (loadedSprite != null)
        {
            targetImage.sprite = loadedSprite;
            targetImage.gameObject.SetActive(true);
        }
        else
        {
            //Debug.LogWarning($"Не удалось загрузить изображение: {path}");
        }
    }

    private bool PickupListsEqual(List<Pickup> list1, List<Pickup> list2)
    {
        if (list1.Count != list2.Count) return false;
        return !list1.Where((t, i) => t != list2[i]).Any();
    }

    public Pickup GetCurrentPickup()
    {
        return pickupPanels
            .Where(p => p != null && p.activeSelf)
            .Select(p => p.GetComponent<Pickup>())
            .FirstOrDefault();
    }
}