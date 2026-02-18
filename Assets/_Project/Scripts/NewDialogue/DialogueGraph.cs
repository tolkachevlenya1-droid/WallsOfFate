using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class DialogueGraph : MonoBehaviour {
    [SerializeField]
    private string DialogueName;

    [SerializeField]
    public List<Node> sentences = new List<Node>();

    [SerializeField]
    public string Portrait;

    public string GetName() {
        return DialogueName;
    }

    [System.Serializable]
    public class Node {
        #region GraphVariables 
        public int id;
        public bool IsMainCharacter;
        public string CharName;
        public string Text;
        public int NextNodeID;
        public bool isOption;
        #endregion

        #region GameVariables
        public bool StartMinigame;
        public MiniGameType MiniGameType = MiniGameType.None;
        public string MiniGameSceneName = "";

        [SerializeField, TextArea(3, 5)] private string _parametersJson = "{}";

        private Dictionary<string, object> _cachedParams;

        public Dictionary<string, object> MinigameParams {
            get {
                if (_cachedParams == null || _cachedParams.Count == 0) {
                    try {
                        _cachedParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(_parametersJson);
                    }
                    catch {
                        _cachedParams = new Dictionary<string, object>();
                    }
                }
                return _cachedParams;
            }
            set {
                _cachedParams = value;
                _parametersJson = JsonConvert.SerializeObject(value, Formatting.Indented);
            }
        }

        #region Resources
        public int Gold;
        public int Food;
        public int PeopleSatisfaction;
        public int CastleStrength;
        #endregion

        #endregion
        public Node(int _id, bool _IsMainCharacter, string _CharName, string _Text) {
            id = _id;
            IsMainCharacter = _IsMainCharacter;
            CharName = _CharName;
            Text = _Text;
        }

        public Node() { }
    }
}
