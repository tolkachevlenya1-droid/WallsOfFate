using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EntryPoint;

public class DialogueManager : MonoBehaviour {
    #region UI
    [SerializeField] GameObject DialogueUI;
    [SerializeField] List<Button> optionButtons;

    [SerializeField] GameObject PlayerPanel;
    [SerializeField] GameObject NPCPanel;
    [SerializeField] Image NPCPortrait;
    [SerializeField] RectTransform SpawnPoint;
    [SerializeField] float panelSpacing = 10f; 
    [SerializeField] int maxPanelsOnScreen = 5; 
    #endregion

    #region MainInfo
    DialogueGraph _currentDialogue;
    #endregion

    #region Utility
    bool SentenceWriten = false;
    public int CurrentID;
    public bool IsInDialogue = false;
    [SerializeField] public List<int> OptionIDs = new List<int>();
    bool OptionChosed = true;

    private List<GameObject> spawnedPanels = new List<GameObject>(); 

    private static DialogueManager _instance;

    public static DialogueManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<DialogueManager>();

                if (_instance == null) {
                    Debug.LogError("No DialogueManager found in scene!");
                    return null;
                }
            }
            return _instance;
        }
        private set {
            _instance = value;
        }
    }

    bool startMinigame;
    MiniGameData miniGameData;
    #endregion

    #region Events
    public event Action OnFinished;
    public event Action<MiniGameData> OnMiniGameStartRequested;
    #endregion

    private void Awake() {
        if (_instance == null) {
            Instance = this;
            _instance = this;
        }
        else if (_instance != this) {
            Destroy(gameObject);
            return;
        }

        if (DialogueUI == null) {
            Debug.LogError("DialogueUI is not assigned in DialogueManager!", this);
        }
    }

    #region PannelsUI
    private void ShiftAllPanelsUp() {
        if (spawnedPanels.Count < 1) return;

        for (int i = 0; i < spawnedPanels.Count; i++) {
            GameObject panel = spawnedPanels[i];
            if (panel != null) {
                RectTransform rectTransform = panel.GetComponent<RectTransform>();
                if (rectTransform != null) {
                    Vector2 newPosition = Vector2.zero;
                    newPosition.y += rectTransform.anchoredPosition.y + panelSpacing;
                    rectTransform.anchoredPosition = newPosition;
                }
            }
        }
    }

    private void RecalculatePanelPositions() {
        float currentY = 0f;

        for (int i = 0; i < spawnedPanels.Count; i++) {
            GameObject panel = spawnedPanels[i];
            if (panel != null) {
                RectTransform rectTransform = panel.GetComponent<RectTransform>();
                if (rectTransform != null) {
                    rectTransform.anchoredPosition = new Vector2(0, currentY);
                    currentY += rectTransform.rect.height + panelSpacing;
                }
            }
        }
    }

    private void ClearSpawnedPanels() {
        foreach (GameObject panel in spawnedPanels) {
            if (panel != null) {
                Destroy(panel);
            }
        }
        spawnedPanels.Clear();
    }
    #endregion

    #region MiniGame

    void ConfigMiniGame() {
        if (_currentDialogue.sentences[CurrentID].StartMinigame) {
            startMinigame = true;
            miniGameData = GetParamsOfMinigame();
        }

    }

    MiniGameData GetParamsOfMinigame() {
        DialogueGraph.MiniGameType minigameType = _currentDialogue.sentences[CurrentID].MinigameType;
        Dictionary<string, object> minigameParams = _currentDialogue.sentences[CurrentID].MinigameParams;

        return new MiniGameData(minigameType, minigameParams);
    }
    #endregion

    #region MainLogic
    private void Start() {
        if (DialogueUI != null) {
            DialogueUI.SetActive(false);
        }
    }

    private void Update() {
        if (IsInDialogue && OptionChosed && DialogueUI != null && DialogueUI.activeSelf) {
            if (Input.GetKeyDown(KeyCode.E)) {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(DialogueGraph currentDialogue) {
        if (!IsInDialogue && currentDialogue != null) {
            _currentDialogue = currentDialogue;

            ClearSpawnedPanels();


            IsInDialogue = true;
            CurrentID = 0;

            if (DialogueUI != null) {
                Sprite mySprite = Resources.Load<Sprite>(_currentDialogue.Portrait);
                NPCPortrait.sprite = mySprite;

                DialogueUI.SetActive(true);
            }

            bool pannelSpawned = TryInstantiatePannel(out GameObject sentencePannel);

            if (pannelSpawned) {
                TMP_Text sentenceText = sentencePannel.transform.Find("Text")?.GetComponent<TMP_Text>();
                TMP_Text nameText = sentencePannel.transform.Find("NameText")?.GetComponent<TMP_Text>();

                if (nameText != null && _currentDialogue.sentences.Count > 0 && CurrentID < _currentDialogue.sentences.Count) {
                    nameText.text = _currentDialogue.sentences[CurrentID].CharName;
                }

                if (sentenceText != null && _currentDialogue.sentences.Count > 0 && CurrentID < _currentDialogue.sentences.Count) {
                    StartCoroutine(TypeSentence(_currentDialogue.sentences[CurrentID].Text, sentenceText));
                }
            }
        }
    }

    private bool TryInstantiatePannel(out GameObject sentencePannel) {
        sentencePannel = null;

        if (_currentDialogue == null || CurrentID < 0 || CurrentID >= _currentDialogue.sentences.Count)
            return false;

        GameObject panelPrefab = null;
        ShiftAllPanelsUp();
 
        if (_currentDialogue.sentences[CurrentID].IsMainCharacter) {
            panelPrefab = PlayerPanel;
        }
        else {
            panelPrefab = NPCPanel;
        }

        if (panelPrefab != null && SpawnPoint != null) {
            sentencePannel = Instantiate(panelPrefab, SpawnPoint);

            sentencePannel.transform.localPosition = Vector3.zero;

            spawnedPanels.Add(sentencePannel);


            if (spawnedPanels.Count > maxPanelsOnScreen) {
                GameObject oldestPanel = spawnedPanels[0];
                spawnedPanels.RemoveAt(0);
                Destroy(oldestPanel);

                RecalculatePanelPositions();
            }

            return true;
        }

        return false;
    }

     public void TakeOption(int OptionId) {
        if (!OptionChosed && OptionId >= 0 && OptionId < OptionIDs.Count) {
            DisplayNextSentence(OptionIDs[OptionId]);
            OptionChosed = true;

            foreach (var b in optionButtons) {
                if (b != null && b.transform.childCount > 0) {
                    TMP_Text buttonText = b.transform.GetChild(0).GetComponent<TMP_Text>();
                    if (buttonText != null) {
                        buttonText.text = "";
                    }
                    b.enabled = false;
                }
            }
        }
    }

    public void DisplayNextSentence() {
        if (!SentenceWriten || _currentDialogue == null) return;


        int nextNodeId = -1;
        if (CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count) {
            nextNodeId = _currentDialogue.sentences[CurrentID].NextNodeID;
        }

        if (nextNodeId == -1) {
            CloseDialogue();
            return;
        }

        CurrentID = nextNodeId;

        UpdateResources();

        if (CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count &&
            _currentDialogue.sentences[CurrentID].isOption) {
            OptionChosed = false;
            LoadAllOptions();
            return;
        }

        bool pannelSpawned = TryInstantiatePannel(out GameObject sentencePannel);

        if (pannelSpawned) {
            TMP_Text sentenceText = sentencePannel.transform.Find("Text")?.GetComponent<TMP_Text>();
            TMP_Text nameText = sentencePannel.transform.Find("Image/NameText")?.GetComponent<TMP_Text>();

            if (sentenceText != null) {
                sentenceText.text = "";
            }

            SentenceWriten = false;

            if (nameText != null) {
                nameText.text = _currentDialogue.sentences[CurrentID].CharName;
            }

            if (sentenceText != null) {
                StartCoroutine(TypeSentence(_currentDialogue.sentences[CurrentID].Text, sentenceText));
            }
        }
    }

    private void UpdateResources() {
        if (_currentDialogue == null || CurrentID < 0 || CurrentID >= _currentDialogue.sentences.Count)
            return;

        GameResources.GameResources.ChangeGold(_currentDialogue.sentences[CurrentID].Gold);
        GameResources.GameResources.ChangeFood(_currentDialogue.sentences[CurrentID].Food);
        GameResources.GameResources.ChangePeopleSatisfaction(_currentDialogue.sentences[CurrentID].PeopleSatisfaction);
        GameResources.GameResources.ChangeCastleStrength(_currentDialogue.sentences[CurrentID].CastleStrength);

    }

    public void DisplayNextSentence(int NodeID) {
        if (SentenceWriten && _currentDialogue != null) {

            bool pannelSpawned = TryInstantiatePannel(out GameObject sentencePannel);

            if (pannelSpawned) {
                TMP_Text sentenceText = sentencePannel.transform.Find("Text")?.GetComponent<TMP_Text>();
                TMP_Text nameText = sentencePannel.transform.Find("Image/NameText")?.GetComponent<TMP_Text>();

                if (sentenceText != null) {
                    sentenceText.text = "";
                }

                CurrentID = NodeID;
                ConfigMiniGame();

                SentenceWriten = false;

                if (nameText != null && CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count) {
                    nameText.text = _currentDialogue.sentences[CurrentID].CharName;
                }

                if (sentenceText != null && CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count) {
                    StartCoroutine(TypeSentence(_currentDialogue.sentences[CurrentID].Text, sentenceText));
                }
            }
        }
    }

    private void LoadAllOptions() {
        if (_currentDialogue == null) return;

        int OptionalID = CurrentID;
        OptionIDs.Clear();
        OptionChosed = false;

        // Включает кнопки
        foreach (var b in optionButtons) {
            if (b != null && b.transform.childCount > 0) {
                TMP_Text buttonText = b.transform.GetChild(0).GetComponent<TMP_Text>();
                if (buttonText != null) {
                    buttonText.text = "";
                }
                b.enabled = true;
            }
        }

        for (int i = 0; i < optionButtons.Count; i++) {
            if (OptionalID >= 0 && OptionalID < _currentDialogue.sentences.Count &&
                _currentDialogue.sentences[OptionalID].isOption) {
                if (i < optionButtons.Count && optionButtons[i] != null &&
                    optionButtons[i].transform.childCount > 0) {
                    TMP_Text buttonText = optionButtons[i].transform.GetChild(0).GetComponent<TMP_Text>();
                    if (buttonText != null) {
                        buttonText.text = _currentDialogue.sentences[OptionalID].Text;
                    }
                    OptionIDs.Add(_currentDialogue.sentences[OptionalID].id);
                    OptionalID++;
                }
            }
            else {
                break;
            }
        }
    }

    private void CloseDialogue() {
        if (DialogueUI != null) {
            DialogueUI.SetActive(false);
        }

        ClearSpawnedPanels();

        IsInDialogue = false;
        CurrentID = 0;
        OnFinished?.Invoke();
        OnMiniGameStartRequested?.Invoke(miniGameData);
    }

    IEnumerator TypeSentence(string _Sentence, TMP_Text textComponent) {
        if (textComponent == null) yield break;

        textComponent.text = "";
        foreach (var c in _Sentence) {
            yield return new WaitForSeconds(0.01f);
            textComponent.text += c;
        }

        SentenceWriten = true;
    }

    private void OnDestroy() {
        OnFinished = null;
        OnMiniGameStartRequested = null;
        ClearSpawnedPanels();
    }
    #endregion
}