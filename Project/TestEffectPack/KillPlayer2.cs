using System;
using System.Reflection;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "KillPlayer2",
        Name = "Kill Player 2",
        Description = "Kills the player from any health",
        Price = 500,
        Categories = new string[] { "Player 2" },
        Morality = Morality.Evil
    )]
    class KillPlayer2 : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;

            try
            {
                if (PlayerStatsManager.DebugInvincible) return EffectResult.Retry;

                    int h = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.Health;
                    PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.SetHealth(0);
                    MethodInfo m = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.GetType().GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    m.Invoke(PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats, new object[] { });
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
