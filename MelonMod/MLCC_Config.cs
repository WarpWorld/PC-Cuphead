using System.IO;
using MelonLoader;

namespace ML_CrowdControl
{
    /// <summary>Class for Mod Config.</summary>
    public static class MLCC_Config
    {
        /// <summary>General Category of the Config.</summary>
        public static MelonPreferences_Category GeneralCategory { get; private set; }

        /// <summary>Toggle for Debug Logs.</summary>
        public static MelonPreferences_Entry<bool> Debug { get; private set; }
        /// <summary>Toggle for Auto-Connecting on Launch.</summary>
        public static MelonPreferences_Entry<bool> AutoConnect { get; private set; }
        /// <summary>How many times to attempt reconnecting? (-1 for unlimited  |  0 to disable reconnecting)</summary>
        public static MelonPreferences_Entry<short> ReconnectRetries { get; private set; }
        /// <summary>Time to wait after triggering an effect before attempting to trigger another.</summary>
        public static MelonPreferences_Entry<float> DelayBetweenEffects { get; private set; }

        internal static MelonPreferences_Entry<string> AuthToken { get; private set; }

        internal static void Setup(string folderpath)
        {
            ModCore.Logger.Msg("Loading Config...");
            string filepath = Path.Combine(folderpath, "Config.cfg");

            GeneralCategory = MelonPreferences.CreateCategory("General", "Crowd Control - General");
            GeneralCategory.SetFilePath(filepath, true, false);

            Debug = GeneralCategory.CreateEntry("Debug", false, description: "Toggle for Debug Logs");
            AutoConnect = GeneralCategory.CreateEntry("AutoConnect", false, description: "Toggle for Auto-Connecting on Launch");
            ReconnectRetries = GeneralCategory.CreateEntry<short>("ReconnectRetries", 3, description: "How many times to attempt reconnecting? (-1 for unlimited  |  0 to disable reconnecting)");
            DelayBetweenEffects = GeneralCategory.CreateEntry("DelayBetweenEffects", .5f, description: "Time to wait after triggering an effect before attempting to trigger another.");

            AuthToken = GeneralCategory.CreateEntry("AuthToken", string.Empty, description: "Private User Authentication Token provided from Warp World's Server API.");

            if (!File.Exists(filepath))
                GeneralCategory.SaveToFile(false);
        }
    }
}
