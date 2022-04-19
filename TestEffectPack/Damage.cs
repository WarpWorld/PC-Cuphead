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
        ID = 10,
        Name = "Damage"
    )]
    class Damage : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            try
            {

                if (PlayerStatsManager.DebugInvincible) return EffectResult.Retry;
                if (Base.isMausoleum()) return EffectResult.Retry;


                int h = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.Health;
                if (h > 1)
                {
                    DamageDealer.DamageInfo di = new DamageDealer.DamageInfo(1.0F, DamageDealer.Direction.Neutral, PlayerManager.GetPlayer(PlayerId.PlayerOne).CameraCenter, DamageDealer.DamageSource.Enemy);
                    PlayerManager.GetPlayer(PlayerId.PlayerOne).damageReceiver.TakeDamage(di);
                    int h2 = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.Health;
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
