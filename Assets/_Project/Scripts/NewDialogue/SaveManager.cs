using UnityEngine;
using System.IO;
using System;

public class SaveManager : MonoBehaviour {
    private DialogueDatabase database;
    private string filePath;

    private void Awake() {
        filePath = Path.Combine(Application.persistentDataPath, "dialogues.json");
        LoadDialogues();
    }

    public void LoadDialogues() {
        if (File.Exists(filePath)) {
            string json = File.ReadAllText(filePath);
            database = JsonUtility.FromJson<DialogueDatabase>(json);
        }
        else {
            database = new DialogueDatabase();
            SaveDialogues();
        }
    }

    public void SaveDialogues() {
        string json = JsonUtility.ToJson(database, true);
        File.WriteAllText(filePath, json);
    }

    public DialogueDatabase GetDatabase() => database;
    public void SetDatabase(DialogueDatabase newDatabase) => database = newDatabase;
}