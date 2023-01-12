using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Timers;
using Chroma.Input;
using Commodore.Framework;
using Commodore.Framework.Persistence.AppData;
using Commodore.GameLogic.Core.IO.Storage;

namespace Commodore.GameLogic.Persistence
{
    [Serializable]
    public class UserProfile
    {
        [NonSerialized] private Timer _profileSaveTimer;

        public static UserProfile Instance { get; private set; }

        private UserProfile()
        {
        }

        private bool _autoSaveActive;

        public bool IsInitialized { get; set; }
        public bool Saving { get; private set; }

        public string UserName { get; set; } = "lazarus";
        public Directory RootDirectory { get; set; } = new Directory();

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

        public void SaveToFile()
        {
            if (Saving)
            {
                return;
            }

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
            catch
            {
            }
            finally
            {
                Saving = false;
            }
        }

        public static void Load()
        {
            UserProfile profile = null;

            if (RoamingStorage.FileExists("Ensign Computer/player.save"))
            {
                using (var stream = RoamingStorage.OpenRead("Ensign Computer/player.save"))
                {
                    var bf = new BinaryFormatter();
                    profile = bf.Deserialize(stream) as UserProfile;
                }
            }

            if (profile == null)
                profile = new UserProfile();

            profile.Saving = false;
            Instance = profile;
        }

        private void ProfileSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
            => SaveToFile();

        public void CreateBaseFileSystem()
        {
            var binDirectory = RootDirectory.AddNewDirectory("bin");
            var homeDirectory = RootDirectory.AddNewDirectory("home");
            RootDirectory.AddNewDirectory("lib");

            homeDirectory.AddNewFile("fs.docs").SetData(
                G.ContentProvider.Read("Text/Docs/fs.txt")
            );

            binDirectory.AddNewFile("ls", FileAttributes.Executable).SetData(
                G.ContentProvider.Read("Sources/BasePrograms/ls")
            );

            binDirectory.AddNewFile("cat", FileAttributes.Executable).SetData(
                G.ContentProvider.Read("Sources/BasePrograms/cat")
            );

            binDirectory.AddNewFile("cp", FileAttributes.Executable).SetData(
                G.ContentProvider.Read("Sources/BasePrograms/cp")
            );

            binDirectory.AddNewFile("rm", FileAttributes.Executable).SetData(
                G.ContentProvider.Read("Sources/BasePrograms/rm")
            );

            binDirectory.AddNewFile("cd", FileAttributes.Executable).SetData(
                G.ContentProvider.Read("Sources/BasePrograms/cd")
            );

            binDirectory.AddNewFile("help", FileAttributes.Executable).SetData(
                G.ContentProvider.Read("Sources/BasePrograms/help")
            );
        }
    }
}