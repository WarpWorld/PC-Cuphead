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
    ID = 2,
    Name = "Take Coin"
    )]
    class TakeCoin : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || (!Base.onMap() && !Base.inLevel()) || (!Base.onMap() && !Base.P1Ready())) return EffectResult.Retry;

            try
            {
                if (PlayerData.Data.GetCurrency(PlayerId.PlayerOne) > 0)
                {
                    PlayerData.Data.AddCurrency(PlayerId.PlayerOne, -1);
                    AudioManager.Play("level_coin_pickup");
                }
                else return EffectResult.Retry;
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
