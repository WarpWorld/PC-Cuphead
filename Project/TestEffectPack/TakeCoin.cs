using System;
using System.Reflection;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "TakeCoin",
        Name = "Take Coin",
        Description = "Takes away one of the player's coins",
        Price = 300,
        Categories = new string[] { "Coins" },
        Morality = Morality.Evil
    )]
    class TakeCoin : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
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
