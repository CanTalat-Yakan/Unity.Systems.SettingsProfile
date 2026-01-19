using System;
using System.Collections.Generic;

namespace UnityEssentials
{
    /// <summary>
    /// Tiny helper to manage multiple named <see cref="SettingsProfile"/> instances and a current selection.
    /// </summary>
    public sealed class SettingsProfileManager
    {
        private readonly SettingsProfileManagerBase<SettingsProfile> _base;

        public string CurrentProfileName => _base.CurrentProfileName;

        public SettingsProfileManager(string profileName) =>
            _base = new SettingsProfileManagerBase<SettingsProfile>(profileName, name => new SettingsProfile(name));

        public SettingsProfile GetProfile(string profileName) => 
            _base.GetProfile(profileName);

        public SettingsProfile GetCurrentProfile() => 
            _base.GetCurrentProfile();

        public void SetCurrentProfile(string profileName, bool loadIfNeeded = true) =>
            _base.SetCurrentProfile(
                profileName,
                loadIfNeeded,
                p => p.GetOrLoad());
    }

    /// <summary>
    /// Tiny helper to manage multiple named <see cref="SettingsProfile{T}"/> instances and a current selection.
    /// </summary>
    public sealed class SettingsProfileManager<T> where T : new()
    {
        private readonly SettingsProfileManagerBase<SettingsProfile<T>> _base;
        private readonly Func<T> _defaultsFactory;

        public string CurrentProfileName => _base.CurrentProfileName;

        public SettingsProfileManager(string profileName, Func<T> defaultsFactory = null)
        {
            _defaultsFactory = defaultsFactory ?? (() => new T());
            _base = new SettingsProfileManagerBase<SettingsProfile<T>>(profileName, name => new SettingsProfile<T>(name, _defaultsFactory));
        }

        public SettingsProfile<T> GetProfile(string profileName) => 
            _base.GetProfile(profileName);

        public SettingsProfile<T> GetCurrentProfile() => 
            _base.GetCurrentProfile();

        public void SetCurrentProfile(string profileName, bool loadIfNeeded = true) =>
            _base.SetCurrentProfile(
                profileName,
                loadIfNeeded,
                p => p.GetValue());
    }
    
    internal sealed class SettingsProfileManagerBase<TProfile>
    {
        public string CurrentProfileName { get; private set; }

        private readonly Dictionary<string, TProfile> _profiles = new();
        private readonly Func<string, TProfile> _createProfile;

        public SettingsProfileManagerBase(string profileName, Func<string, TProfile> createProfile)
        {
            _createProfile = createProfile ?? throw new ArgumentNullException(nameof(createProfile));
            CurrentProfileName = Sanitize(profileName);
        }

        public TProfile GetProfile(string profileName)
        {
            var name = Sanitize(profileName);

            if (_profiles.TryGetValue(name, out var existing))
                return existing;

            var created = _createProfile(name);
            _profiles[name] = created;
            return created;
        }

        public TProfile GetCurrentProfile() => 
            GetProfile(CurrentProfileName);

        public void SetCurrentProfile(string profileName, bool loadIfNeeded, Action<TProfile> loadIfNeededAction)
        {
            CurrentProfileName = Sanitize(profileName);

            if (loadIfNeeded)
                loadIfNeededAction?.Invoke(GetCurrentProfile());
        }

        private static string Sanitize(string name) =>
            string.IsNullOrWhiteSpace(name) ? "Default" : name.Trim();
    }
}