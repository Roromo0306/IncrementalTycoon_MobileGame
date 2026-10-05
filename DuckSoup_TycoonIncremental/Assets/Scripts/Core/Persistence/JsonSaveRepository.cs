using System;
using System.IO;
using UnityEngine;

public class JsonSaveRepository : ISaveRepository
{
    private const string SaveFileName = "savegame.json";

    private readonly string saveFilePath;


    public JsonSaveRepository()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath,SaveFileName);
    }


    public GameSaveData Load()
    {
        if (!File.Exists(saveFilePath))
        {
            return null;
        }

        string json = File.ReadAllText(saveFilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonUtility.FromJson<GameSaveData>(json);
    }


    public void Save(GameSaveData data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        string json = JsonUtility.ToJson(data,true);

        File.WriteAllText(saveFilePath,json);
    }


    public void Delete()
    {
        if (!File.Exists(saveFilePath))
        {
            return;
        }

        File.Delete(saveFilePath);
    }
}