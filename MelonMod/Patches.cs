using MelonLoader;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl
{
    internal static class Patches
    {
        internal static MelonLogger.Instance CCLogger;

        internal static void PreInit(HarmonyLib.Harmony HarmonyInstance)
        {
            CCLogger = new MelonLogger.Instance("CC", ConsoleColor.Yellow);

            Type patchesType = typeof(Patches);

            ModCore.Logger.Msg("Patching CrowdControl...");
            Type cctype = typeof(CrowdControl);
            HarmonyInstance.Patch(cctype.GetMethod("Log", BindingFlags.Public | BindingFlags.Static), patchesType.GetMethod("Log", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
            HarmonyInstance.Patch(cctype.GetMethod("LogWarning", BindingFlags.Public | BindingFlags.Static), patchesType.GetMethod("LogWarning", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
            HarmonyInstance.Patch(cctype.GetMethod("LogError", BindingFlags.Public | BindingFlags.Static), patchesType.GetMethod("LogError", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
            HarmonyInstance.Patch(cctype.GetMethod("LogException", BindingFlags.Public | BindingFlags.Static), patchesType.GetMethod("LogException", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
        }

        internal static unsafe void Init(HarmonyLib.Harmony HarmonyInstance)
        {
            try
            {
                Type patchesType = typeof(Patches);

                ModCore.Logger.Msg("Patching PlayerPrefs...");

                Type PlayerPrefsType = typeof(PlayerPrefs);
                HarmonyInstance.Patch(PlayerPrefsType.GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => x.Name.Equals("GetString") && (x.GetParameters().Count() == 2)), patchesType.GetMethod("PlayerPrefs_GetString", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
                HarmonyInstance.Patch(PlayerPrefsType.GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => x.Name.Equals("SetString") && (x.GetParameters().Count() == 2)), patchesType.GetMethod("PlayerPrefs_SetString", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
            }
            catch (Exception ex)
            {
                if (MLCC_Config.Debug.Value)
                    MelonLogger.Error($"Failed to Patch PlayerPrefs: {ex}");
            }
        }

        private static bool Log(string __0)
        {
            if (MLCC_Config.Debug?.Value == true)
                CCLogger.Msg(__0);
            return false;
        }

        private static bool LogWarning(string __0)
        {
            if (MLCC_Config.Debug?.Value == true)
                CCLogger.Warning(__0);
            return false;
        }

        private static bool LogError(string __0)
        {
            if (MLCC_Config.Debug?.Value == true)
                CCLogger.Error(__0);
            return false;
        }

        private static bool LogException(Exception __0)
        {
            if (MLCC_Config.Debug?.Value == true)
                CCLogger.Error(__0);
            return false;
        }

        private static bool PlayerPrefs_GetString(string __0, string __1, ref string __result)
        {
            if (!string.IsNullOrEmpty(__0) && __0.ToLowerInvariant().StartsWith("cctoken"))
            {
                if (string.IsNullOrEmpty(MLCC_Config.AuthToken.Value))
                    __result = MLCC_Config.AuthToken.Value = __1;
                else
                    __result = MLCC_Config.AuthToken.Value;
                return false;
            }
            return true;
        }

        private static bool PlayerPrefs_SetString(string __0, string __1)
        {
            if (!string.IsNullOrEmpty(__0) && __0.ToLowerInvariant().StartsWith("cctoken"))
            {
                if (string.IsNullOrEmpty(__1))
                    MLCC_Config.AuthToken.Value = string.Empty;
                else
                    MLCC_Config.AuthToken.Value = __1;
            }
            return true;
        }

        private static bool NullOriginal() => false;
    }
}
