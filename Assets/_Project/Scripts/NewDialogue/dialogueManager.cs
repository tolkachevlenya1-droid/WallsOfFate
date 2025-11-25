using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour 
{
    #region UI
    [SerializeField] GameObject DialogueUI; 
    [SerializeField] TMP_Text NameTag;
    [SerializeField] TMP_Text Sentence;
    [SerializeField] List<Button> optionButtons;
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
    #endregion

    #region Events
    public event Action OnFinished;
    #endregion

    private void Awake() {
        if (_instance == null) {
            Instance = this;
            _instance = this;;
        }

        if (DialogueUI == null) {
            Debug.LogError("DialogueUI is not assigned in DialogueManager!", this);
        }

        if (NameTag == null) {
            Debug.LogError("NameTag is not assigned in DialogueManager!", this);
        }

        if (Sentence == null) {
            Debug.LogError("Sentence is not assigned in DialogueManager!", this);
        }
    }

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

            if (DialogueUI != null) {
                DialogueUI.SetActive(true);
            }

            IsInDialogue = true;
            CurrentID = 0;

            if (NameTag != null && _currentDialogue.sentences.Count > 0) {
                NameTag.text = _currentDialogue.sentences[CurrentID].CharName;
            }

            if (Sentence != null && _currentDialogue.sentences.Count > 0) {
                StartCoroutine(TypeSentence(_currentDialogue.sentences[CurrentID].Text));
            }
        }
    }

    public void TakeOption(int OptionId) {
        if (!OptionChosed && OptionId >= 0 && OptionId < OptionIDs.Count) {
            DisplayNextSentence(OptionIDs[OptionId]);
            OptionChosed = true;

            foreach (var b in optionButtons) {
                if (b != null && b.transform.childCount > 0) {
                    Text buttonText = b.transform.GetChild(0).GetComponent<Text>();
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

        UpdateResources();

        if (Sentence != null) {
            Sentence.text = "";
        }

        if (CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count) {
            CurrentID = _currentDialogue.sentences[CurrentID].NextNodeID;
        }

        if (CurrentID == -1) {
            CloseDialogue();
            return;
        }

        if (CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count &&
            _currentDialogue.sentences[CurrentID].isOption) {
            OptionChosed = false;
            LoadAllOptions();
        }

        if (OptionChosed && CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count) {
            SentenceWriten = false;

            if (NameTag != null) {
                NameTag.text = _currentDialogue.sentences[CurrentID].CharName;
            }

            if (Sentence != null) {
                StartCoroutine(TypeSentence(_currentDialogue.sentences[CurrentID].Text));
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
            if (Sentence != null) {
                Sentence.text = "";
            }

            CurrentID = NodeID;
            SentenceWriten = false;

            if (NameTag != null && CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count) {
                NameTag.text = _currentDialogue.sentences[CurrentID].CharName;
            }

            if (Sentence != null && CurrentID >= 0 && CurrentID < _currentDialogue.sentences.Count) {
                StartCoroutine(TypeSentence(_currentDialogue.sentences[CurrentID].Text));
            }
        }
    }

    private void LoadAllOptions() {
        if (_currentDialogue == null) return;

        int OptionalID = CurrentID;
        OptionIDs.Clear();
        OptionChosed = false;

        foreach (var b in optionButtons) {
            if (b != null && b.transform.childCount > 0) {
                Text buttonText = b.transform.GetChild(0).GetComponent<Text>();
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
                    Text buttonText = optionButtons[i].transform.GetChild(0).GetComponent<Text>();
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

        IsInDialogue = false;
        CurrentID = 0;
        OnFinished?.Invoke(); 
    }

    IEnumerator TypeSentence(string _Sentence) {
        if (Sentence == null) yield break;

        Sentence.text = "";
        foreach (var c in _Sentence) {
            yield return new WaitForSeconds(0.01f);
            Sentence.text += c;
        }

        SentenceWriten = true;
    }

    private void OnDestroy() {
        OnFinished = null;
    }
}