using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "GiveCoin",
        Name = "Give Coin",
        Description = "Gives the player a coin",
        Price = 200,
        Categories = new string[] { "Coins" },
        Morality = Morality.Good
    )]
    class GiveCoin : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || (!Base.onMap() && !Base.inLevel()) || (!Base.onMap() && !Base.P1Ready())) return EffectResult.Retry;

            try
            {
                    PlayerData.Data.AddCurrency(PlayerId.PlayerOne, 1);
                    AudioManager.Play("level_coin_pickup");
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }
            return EffectResult.Success;
        }
    }
}
