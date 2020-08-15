using System;
using Chroma.Audio;
using Chroma.ContentManagement;
using Chroma.Graphics;
using Chroma.Windowing;
using Commodore.Framework.Generators;
using Commodore.Framework.Managers;

namespace Commodore.Framework
{
    public static class G
    {
        private static AudioManager _audioManager;
        private static GraphicsManager _graphicsSettings;
        private static IContentProvider _contentProvider;
        private static DebugManager _debugManager;
        private static SettingsManager _settingsManager;
        private static MersenneTwister _random;

        public static Window Window { get; set; }

        public static GraphicsManager GraphicsSettings
        {
            get => _graphicsSettings ?? throw new InvalidOperationException("Graphics device manager has not been initialized yet.");
            set
            {
                if (_graphicsSettings != null)
                    throw new InvalidOperationException("Graphics device manager has already been initialized.");

                _graphicsSettings = value;
            }
        }

        public static IContentProvider ContentProvider
        {
            get => _contentProvider ?? throw new InvalidOperationException("Content provider has not been initialized yet.");
            set
            {
                if (_contentProvider != null)
                    throw new InvalidOperationException("Content provider has already been initialized.");

                _contentProvider = value;
            }
        }

        public static DebugManager DebugManager
        {
            get => _debugManager ?? throw new InvalidOperationException("Debug manager has not been initialized yet.");
            set
            {
                if (_debugManager != null)
                    throw new InvalidOperationException("Debug manager has already been initialized.");

                _debugManager = value;
            }
        }

        public static SettingsManager SettingsManager
        {
            get => _settingsManager ?? throw new InvalidOperationException("Settings manager has not been initialized yet.");
            set
            {
                if (_settingsManager != null)
                    throw new InvalidOperationException("Settings manager has already been initialized.");

                _settingsManager = value;
            }
        }

        public static AudioManager AudioManager
        {
            get => _audioManager ?? throw new InvalidOperationException("Audio manager has not been initialized yet.");
            set
            {
                if (_audioManager != null)
                    throw new InvalidOperationException("Audio manager has already been initialized.");

                _audioManager = value;
            }
        }

        public static MersenneTwister Random { get; } = _random ?? (_random = new MersenneTwister());
    }
}
