using Microsoft.Data.Sqlite;

namespace RLStatus;

public sealed class Database
{
    private static Database? instance = null;
    private static Object lockObject = new();

    private SqliteConnection connection;

    private Database(string path = "data.sqlite")
    {
        connection = new($"Data Source={path}");
    }

    public static Database Instance
    {
        get
        {
            lock (lockObject)
            {
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
}
