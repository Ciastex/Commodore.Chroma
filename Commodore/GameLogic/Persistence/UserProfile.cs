using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Timers;
using Chroma.Input;
using Commodore.Framework;
using Commodore.Framework.Extensions;
using Commodore.Framework.Generators;
using Commodore.Framework.Persistence.AppData;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Network;

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

        public MersenneTwister Random { get; set; } = new MersenneTwister();
        
        public Directory RootDirectory { get; set; } = new Directory();
        public Internet Internet { get; set; } = new Internet();

        public string Username { get; set; } = "lazarus";
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
            var homeDirectory = RootDirectory.AddNewDirectory("home");
            RootDirectory.AddNewDirectory("lib");

            homeDirectory.AddNewFile("fs.docs").SetData(
                G.ContentProvider.Read("Text/Docs/fs.txt")
            );
            
            PopulateBaseBinDirectory();
            PopulateBaseEtcDirectory();
        }

        private void PopulateBaseBinDirectory()
        {
            var binDirectory = RootDirectory.AddNewDirectory("bin");
            var fileNames = G.ContentProvider.GetContentFileNames("Sources/BaseBin");

            foreach (var fileName in fileNames)
            {
                binDirectory.AddNewFile(fileName, FileAttributes.Executable).SetData(
                    G.ContentProvider.Read("Sources/BaseBin/" + fileName)
                );
            }
        }

        private void PopulateBaseEtcDirectory()
        {
            var etcDirectory = RootDirectory.AddNewDirectory("etc");
            var fileNames = G.ContentProvider.GetContentFileNames("Sources/BaseEtc");

            foreach (var fileName in fileNames)
            {
                etcDirectory.AddNewFile(fileName).SetData(
                    G.ContentProvider.Read("Sources/BaseEtc/" + fileName)
                );
            }
        }
    }
}