using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "TwoStrip",
        Name = "Set Filter to Two Strip",
        Description = "Enables the Two Strip visual filter",
        Price = 25,
        Categories = new string[] { "Screen Effects" }
    )]
    class TwoStrip : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || (!Base.onMap() && !Base.inLevel()) || (!Base.onMap() && !Base.P1Ready())) return EffectResult.Retry;

            try
            {
                if (PlayerData.Data.filter == BlurGamma.Filter.TwoStrip) return EffectResult.Retry;

                    PlayerData.Data.filter = BlurGamma.Filter.TwoStrip;
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
