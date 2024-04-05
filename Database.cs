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
            cmd.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS DiscordUsers (
                        DiscordId INTEGER PRIMARY KEY,
                        UserName TEXT,
                        Platform TEXT
                    );
                CREATE TABLE IF NOT EXISTS Stats (
                        DiscordId INTEGER PRIMARY KEY,
                        CacheDate INTEGER NOT NULL,
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
                        DiscordId INTEGER NOT NULL,
                        Playlist INTEGER,
                        Rank INTEGER,
                        MMR INTEGER,
                        Division INTEGER,
                        FOREIGN KEY (DiscordId) REFERENCES Stats(DiscordId)
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

    public void SaveAccount(string username, ulong userId, Platforms platform)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText =
                @"
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
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText =
                @"
                REPLACE INTO Stats (DiscordId, CacheDate, Wins, Goals, Saves, Assists, MVPs, Shots, RewardLevel, ProfileViews, UserName)
                VALUES (@DiscordId, @CacheDate, @Wins, @Goals, @Saves, @Assists, @MVPs, @Shots, @RewardLevel, @ProfileViews, @UserName);
                ";
            cmd.Parameters.AddWithValue("@DiscordId", userId);
            cmd.Parameters.AddWithValue("@CacheDate", stats.Date.ToBinary());
            cmd.Parameters.AddWithValue("@Wins", stats.Wins);
            cmd.Parameters.AddWithValue("@Goals", stats.Goals);
            cmd.Parameters.AddWithValue("@Saves", stats.Saves);
            cmd.Parameters.AddWithValue("@Assists", stats.Assists);
            cmd.Parameters.AddWithValue("@MVPs", stats.MVPs);
            cmd.Parameters.AddWithValue("@Shots", stats.Shots);
            cmd.Parameters.AddWithValue("@RewardLevel", ((uint)stats.RewardLevel + 1) / 3);
            cmd.Parameters.AddWithValue("@ProfileViews", stats.ProfileViews);
            cmd.Parameters.AddWithValue("@UserName", stats.Username);

            cmd.ExecuteNonQuery();
        }

        CacheModeStats(stats.Casual!, userId);
        CacheModeStats(stats.Vs1!, userId);
        CacheModeStats(stats.Vs2!, userId);
        CacheModeStats(stats.Vs3!, userId);
        CacheModeStats(stats.Dropshot!, userId);
        CacheModeStats(stats.Hoops!, userId);
        CacheModeStats(stats.Rumble!, userId);
        CacheModeStats(stats.Snowday!, userId);
    }

    private void CacheModeStats(Mode mode, ulong userId)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            // TODO IMPORTANT! Duplicate modestat entries when mode values update
            // check for modestat existance
            cmd.CommandText =
                @"
                SELECT COUNT(1) FROM ModeStats WHERE DiscordId = @DiscordId AND Playlist = @Playlist;
                ";

            cmd.Parameters.AddWithValue("@DiscordId", userId);
            cmd.Parameters.AddWithValue("@Playlist", mode.Playlist);

            bool rowsExist;
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    rowsExist = reader.GetInt32(0) > 0;
                }
                else
                {
                    rowsExist = false;
                }
            }

            if (rowsExist)
            {
                cmd.CommandText =
                    @"
                        UPDATE ModeStats
                        SET Rank = @Rank, MMR = @MMR, Division = @Division
                        WHERE DiscordId = @DiscordId AND Playlist = @Playlist;
                        ";
            }
            else
            {
                cmd.CommandText =
                    @"
                        INSERT INTO ModeStats (DiscordId, Playlist, Rank, MMR, Division)
                        VALUES (@DiscordId, @Playlist, @Rank, @MMR, @Division);
                    ";
            }

            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@DiscordId", userId);
            cmd.Parameters.AddWithValue("@Playlist", mode.Playlist);
            cmd.Parameters.AddWithValue("@Rank", mode.Rank);
            cmd.Parameters.AddWithValue("@MMR", mode.MMR);
            cmd.Parameters.AddWithValue("@Division", mode.Division);

            cmd.ExecuteNonQuery();
        }
    }

    public (string, string) GetAccount(ulong userId)
    {
        string username,
            platform;
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText =
                @"SELECT UserName, Platform FROM DiscordUsers WHERE DiscordId = @DiscordId;";
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

    public bool AccountExists(ulong userId)
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

    private bool AccountHasStats(ulong userId)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"SELECT COUNT(1) FROM Stats WHERE DiscordId = @DiscordId";
            cmd.Parameters.AddWithValue("@DiscordId", userId);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return 0 != reader.GetInt16(0);
                }
                else
                {
                    return false;
                }
            }
        }
    }

    private bool CacheOutdated(ulong userId)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"SELECT CacheDate FROM Stats WHERE DiscordId = @DiscordId";
            cmd.Parameters.AddWithValue("@DiscordId", userId);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    var cacheDate = DateTime.FromBinary(reader.GetInt64(0));
                    if (cacheDate.AddMinutes(10) <= DateTime.UtcNow)
                    {
                        Console.WriteLine(cacheDate.ToLongTimeString());
                        Console.WriteLine(DateTime.UtcNow.ToLongTimeString());
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public async Task<Stats?> RetreiveStats(ulong userId, Query query)
    {
        if (CacheOutdated(userId) || !AccountHasStats(userId))
        {
            Console.WriteLine($"{userId} didnt have stats");
            (var _username, var _platform) = GetAccount(userId);
            Platforms platform;
            Enum.TryParse(_platform, out platform);
            Stats? stats = await query.GetStats(platform, _username);
            CacheStats(stats!, userId);
            return stats;
        }

        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText =
                @"
                SELECT CacheDate, Wins, Goals, Saves, Assists, MVPs, Shots, RewardLevel, ProfileViews, UserName FROM Stats
                WHERE DiscordId = @DiscordId;
                ";
            cmd.Parameters.AddWithValue("@DiscordId", userId);

            Dictionary<GeneralStatTypes, uint> generics = new();
            Dictionary<Playlists, Mode> modes;
            DateTime date;
            string username;
            using (var reader = cmd.ExecuteReader())
            {
                // Not in an if statement, existance of an account should be checked outside the function
                reader.Read();
                modes = GetModes(userId);
                date = DateTime.FromBinary(reader.GetInt64(0));

                generics.Add(GeneralStatTypes.Wins, (uint)reader.GetInt32(1));
                generics.Add(GeneralStatTypes.Goals, (uint)reader.GetInt32(2));
                generics.Add(GeneralStatTypes.Saves, (uint)reader.GetInt32(3));
                generics.Add(GeneralStatTypes.Assists, (uint)reader.GetInt32(4));
                generics.Add(GeneralStatTypes.MVPs, (uint)reader.GetInt32(5));
                generics.Add(GeneralStatTypes.Shots, (uint)reader.GetInt32(6));
                generics.Add(GeneralStatTypes.RewardLevel, (uint)reader.GetInt32(7));
                generics.Add(GeneralStatTypes.ProfileViews, (uint)reader.GetInt32(8));

                username = reader.GetString(9);
            }
            return new Stats(generics, modes, date, username);
        }
    }

    private Dictionary<Playlists, Mode> GetModes(ulong userid)
    {
        using (SqliteCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText =
                @"
                SELECT Playlist, Rank, MMR, Division FROM ModeStats
                WHERE DiscordId = @DiscordId;
                ";
            cmd.Parameters.AddWithValue("@DiscordId", userid);

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
