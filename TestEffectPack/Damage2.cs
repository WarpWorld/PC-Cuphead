using ML_CrowdControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = 59,
        Name = "Damage P2"
    )]
    class Damage2 : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;

            try
            {
                if (PlayerStatsManager.DebugInvincible) return EffectResult.Retry;
                if (Base.isMausoleum()) return EffectResult.Retry;


                int h = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.Health;
                if (h > 1)
                {
                    DamageDealer.DamageInfo di = new DamageDealer.DamageInfo(1.0F, DamageDealer.Direction.Neutral, PlayerManager.GetPlayer(PlayerId.PlayerTwo).CameraCenter, DamageDealer.DamageSource.Enemy);
                    PlayerManager.GetPlayer(PlayerId.PlayerTwo).damageReceiver.TakeDamage(di);
                    int h2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.Health;
                    if (h2 >= h) return EffectResult.Retry;
                }
                else return EffectResult.Retry;


                AudioManager.Play(Sfx.Player_Hit);
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}