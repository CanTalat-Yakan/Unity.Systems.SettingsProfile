using UnityEngine;

namespace UnityEssentials
{
    /// <summary>
    /// Global, PlayerPrefs-like access to a JSON-backed settings profile.
    /// Loads automatically at <see cref="RuntimeInitializeLoadType.BeforeSceneLoad"/>.
    /// </summary>
    public static class SettingsService
    {
        private static SettingsProfile _profile;

        /// <summary>The active profile instance (loads on first access).</summary>
        public static SettingsProfile Profile => _profile ??= CreateAndLoad("Default");

        /// <summary>
        /// Ensures settings are loaded before the first scene.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad() =>
            _ = Profile.GetOrLoad();

        public static void SetProfile(string profileName) =>
            _profile = CreateAndLoad(profileName);

        public static bool HasKey(string key) => Profile.GetOrLoad().HasKey(key);
        public static void DeleteKey(string key) { Profile.Value.DeleteKey(key); }
        public static void DeleteAll() { Profile.Value.DeleteAll(); }

        public static string GetString(string key, string defaultValue = "") => Profile.GetOrLoad().GetString(key, defaultValue);
        public static int GetInt(string key, int defaultValue = 0) => Profile.GetOrLoad().GetInt(key, defaultValue);
        public static float GetFloat(string key, float defaultValue = 0f) => Profile.GetOrLoad().GetFloat(key, defaultValue);
        public static bool GetBool(string key, bool defaultValue = false) => Profile.GetOrLoad().GetBool(key, defaultValue);

        public static T Get<T>(string key, T defaultValue = default) => Profile.GetOrLoad().Get(key, defaultValue);

        public static void SetString(string key, string value) => Profile.Value.SetString(key, value);
        public static void SetInt(string key, int value) => Profile.Value.SetInt(key, value);
        public static void SetFloat(string key, float value) => Profile.Value.SetFloat(key, value);
        public static void SetBool(string key, bool value) => Profile.Value.SetBool(key, value);

        public static void Set<T>(string key, T value) => Profile.Value.Set(key, value);

        /// <summary>Explicit save to disk</summary>
        public static void Save() =>
            _profile?.Save();

        private static SettingsProfile CreateAndLoad(string profileName)
        {
            var profile = SettingsProfileFactory.Create(profileName);
            profile.Load();
            return profile;
        }
    }
}