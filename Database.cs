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
        connection.Open();
        CreateTables();
    }

    ~Database()
    {
        connection.Close();
        connection.Dispose();
    }

    private void CreateTables()
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS DiscordUsers (
                        DiscordId INTEGER PRIMARY KEY,
                        UserName TEXT,
                        Platform TEXT
                    );
                CREATE TABLE IF NOT EXISTS Stats (
                        StatId INTEGER PRIMARY KEY AUTOINCREMENT,
                        DiscordId INTEGER NOT NULL,
                        CacheDate TEXT NOT NULL,
                        Wins INTEGER DEFAULT 0,
                        Goals INTEGER DEFAULT 0,
                        Saves INTEGER DEFAULT 0,
                        Assists INTEGER DEFAULT 0,
                        MVPs INTEGER DEFAULT 0,
                        Shots INTEGER DEFAULT 0,
                        RewardLevel INTEGER DEFAULT 0,
                        ProfileViews INTEGER DEFAULT 0,
                        UserName TEXT,
                        FOREIGN KEY (DiscordId) REFERENCES DiscordUsers(DiscordId)
                    );
                CREATE TABLE IF NOT EXISTS ModeStats (
                        ModeStatId INTEGER PRIMARY KEY AUTOINCREMENT,
                        StatId INTEGER NOT NULL,
                        Playlist INTEGER,
                        Rank INTEGER,
                        MMR INTEGER,
                        Division INTEGER,
                        FOREIGN KEY (StatId) REFERENCES Stats(StatId)
                    );
                ";
            cmd.ExecuteNonQuery();
        }
    }

    public static Database Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new();
            }

            return instance;
        }
    }

    public void SaveAccount(string username, long userId, Platforms platform)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                INSERT INTO DiscordUsers (DiscordId, UserName, Platform)
                VALUES (@UserId, @Usr, @Platform)
                ";
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Usr", username);
            cmd.Parameters.AddWithValue("@Platform", platform.ToString());

            cmd.ExecuteNonQuery();
        }
    }

    public void CacheStats(Stats stats, ulong userId)
    {
        long statId;
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                INSERT INTO Stats (DiscordId, CacheDate, Wins, Goals, Saves, Assists, MVPs, Shots, RewardLevel, ProfileViews, UserName)
                VALUES (@DiscordId, @CacheDate, @Wins, @Goals, @Saves, @Assists, @MVPs, @Shots, @RewardLevel, @ProfileViews, @UserName)
                RETURNING StatId;
                ";
            cmd.Parameters.AddWithValue("@DiscordId", userId);
            cmd.Parameters.AddWithValue("@CacheDate", stats.Date.Raw);
            cmd.Parameters.AddWithValue("@Wins", stats.Wins);
            cmd.Parameters.AddWithValue("@Goals", stats.Goals);
            cmd.Parameters.AddWithValue("@Saves", stats.Saves);
            cmd.Parameters.AddWithValue("@Assists", stats.Assists);
            cmd.Parameters.AddWithValue("@MVPs", stats.MVPs);
            cmd.Parameters.AddWithValue("@Shots", stats.Shots);
            cmd.Parameters.AddWithValue("@RewardLevel", stats.RewardLevel);
            cmd.Parameters.AddWithValue("@ProfileViews", stats.ProfileViews);
            cmd.Parameters.AddWithValue("@UserName", stats.Username);

            statId = (long)cmd.ExecuteScalar()!;
        }

        CacheModeStats(stats.Casual!, statId);
        CacheModeStats(stats.Vs1!, statId);
        CacheModeStats(stats.Vs2!, statId);
        CacheModeStats(stats.Vs3!, statId);
        CacheModeStats(stats.Dropshot!, statId);
        CacheModeStats(stats.Hoops!, statId);
        CacheModeStats(stats.Rumble!, statId);
        CacheModeStats(stats.Snowday!, statId);
    }

    private void CacheModeStats(Mode mode, long statId)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                 INSERT INTO ModeStats (StatId, Playlist, Rank, MMR, Division)
                 VALUES (@StatId, @Playlist, @Rank, @MMR, @Division);
                 ";
            cmd.Parameters.AddWithValue("@StatId", statId);
            cmd.Parameters.AddWithValue("@Playlist", mode.Playlist);
            cmd.Parameters.AddWithValue("@Rank", mode.Rank);
            cmd.Parameters.AddWithValue("@MMR", mode.MMR);
            cmd.Parameters.AddWithValue("@Division", mode.Division);

            cmd.ExecuteNonQuery();
        }
    }

    public (string, string) GetAccount(long userId)
    {
        string username, platform;
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"SELECT UserName, Platform FROM DiscordUsers WHERE DiscordId = @DiscordId;";
            cmd.Parameters.AddWithValue("@DiscordId", userId);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    username = reader.GetString(0);
                    platform = reader.GetString(1);
                }
                else
                {
                    username = string.Empty;
                    platform = string.Empty;
                }
            }
        }
        return (username, platform);
    }

    public bool AccountExists(long userId)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @" SELECT COUNT(1) FROM DiscordUsers WHERE DiscordId = @DiscordId;";
            cmd.Parameters.AddWithValue("@DiscordId", userId);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // If the account amount is not 0 (its 1) return true
                    return 0 != reader.GetInt16(0);
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public Stats RetreiveStats(long userId)
    {
        // TODO
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                SELECT StatId, CacheDate, Wins, Goals, Saves, Assists, MVPs, Shots, RewardLevel, ProfileViews, UserName FROM Stats
                WHERE DiscordId = @DiscordId;
                ";
            cmd.Parameters.AddWithValue("@DiscordId", userId);

            Dictionary<GeneralStatTypes, uint> generics = new();
            Dictionary<Playlists, Mode> modes;
            Date date;
            string username;
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read(); // Not in an if statement, should be checked outside the function
                modes = GetModes(reader.GetInt32(0));
                date = new(reader.GetString(1));

                generics.Add(GeneralStatTypes.Wins, (uint)reader.GetInt32(2));
                generics.Add(GeneralStatTypes.Goals, (uint)reader.GetInt32(3));
                generics.Add(GeneralStatTypes.Saves, (uint)reader.GetInt32(4));
                generics.Add(GeneralStatTypes.Assists, (uint)reader.GetInt32(5));
                generics.Add(GeneralStatTypes.MVPs, (uint)reader.GetInt32(6));
                generics.Add(GeneralStatTypes.Shots, (uint)reader.GetInt32(7));
                generics.Add(GeneralStatTypes.RewardLevel, (uint)reader.GetInt32(8));
                generics.Add(GeneralStatTypes.ProfileViews, (uint)reader.GetInt32(9));

                username = reader.GetString(10);
            }
            return new Stats(generics, modes, date, username);
        }
    }

    private Dictionary<Playlists, Mode> GetModes(int statId)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                SELECT Playlist, Rank, MMR, Division FROM ModeStats
                WHERE StatId = @StatId;
                ";
            cmd.Parameters.AddWithValue("@StatId", statId);

            Dictionary<Playlists, Mode> modes = new();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Ranks rank = (Ranks)reader.GetInt32(1);
                    int mmr = reader.GetInt32(2);
                    int division = reader.GetInt32(3);
                    Playlists plist;
                    switch (reader.GetInt32(0))
                    {
                        case (ushort)Playlists.Casual:
                            plist = Playlists.Casual;
                            break;
                        case (ushort)Playlists.Vs1:
                            plist = Playlists.Vs1;
                            break;
                        case (ushort)Playlists.Vs2:
                            plist = Playlists.Vs2;
                            break;
                        case (ushort)Playlists.Vs3:
                            plist = Playlists.Vs3;
                            break;
                        case (ushort)Playlists.Hoops:
                            plist = Playlists.Hoops;
                            break;
                        case (ushort)Playlists.Rumble:
                            plist = Playlists.Rumble;
                            break;
                        case (ushort)Playlists.Dropshot:
                            plist = Playlists.Dropshot;
                            break;
                        case (ushort)Playlists.Snowday:
                            plist = Playlists.Snowday;
                            break;
                        default:
                            continue;
                    }
                    Mode mode = new(mmr, division, rank, plist);
                    modes.Add(plist, mode);
                }
            }
            return modes;
        }
    }
}
