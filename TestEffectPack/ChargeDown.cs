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
        ID = 5,
        Name = "Charge Down"
    )]
    class ChargeDown : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return EffectResult.Retry;

            if (Base.isMausoleum()) return EffectResult.Retry;

            try
            {
                if (PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.SuperMeter >= 10.0F)
                {
                    PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.OnEx();
                    AudioManager.Play("player_parry_power_up");
                }
                else
                    return EffectResult.Retry;

                if (Base.P2Ready())
                {
                    if (PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.SuperMeter >= 10.0F)
                    {
                        PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.OnEx();
                    }
                }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
