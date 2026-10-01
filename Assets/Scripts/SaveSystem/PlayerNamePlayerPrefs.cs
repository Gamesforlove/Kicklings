using UnityEngine;

namespace SaveSystem
{
    public static class PlayerNamePlayerPrefs
    {
        public const string PlayerNameKey = "PlayerName";
        public const string DefaultPlayerName = "Player";

        public static void SetPlayerName(string playerName)
        {
            PlayerPrefs.SetString(PlayerNameKey, NormalizePlayerName(playerName));
            PlayerPrefs.Save();
        }

        public static string GetPlayerName()
        {
            return NormalizePlayerName(PlayerPrefs.GetString(PlayerNameKey, DefaultPlayerName));
        }

        private static string NormalizePlayerName(string playerName)
        {
            return string.IsNullOrWhiteSpace(playerName)
                ? DefaultPlayerName
                : playerName.Trim();
        }
    }
}
