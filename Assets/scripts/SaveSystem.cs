using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;

public class SaveSystem
{
    public static SaveSystem instance;
    public SaveSystem()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception("Multiple instances of SaveSystem detected. SaveSystem is a singleton and should only have one instance.");
        }
    }
    readonly string file = "/savefile.json";
    readonly string fullPath = Application.persistentDataPath + "/savefile.json";

    public void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(fullPath, json);
        Debug.Log($"Saved to {fullPath} in {file}");
    }

    public SaveData Load()
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning("Save file not found. Returning null.");
            return null;
        }
        Debug.Log($"Loading from {fullPath} in {file}");
        string json = File.ReadAllText(fullPath);
        return JsonUtility.FromJson<SaveData>(json);
    }

}

public class SaveData
{
    public readonly int _currency;
    public readonly int _levelIndex;
    public readonly Vector3 _playerPos;
    public readonly List<Items> _inventoryItems;
    public SaveData(int currency, int levelIndex, Vector3 playerPos, List<Items> inventoryItems)
    {
        _currency = currency;
        _levelIndex = levelIndex;
        _playerPos = playerPos;
        _inventoryItems = inventoryItems;
    }
}