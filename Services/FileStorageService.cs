using System.Text.Json;
using System.IO;


namespace LibraryManagementSystem.Services;

public class FileStorageService
{
    public List<T> LoadData<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return new List<T>();

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public void SaveData<T>(string filePath, List<T> data)
    {
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
    }
}
