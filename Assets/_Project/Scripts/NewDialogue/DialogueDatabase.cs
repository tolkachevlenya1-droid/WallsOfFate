using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueDatabase {
    public List<DialogueGraph> dialogues = new List<DialogueGraph>();

    [Serializable]
    public class DialogueGraph {
        public string id;   // Уникальный идентификатор диалога
        public string name; // Человекочитаемое имя
        public List<Node> sentences = new List<Node>();

        [Serializable]
        public class Node {
            public int id;
            public bool IsMainCharacter;
            public string CharName;
            public string Text;
            public int NextNodeID;
            public bool isOption;
            public bool isAnswer;
        }
    }
}