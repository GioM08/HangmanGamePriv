namespace HangmanGameWPF
{
    internal static class SessionManager
    {
        public static int UserId { get; private set; }
        public static string FullName { get; private set; }
        public static string Email { get; private set; }
        public static int GlobalScore { get; private set; }
        public static bool IsLoggedIn => UserId > 0;

        public static void SetUser(int userId, string fullName, string email, int globalScore)
        {
            UserId = userId;
            FullName = fullName;
            Email = email;
            GlobalScore = globalScore;
        }

        public static void UpdateScore(int newScore) => GlobalScore = newScore;

        public static void Logout()
        {
            UserId = 0;
            FullName = null;
            Email = null;
            GlobalScore = 0;
        }
    }
}
