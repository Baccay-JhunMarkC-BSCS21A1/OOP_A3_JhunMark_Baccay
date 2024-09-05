using System.Text.Json;

namespace coffee_shop_procedural;

public class Identity
{
    Dictionary<string, string> users = new Dictionary<string, string>();
    string _usersFile;

    public Identity(string usersFile)
    {
        _usersFile = usersFile;
        LoadUsers(usersFile);
    }

    void SaveUsers()
    {
        string json = JsonSerializer.Serialize(users);
        File.WriteAllText(_usersFile, json);
    }

    void LoadUsers(string usersFile)
    {
        if (File.Exists(usersFile))
        {
            string json = File.ReadAllText(usersFile);
            users = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        else
        {
            users.Add("admin", "password"); 
            SaveUsers();
        }
    }

    public string Login(string username, string password)
    {
        if (users.ContainsKey(username) && users[username] == password)
        {
            return username;
        }
        return string.Empty;
    }

    public string Register(string username, string password)
    {
        if (!users.ContainsKey(username))
        {
            users.Add(username, password);
            SaveUsers(); 
            return username;
        }
        else
        {
            Console.WriteLine("Username already exists.");
            return string.Empty;
        }
    }
}
