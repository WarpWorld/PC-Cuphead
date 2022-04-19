using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl
{
    internal static class Extensions
    {
        private static FieldInfo _gameKey;
        private static FieldInfo _token;
        private static FieldInfo _dontDestroyOnLoad;
        private static FieldInfo _reconnectRetryCount;
        private static FieldInfo _reconnectRetryDelay;

        private static FieldInfo parameters;

        private static FieldInfo bidFor;
        private static FieldInfo cost;

        static Extensions()
        {
            Type cctype = typeof(CrowdControl);

            _dontDestroyOnLoad = cctype.GetField(nameof(_dontDestroyOnLoad), BindingFlags.NonPublic | BindingFlags.Instance);
            _gameKey = cctype.GetField(nameof(_gameKey), BindingFlags.NonPublic | BindingFlags.Instance);
            _token = cctype.GetField(nameof(_token), BindingFlags.NonPublic | BindingFlags.Instance);
            _reconnectRetryCount = cctype.GetField(nameof(_reconnectRetryCount), BindingFlags.NonPublic | BindingFlags.Instance);
            _reconnectRetryDelay = cctype.GetField(nameof(_reconnectRetryDelay), BindingFlags.NonPublic | BindingFlags.Instance);

            Type paramType = typeof(CCEffectParameters);
            parameters = paramType.GetField(nameof(parameters), BindingFlags.NonPublic | BindingFlags.Instance);

            Type bidWarType = typeof(CCEffectBidWar);
            bidFor = bidWarType.GetField(nameof(bidFor), BindingFlags.NonPublic | BindingFlags.Instance);
            cost = bidWarType.GetField(nameof(cost), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal static bool GetDontDestroyOnLoad(this CrowdControl instance)
            => (bool)_dontDestroyOnLoad.GetValue(instance);
        internal static void SetDontDestroyOnLoad(this CrowdControl instance, bool val)
            => _dontDestroyOnLoad.SetValue(instance, val);

        internal static uint GetGameKey(this CrowdControl instance)
            => (uint)_gameKey.GetValue(instance);
        internal static void SetGameKey(this CrowdControl instance, uint val)
            => _gameKey.SetValue(instance, val);

        internal static string GetToken(this CrowdControl instance)
            => (string)_token.GetValue(instance);
        internal static void SetToken(this CrowdControl instance, string val)
            => _token.SetValue(instance, val);

        internal static short GetReconnectRetryCount(this CrowdControl instance)
            => (short)_reconnectRetryCount.GetValue(instance);
        internal static void SetReconnectRetryCount(this CrowdControl instance, short val)
            => _reconnectRetryCount.SetValue(instance, val);

        internal static float GetReconnectRetryDelay(this CrowdControl instance)
            => (float)_reconnectRetryDelay.GetValue(instance);
        internal static void SetReconnectRetryDelay(this CrowdControl instance, float val)
            => _reconnectRetryDelay.SetValue(instance, val);


        internal static List<string> GetParameters(this CCEffectParameters instance)
            => (List<string>)parameters.GetValue(instance);
        internal static void SetParameters(this CCEffectParameters instance, List<string> val)
            => parameters.SetValue(instance, val);


        internal static string GetBidFor(this CCEffectBidWar instance)
            => (string)bidFor.GetValue(instance);
        internal static void SetBidFor(this CCEffectBidWar instance, string val)
            => bidFor.SetValue(instance, val);
        internal static uint GetCost(this CCEffectBidWar instance)
            => (uint)cost.GetValue(instance);
        internal static void SetCost(this CCEffectBidWar instance, uint val)
            => cost.SetValue(instance, val);
    }
}
