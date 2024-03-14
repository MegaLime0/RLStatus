using Microsoft.Data.Sqlite;

namespace RLStatus;

public sealed class Database
{
    private static Database? instance = null;
    private static Object lockObject = new();
    private static string _path = "data.sqlite";

    private SqliteConnection connection;

    private Database()
    {
        connection = new($"Data Source={_path}");
    }

    private static void GenerateFile()
    {
        // TODO: Create Database file and
        // poppulate it with tables
    }

    public static Database Instance
    {
        get
        {
            lock (lockObject)
            {
                if (!File.Exists(_path))
                {
                    GenerateFile();
                }

                if (instance == null)
                {
                    instance = new();
                }
                return instance;
            }
        }
    }

    public void SaveAccount(string acc, long userId)
    {
        // TODO
    }

    public string RetreiveAccount(long userId)
    {
        // TODO: Retreive account based on userid
        return String.Empty;
    }

    public void CacheStats()
    {
        // TODO: Implement a Stats class and cache the results
        // p.s. the results should include a time
    }

    public void RetreiveStats()
    {
        // TODO: Should require a stats class
        // (it doesnt exist yet
    }
}
