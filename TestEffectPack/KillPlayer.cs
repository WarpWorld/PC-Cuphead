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
        ID = 29,
        Name = "Kill Player"
    )]
    class KillPlayer : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            try
            {
                if (PlayerStatsManager.DebugInvincible) return EffectResult.Retry;

                int h = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.Health;
                PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.SetHealth(0);
                MethodInfo m = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.GetType().GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                m.Invoke(PlayerManager.GetPlayer(PlayerId.PlayerOne).stats, new object[] { });
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
