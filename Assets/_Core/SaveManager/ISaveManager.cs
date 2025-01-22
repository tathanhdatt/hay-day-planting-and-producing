public interface ISaveManager
{
    string Load(string id);
    void SaveData(string id, string data);
}