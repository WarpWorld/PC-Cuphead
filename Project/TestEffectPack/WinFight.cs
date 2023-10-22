using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System.Reflection;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "WinFight",
        Name = "Win Fight",
        Description = "Instantly wins the current boss level!",
        Price = 1000,
        Morality = Morality.Good
    )]
    class WinFight : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            try
            {
                var p = Level.Current.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                var prop = p.GetValue(Level.Current);
                MethodInfo m = prop.GetType().GetMethod("WinInstantly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                m.Invoke(prop, new object[] { });
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
