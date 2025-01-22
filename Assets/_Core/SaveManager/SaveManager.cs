using System.Collections.Generic;
using System.IO;
using Dt.Attribute;
using UnityEngine;

public class SaveManager : MonoBehaviour, ISaveManager
{
    private List<object> data = new List<object>(100);

    public void Save()
    {
    }

    public void Load()
    {
    }

    public void Add(object value)
    {
        this.data.Add(value);
    }

    public void SaveData(string id, List<object[]> data)
    {
    }

    [Button]
    public void SaveDate()
    {
        var data = new List<object[]>(10);
        data.Add(new object[] { "id", Vector3.zero, ItemType.Bakery });
        object[] datas =
        {
            "Save Id",
            data
        };
        Vector3[] positions = new Vector3[3];
        positions[0] = (Vector3.zero);
        positions[1] = (Vector3.one);
        positions[2] = (Vector3.back);

        object a = Vector3.zero;
        string json = JsonUtility.ToJson(a);
        Debug.Log(json);
        Debug.Log(Application.persistentDataPath);
        File.WriteAllText(Application.persistentDataPath + "/data.json", json);
    }
}