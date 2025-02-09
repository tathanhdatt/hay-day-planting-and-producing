﻿using System;
using UnityEngine;

public class SaveManager : ISaveManager
{
    private const string saveFileExtension = ".json";

    public string Load(string id)
    {
        try
        {
            string data = PlayerPrefs.GetString(id, string.Empty);
            return data;
            /*
            string filePath = $"{Application.persistentDataPath}/{id}{saveFileExtension}";
            if (!File.Exists(filePath)) return string.Empty;
            byte[] bytes = File.ReadAllBytes(filePath);
            if (bytes == null)
            {
                return string.Empty;
            }
            string data = Encoding.UTF8.GetString(bytes);
            return data;
            */
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        return string.Empty;
    }

    public void SaveData(string id, string data)
    {
        PlayerPrefs.SetString(id, data);
        /*
        File.WriteAllBytes($"{Application.persistentDataPath}/{id}{saveFileExtension}",
            Encoding.UTF8.GetBytes(data));
            */
    }
}