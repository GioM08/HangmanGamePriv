using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HangmanGameWPF
{
    internal static class UserStore
    {
        private static readonly string DataFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HangmanGame", "users.dat");

        private static readonly Dictionary<string, string> Users =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static string CurrentUser { get; private set; }

        static UserStore()
        {
            Load();
            if (!Users.ContainsKey("admin"))
                Users["admin"] = "1234";
        }

        public static bool Login(string username, string password)
        {
            if (Users.TryGetValue(username, out var pw) && pw == password)
            {
                CurrentUser = username;
                return true;
            }
            return false;
        }

        public static bool Register(string username, string password)
        {
            if (Users.ContainsKey(username)) return false;
            Users[username] = password;
            Save();
            return true;
        }

        public static bool UserExists(string username) => Users.ContainsKey(username);

        private static void Load()
        {
            if (!File.Exists(DataFile)) return;
            foreach (var line in File.ReadAllLines(DataFile))
            {
                var i = line.IndexOf(':');
                if (i > 0) Users[line.Substring(0, i)] = line.Substring(i + 1);
            }
        }

        private static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DataFile));
            File.WriteAllLines(DataFile, Users.Select(kv => $"{kv.Key}:{kv.Value}"));
        }
    }
}
