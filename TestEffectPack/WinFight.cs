using ML_CrowdControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
    ID = 28,
    Name = "Win Fight"
    )]
    class WinFight : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
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
