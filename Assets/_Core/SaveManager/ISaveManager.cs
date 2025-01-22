using System.Collections.Generic;

public interface ISaveManager
{
    void Save();
    void Load();
    void Add(object value);
    void SaveData(string id, List<object[]> data);
}