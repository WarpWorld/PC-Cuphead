using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using MelonLoader;
using System.IO;
using System.Net.Sockets;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using ML_CrowdControl.Wrappers;
using WarpWorld.CrowdControl;
#if ML_Il2Cpp
using UnhollowerRuntimeLib;
#endif

namespace ML_CrowdControl
{
    /// <summary>Effects Manager for the Mod.</summary>
    public static class MLCC_EffectsManager
    {
        internal static string BaseFolder;
        private static GameObject gameObject;
        internal static MLCC_UI ui;

        /// <summary>Array of all loaded effects casted as MLCC_EffectBase.</summary>
        public static MLCC_EffectBase[] Effects { get; private set; } = new MLCC_EffectBase[0];

        internal static void LinkManagerEvents()
        {
            ModCore.ccinstance.OnConnecting += () => RunMethod(x => x.OnConnecting(), x => x.OnConnecting());
            ModCore.ccinstance.OnConnectionError += (SocketError error) => RunMethod(x => x.OnConnectionError(error), x => x.OnConnectionError(error));
            ModCore.ccinstance.OnConnected += () => RunMethod(x => x.OnConnected(), x => x.OnConnected());
            ModCore.ccinstance.OnAuthenticated += () => RunMethod(x => x.OnAuthenticated(), x => x.OnAuthenticated());
            ModCore.ccinstance.OnDisconnected += () => RunMethod(x => x.OnDisconnected(), x => x.OnDisconnected());
            ModCore.ccinstance.OnDisplayMessage += (string msg) => RunMethod(x => x.OnMessage(msg), x => x.OnMessage(msg));

            ModCore.ccinstance.OnTempTokenFailure += () => RunMethod(x => x.OnAuthenticationFailed(), x => x.OnAuthenticationFailed());
            ModCore.ccinstance.OnToggleTokenView += (bool isShown) => {
                if (ui != null)
                    ui.OnAuthenticationPrompt(isShown);
                else if (isShown)
                    ModCore.AskForAuthToken();
            };
        }

        internal static void LoadUI()
        {
            string filepath = Path.Combine(ModCore.BaseFolder, "UI.dll");
            if (!File.Exists(filepath))
                return;

            ModCore.Logger.Msg("Found UI.dll");

            Assembly assembly = null;
            try { assembly = Assembly.LoadFrom(filepath); }
            catch (Exception ex)
            {
                ModCore.Logger.Error($"Failed to Load Assembly for {filepath}: {ex}");
                return;
            }
            if (assembly == null)
            {
                ModCore.Logger.Error($"Failed to Load Assembly for {filepath}: Assembly.LoadFrom returned null"); ;
                return;
            }

            Type uiType = assembly.GetValidTypes(x => x.IsSubclassOf(typeof(MLCC_UI))).FirstOrDefault();
            if (uiType == null)
            {
                ModCore.Logger.Error($"No Subclass of MLCC_UI found in {filepath}");
                return;
            }

            ModCore.Logger.Msg("Creating UI Interface...");
            MLCC_UIWrapper wrapper = AddWrapper<MLCC_UIWrapper>(ModCore.gameObject);
            wrapper.ui = ui = Activator.CreateInstance(uiType) as MLCC_UI;
            wrapper.ui.Logger = new MelonLogger.Instance("CC.UI", ConsoleColor.Blue);

            RunMethodUI(x => x.OnLoad());
        }

        internal static void LoadEffects()
        {
            BaseFolder = Path.Combine(ModCore.BaseFolder, "Effects");
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);

            ModCore.Logger.Msg("Creating Effects GameObject...");
            gameObject = new GameObject();
            GameObject.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            gameObject.transform.SetParent(ModCore.gameObject.transform);
            gameObject.SetActive(true);

            ModCore.Logger.Msg("Loading Effects...");
            List<MLCC_EffectBase> loadedEffects = new List<MLCC_EffectBase>();
            LemonEnumerator<string> filesEnumerator = new LemonEnumerator<string>(Directory.GetFiles(BaseFolder).Where(x => Path.GetExtension(x).ToLowerInvariant().Equals(".dll")).ToArray());
            while (filesEnumerator.MoveNext())
            {
                string filepath = filesEnumerator.Current;
                if (string.IsNullOrEmpty(filepath))
                    continue;

                Assembly assembly = null;
                try { assembly = Assembly.LoadFrom(filepath); }
                catch (Exception ex)
                {
                    ModCore.Logger.Error($"Failed to Load Assembly for {filepath}: {ex}");
                    continue;
                }
                if (assembly == null)
                {
                    ModCore.Logger.Error($"Failed to Load Assembly for {filepath}: Assembly.LoadFrom returned null"); ;
                    continue;
                }

                IEnumerable<Type> effect_types = assembly.GetValidTypes(x => 
                    x.IsSubclassOf(typeof(MLCC_Effect)) 
                    || x.IsSubclassOf(typeof(MLCC_TimedEffect))
                    || x.IsSubclassOf(typeof(MLCC_BidWarEffect))
                    || x.IsSubclassOf(typeof(MLCC_ParamEffect)));
                if (effect_types.Count() <= 0)
                {
                    ModCore.Logger.Error($"No Effect Types Found in {filepath}");
                    continue;
                }

                LemonEnumerator<Type> typeEnumerator = new LemonEnumerator<Type>(effect_types.ToArray());
                while (typeEnumerator.MoveNext())
                {
                    Type type = typeEnumerator.Current;
                    if (type == null)
                        continue;

                    object dataAttribute = type.GetCustomAttributes(true).FirstOrDefault(x => ((x.GetType() == typeof(MLCC_EffectData)) || x.GetType().IsSubclassOf(typeof(MLCC_EffectData))));
                    if (dataAttribute == null)
                    {
                        ModCore.Logger.Error($"No Effect Data Attribute on {type.FullName}");
                        continue;
                    }
                    Type dataAttributeType = dataAttribute.GetType();

                    if (type.IsSubclassOf(typeof(MLCC_Effect))) // Normal
                    {
                        if (dataAttributeType != typeof(MLCC_EffectData))
                        {
                            ModCore.Logger.Error($"Incorrect Effect Data Attribute on {type.FullName}");
                            ModCore.Logger.Error($"Expected {nameof(MLCC_EffectData)} got {dataAttributeType.Name}");
                            continue;
                        }

                        MLCC_EffectData effectData = (MLCC_EffectData)dataAttribute;
                        if (effectData.ID <= 0)
                        {
                            ModCore.Logger.Error($"{type.FullName} has Invalid ID! Must be greater than or equal to 1!");
                            continue;
                        }
                        if (string.IsNullOrEmpty(effectData.Name))
                        {
                            ModCore.Logger.Error($"{type.FullName} has no Name!");
                            continue;
                        }
                        if (!IsIDUnique(effectData.ID))
                        {
                            ModCore.Logger.Error($"ID of {effectData.ID} on {type.FullName} is Already Registered!");
                            continue;
                        }

                        MLCC_EffectWrapper wrapper = AddWrapper<MLCC_EffectWrapper>(gameObject);
                        effectData.ApplyData(wrapper);

                        wrapper.effect = Activator.CreateInstance(type) as MLCC_Effect;
                        wrapper.effect.Logger = new MelonLogger.Instance(wrapper.displayName);
                        wrapper.effect.DataAttribute = effectData;
                        wrapper.effect.Wrapper = wrapper;

                        ModCore.ccinstance.RegisterEffect(wrapper);
                        loadedEffects.Add(wrapper.effect);

                        if (MLCC_Config.Debug.Value)
                            ModCore.Logger.Msg($"Registered {type.FullName} : {wrapper.displayName} : {wrapper.identifier}");
                    }
                    else if (type.IsSubclassOf(typeof(MLCC_TimedEffect))) // Timed
                    {
                        if (dataAttributeType != typeof(MLCC_TimedEffectData))
                        {
                            ModCore.Logger.Error($"Incorrect Effect Data Attribute on {type.FullName} in {filepath}");
                            ModCore.Logger.Error($"Expected {nameof(MLCC_TimedEffectData)} got {dataAttributeType.Name}");
                            continue;
                        }

                        MLCC_TimedEffectData effectData = (MLCC_TimedEffectData)dataAttribute;
                        if (effectData.ID <= 0)
                        {
                            ModCore.Logger.Error($"{type.FullName} has Invalid ID! Must be greater than or equal to 1!");
                            continue;
                        }
                        if (!IsIDUnique(effectData.ID))
                        {
                            ModCore.Logger.Error($"ID of {effectData.ID} on {type.FullName} is Already Registered!");
                            continue;
                        }
                        if (string.IsNullOrEmpty(effectData.Name))
                        {
                            ModCore.Logger.Error($"{type.FullName} has no Name!");
                            continue;
                        }

                        MLCC_TimedEffectWrapper wrapper = AddWrapper<MLCC_TimedEffectWrapper>(gameObject);
                        effectData.ApplyData(wrapper);

                        wrapper.effect = Activator.CreateInstance(type) as MLCC_TimedEffect;
                        wrapper.effect.Logger = new MelonLogger.Instance(wrapper.displayName);
                        wrapper.effect.DataAttribute = effectData;
                        wrapper.effect.Wrapper = wrapper;

                        ModCore.ccinstance.RegisterEffect(wrapper);
                        loadedEffects.Add(wrapper.effect);

                        if (MLCC_Config.Debug.Value)
                            ModCore.Logger.Msg($"Registered {type.FullName} : {wrapper.displayName} : {wrapper.identifier}");
                    }
                    else if (type.IsSubclassOf(typeof(MLCC_BidWarEffect))) // Bid War
                    {
                        if (dataAttributeType != typeof(MLCC_BidWarEffectData))
                        {
                            ModCore.Logger.Error($"Incorrect Effect Data Attribute on {type.FullName} in {filepath}");
                            ModCore.Logger.Error($"Expected {nameof(MLCC_BidWarEffectData)} got {dataAttributeType.Name}");
                            continue;
                        }

                        MLCC_BidWarEffectData effectData = (MLCC_BidWarEffectData)dataAttribute;
                        if (effectData.ID <= 0)
                        {
                            ModCore.Logger.Error($"{type.FullName} has Invalid ID! Must be greater than or equal to 1!");
                            continue;
                        }
                        if (!IsIDUnique(effectData.ID))
                        {
                            ModCore.Logger.Error($"ID of {effectData.ID} on {type.FullName} is Already Registered!");
                            continue;
                        }
                        if (string.IsNullOrEmpty(effectData.Name))
                        {
                            ModCore.Logger.Error($"{type.FullName} has no Name!");
                            continue;
                        }

                        MLCC_BidWarEffectWrapper wrapper = AddWrapper<MLCC_BidWarEffectWrapper>(gameObject);
                        effectData.ApplyData(wrapper);

                        wrapper.effect = Activator.CreateInstance(type) as MLCC_BidWarEffect;
                        wrapper.effect.Logger = new MelonLogger.Instance(wrapper.displayName);
                        wrapper.effect.DataAttribute = effectData;
                        wrapper.effect.Wrapper = wrapper;

                        ModCore.ccinstance.RegisterEffect(wrapper);
                        loadedEffects.Add(wrapper.effect);

                        if (MLCC_Config.Debug.Value)
                            ModCore.Logger.Msg($"Registered {type.FullName} : {wrapper.displayName} : {wrapper.identifier}");
                    }
                    else if (type.IsSubclassOf(typeof(MLCC_ParamEffect))) // Parameter
                    {
                        if (dataAttributeType != typeof(MLCC_ParamEffectData))
                        {
                            ModCore.Logger.Error($"Incorrect Effect Data Attribute on {type.FullName} in {filepath}");
                            ModCore.Logger.Error($"Expected {nameof(MLCC_ParamEffectData)} got {dataAttributeType.Name}");
                            continue;
                        }

                        MLCC_ParamEffectData effectData = (MLCC_ParamEffectData)dataAttribute;
                        if (effectData.ID <= 0)
                        {
                            ModCore.Logger.Error($"{type.FullName} has Invalid ID! Must be greater than or equal to 1!");
                            continue;
                        }
                        if (!IsIDUnique(effectData.ID))
                        {
                            ModCore.Logger.Error($"ID of {effectData.ID} on {type.FullName} is Already Registered!");
                            continue;
                        }
                        if (string.IsNullOrEmpty(effectData.Name))
                        {
                            ModCore.Logger.Error($"{type.FullName} has no Name!");
                            continue;
                        }

                        MLCC_ParamEffectWrapper wrapper = AddWrapper<MLCC_ParamEffectWrapper>(gameObject);
                        effectData.ApplyData(wrapper);

                        wrapper.effect = Activator.CreateInstance(type) as MLCC_ParamEffect;
                        wrapper.effect.Logger = new MelonLogger.Instance(wrapper.displayName);
                        wrapper.effect.DataAttribute = effectData;
                        wrapper.effect.Wrapper = wrapper;

                        ModCore.ccinstance.RegisterEffect(wrapper);
                        loadedEffects.Add(wrapper.effect);

                        if (MLCC_Config.Debug.Value)
                            ModCore.Logger.Msg($"Registered {type.FullName} : {wrapper.displayName} : {wrapper.identifier}");
                    }
                }
            }

            ModCore.Logger.Msg($"{loadedEffects.Count} Effects Loaded!");
            Effects = loadedEffects.ToArray();
            RunMethod(x => x.OnLoad());
        }

        private static T AddWrapper<T>(GameObject gameObject) where T : Component
        {
#if ML_Il2Cpp
            T wrapper = gameObject.AddComponent(Il2CppType.Of<T>()).TryCast<T>();
#else
            T wrapper = (T)gameObject.AddComponent(typeof(T));
#endif
            return wrapper;
        }

        private static bool IsIDUnique(uint id)
        {
            LemonEnumerator<MLCC_EffectBase> enumerator = new LemonEnumerator<MLCC_EffectBase>(Effects.ToArray());
            while (enumerator.MoveNext())
                if ((enumerator.Current != null) && (enumerator.Current.GetID() == id))
                    return false;
            return true;
        }

        internal static void RunMethodUI(Action<MLCC_UI> method)
        {
            if (ui == null)
                return;
            method(ui);
        }

        internal static void RunMethod(Action<MLCC_EffectBase> method, Action<MLCC_UI> method_ui = null)
        {
            if (method_ui != null)
                RunMethodUI(method_ui);

            LemonEnumerator<MLCC_EffectBase> enumerator = new LemonEnumerator<MLCC_EffectBase>(Effects);
            while (enumerator.MoveNext())
                if (enumerator.Current != null)
                    method(enumerator.Current);
        }
    }
}
