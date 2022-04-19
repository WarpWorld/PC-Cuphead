using System;
using System.IO;
using System.Net.Sockets;
using MelonLoader;
using UnityEngine;
using WarpWorld.CrowdControl;
#if ML_Il2Cpp
using UnhollowerRuntimeLib;
using ML_CrowdControl.Wrappers;
#endif

namespace ML_CrowdControl
{
    internal class ModCore : MelonMod
    {
        internal static MelonLogger.Instance Logger;

        internal static string BaseFolder;
        internal static uint GameKey = 92;

        internal static GameObject gameObject;
        internal static CrowdControl ccinstance;

        public override void OnPreSupportModule()
        {
            Logger = LoggerInstance;

#if !ML_Il2Cpp
            // DO NOT REMOVE! Dumb Fix for MonoBehaviour Resolving on Mono & MonoBleedingEdge
            System.Reflection.Assembly.Load(typeof(CrowdControl).Assembly.GetName().Name);
#endif

            Patches.PreInit(HarmonyInstance);
        }

        public override void OnApplicationStart()
        {
            BaseFolder = Path.Combine(MelonUtils.BaseDirectory, "CrowdControl");
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);

            MLCC_Config.Setup(BaseFolder);

            Patches.Init(HarmonyInstance);

#if ML_Il2Cpp
            // CrowdControl
            ClassInjector.RegisterTypeInIl2Cpp<CCEffectBase>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<CCEffect>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<CCEffectTimed>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<CCEffectBidWar>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<CCEffectParameters>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<CrowdControl>(MLCC_Config.Debug.Value);

            // ML.CC
            ClassInjector.RegisterTypeInIl2Cpp<MLCC_UIWrapper>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<MLCC_EffectWrapper>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<MLCC_TimedEffectWrapper>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<MLCC_BidWarEffectWrapper>(MLCC_Config.Debug.Value);
            ClassInjector.RegisterTypeInIl2Cpp<MLCC_ParamEffectWrapper>(MLCC_Config.Debug.Value);
#endif
        }

        public override void OnApplicationLateStart()
        {
            Logger.Msg("Creating Root GameObject...");
            gameObject = new GameObject();
            GameObject.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            SetupManager();
            MLCC_EffectsManager.LinkManagerEvents();
            MLCC_EffectsManager.LoadUI();
            MLCC_EffectsManager.LoadEffects();

            if (MLCC_EffectsManager.ui == null)
            {
                Logger.Msg("No UI Interface. Forcing Auto-Connect on Launch...");
                ccinstance.Connect();
            }
            else
            {

                if (MLCC_Config.AutoConnect.Value)
                    ccinstance.Connect();
            }
        }

        public override void OnApplicationQuit()
            => CrowdControl.instance.Disconnect();

        internal static void AskForAuthToken()
        {
            Logger.Msg("Please specify your Auth Token given from \"https://crowdcontrol.live/activate\"");
            string AuthToken = Console.ReadLine();

            if (string.IsNullOrEmpty(AuthToken))
            {
                Logger.Error("No Auth Token Specified!");
                CrowdControl.instance.Disconnect();
                return;
            }

            CrowdControl.instance.SubmitTempToken(AuthToken);
        }

        private static void SetupManager()
        {
            Logger.Msg("Creating Manager...");

            ccinstance =
#if ML_Il2Cpp
            gameObject.AddComponent(Il2CppType.Of<CrowdControl>()).TryCast<CrowdControl>();
#else
            (CrowdControl)gameObject.AddComponent(typeof(CrowdControl));
#endif

            ccinstance.OnConnecting += () => Logger.Msg("Connecting...");
            ccinstance.OnConnected += () => Logger.Msg("Connected!");
            ccinstance.OnAuthenticated += () => { Logger.Msg("Authentication Successful!"); };
            ccinstance.OnConnectionError += (SocketError error) => Logger.Error($"Connection Error: {Enum.GetName(typeof(SocketError), error)}");
            ccinstance.OnTempTokenFailure += () => Logger.Error("Invalid Auth Token!");
            ccinstance.OnDisconnected += () => Logger.Msg("Disconnected!");

            ccinstance.SetDontDestroyOnLoad(true);

            try
            {
                string gamekey_filepath = Path.Combine(BaseFolder, "GameKey.txt");
                if (File.Exists(gamekey_filepath))
                {
                    string gamekey_text = File.ReadAllText(gamekey_filepath);
                    if (!string.IsNullOrEmpty(gamekey_text))
                        uint.TryParse(gamekey_text, out GameKey);
                }
            }
            catch (Exception ex) { Logger.Error($"Exception while reading GameKey.txt from File: {ex}"); }
            Logger.Msg($"Setting GameKey to {GameKey}");
            ccinstance.SetGameKey(GameKey);

            ccinstance.SetToken(MLCC_Config.AuthToken.Value);

            ccinstance.SetReconnectRetryCount(MLCC_Config.ReconnectRetries.Value);
            MLCC_Config.ReconnectRetries.OnValueChanged += (oldval, newval) => ccinstance.SetReconnectRetryCount(newval);

            ccinstance.delayBetweenEffects = MLCC_Config.DelayBetweenEffects.Value;
            MLCC_Config.DelayBetweenEffects.OnValueChanged += (oldval, newval) => ccinstance.delayBetweenEffects = newval;

            Logger.Msg("Created Manager!");
        }
    }
}