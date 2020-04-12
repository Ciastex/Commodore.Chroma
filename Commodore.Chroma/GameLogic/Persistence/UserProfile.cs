using Commodore.Engine;
using Commodore.Engine.Persistence.AppData;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.World;
using Chroma.Input;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;

namespace Commodore.GameLogic.Persistence
{
    [Serializable]
    public class UserProfile
    {
        public static UserProfile Instance { get; private set; }

        private UserProfile() { }

        private bool _autoSaveActive;

        [NonSerialized]
        private Timer _profileSaveTimer;

        public bool IsInitialized { get; set; }
        public bool Saving { get; private set; }

        public Dictionary<string, bool> MemoryBankStates = new Dictionary<string, bool>
        {
            { "USR_1", true },
            { "USR_2", false },
            { "USR_3", false },
            { "USR_4", false },
            { "USR_5", false },
            { "USR_6", false },
            { "USR_7", false },
        };

        public string UserName { get; set; } = "Enigma";
        public Directory RootDirectory { get; set; } = new Directory();
        public Network Network { get; set; } = new Network();

        public KeyCode PreferredBreakKey { get; set; } = KeyCode.Pause;
        public KeyCode GfxModeResetKey { get; set; } = KeyCode.F12;

        public bool AutoSave
        {
            get => _autoSaveActive;
            set
            {
                if (value && !_autoSaveActive)
                {
                    if (_profileSaveTimer == null)
                        _profileSaveTimer = new Timer(10000);

                    _profileSaveTimer.Elapsed += ProfileSaveTimer_Elapsed;
                    _profileSaveTimer.Start();
                }
                else if (!value && _autoSaveActive)
                {
                    if (_profileSaveTimer != null)
                    {
                        _profileSaveTimer.Stop();
                        _profileSaveTimer.Elapsed -= ProfileSaveTimer_Elapsed;
                    }
                }

                _autoSaveActive = value;
            }
        }

        public bool IsMemoryBankLocked(int number)
        {
            var name = $"USR_{number}";

            if (MemoryBankStates.ContainsKey(name))
            {
                return !MemoryBankStates[name];
            }

            return true;
        }

        public void UnlockUserMemoryBank(int number)
        {
            var name = $"USR_{number}";

            if (MemoryBankStates.ContainsKey(name))
            {
                MemoryBankStates[name] = true;
            }
        }

        public void LockUserMemoryBank(int number)
        {
            var name = $"USR_{number}";

            if (MemoryBankStates.ContainsKey(name))
            {
                MemoryBankStates[name] = false;
            }
        }

        public void SaveToFile()
        {
            if (Saving)
            {
                DebugLog.Warning("Tried to save a profile to file, but it's already being saved!");
                return;
            }

            DebugLog.Info("Saving profile...");

            try
            {
                Saving = true;
                RoamingStorage.CreateDirectoryIfDoesNotExist("Ensign Computer");

                if (RoamingStorage.FileExists("Ensign Computer/player.save"))
                    RoamingStorage.RemoveFile("Ensign Computer/player.save");

                using (var stream = RoamingStorage.OpenWrite("Ensign Computer/player.save"))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(stream, this);
                }
            }
            catch (Exception e)
            {
                DebugLog.Exception(e);
            }
            finally
            {
                Saving = false;
            }
        }

        public static void Load()
        {
            UserProfile profile = null;

            try
            {
                if (RoamingStorage.FileExists("Ensign Computer/player.save"))
                {
                    using (var stream = RoamingStorage.OpenRead("Ensign Computer/player.save"))
                    {
                        var bf = new BinaryFormatter();
                        profile = bf.Deserialize(stream) as UserProfile;
                    }
                }
            }
            catch (Exception e)
            {
                DebugLog.Exception(e);
            }

            if (profile == null)
                profile = new UserProfile();

            profile.Saving = false;
            Instance = profile;
        }

        private void ProfileSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveToFile();
        }
    }
}