using System.IO;
using MelonLoader;

namespace ML_CrowdControl
{
    internal static class Config
    {
        internal static void Setup(string folderpath)
        {
            ModCore.Logger.Msg("Loading Config...");
            string filepath = Path.Combine(folderpath, "Config.cfg");
            General.Setup(filepath);
            if (!File.Exists(filepath))
                General.SaveToFile(false);
        }

        internal static class General
        {
            private static MelonPreferences_Category Category;

            internal static MelonPreferences_Entry<bool> Debug;
            internal static MelonPreferences_Entry<short> ReconnectRetryCount;
            internal static MelonPreferences_Entry<float> ReconnectRetryDelay;
            internal static MelonPreferences_Entry<float> DelayBetweenEffects;
            internal static MelonPreferences_Entry<string> AuthToken;

            internal static void Setup(string filepath)
            {
                Category = MelonPreferences.CreateCategory("General", "Crowd Control - General");
                Category.SetFilePath(filepath, true, false);

                Debug = Category.CreateEntry("Debug", false, description: "Enables Debug Logs");
                ReconnectRetryCount = Category.CreateEntry<short>("ReconnectRetryCount", -1, description: "How many times to attempt reconnecting? (-1 for unlimited)");
                ReconnectRetryDelay = Category.CreateEntry("ReconnectRetryDelay", 5f, description: "How many seconds to wait until trying to automatically reconnect again?");
                DelayBetweenEffects = Category.CreateEntry("DelayBetweenEffects", .5f, description: "Time to wait after triggering an effect before attempting to trigger another.");
                AuthToken = Category.CreateEntry("AuthToken", string.Empty, description: "Private User Authentication Token provided from Warp World.");
            }

            internal static void SaveToFile(bool printmsg = true)
                => Category.SaveToFile(printmsg);
        }
    }
}
