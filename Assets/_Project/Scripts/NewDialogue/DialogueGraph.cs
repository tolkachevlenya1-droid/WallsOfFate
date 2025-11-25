using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[System.Serializable]
public class DialogueGraph : MonoBehaviour 
{
   [SerializeField]
   private string DialogueName; /*{ get; private set; }*/

   [SerializeField]
   public List<Node> sentences = new List<Node>();
    
   public string GetName() {
        return DialogueName;
    }

   [System.Serializable]
   public class Node
    {
        #region GraphVariables 
        public int id;
        public bool IsMainCharacter;
        public string CharName;
        public string Text;
        public int NextNodeID;
        public bool isOption;
        public bool isAnswer;
        #endregion

        #region GameVariables
        public bool StartMinigame;
        

        #region Rsources
        public int Gold;
        public int Food;
        public int PeopleSatisfaction;
        public int CastleStrength;
        #endregion
        #endregion
        public Node(int _id, bool _IsMainCharacter, string _CharName, string _Text)
        {
            id = _id;
            IsMainCharacter = _IsMainCharacter;
            CharName = _CharName;
            Text = _Text;
        }

        public Node() { }
    }   
}
