using ML_CrowdControl;
using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "BlackAndWhite",
        Name = "Set Filter to Black and White",
        Description = "Enables the Black & White visual filter",
        Price = 25,
        Categories = new[] { "Screen Effects" }
    )]
    class BlackWhite : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            try
            {
                if (!Base.isReady() || (!Base.onMap() && !Base.inLevel()) || (!Base.onMap() && !Base.P1Ready())) return EffectResult.Retry;

                if (PlayerData.Data == null || PlayerData.Data.filter == BlurGamma.Filter.BW) return EffectResult.Retry;


                    PlayerData.Data.filter = BlurGamma.Filter.BW;
 
            }
            catch(Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
