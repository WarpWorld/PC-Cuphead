using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "NoFilter",
        Name = "Set Filter to None",
        Description = "Disables Visual Filters",
        Price = 25,
       Categories = new string[] { "Screen Effects" }
    )]
    class NoFilter : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || (!Base.onMap() && !Base.inLevel()) || (!Base.onMap() && !Base.P1Ready())) return EffectResult.Retry;

            try
            {
                if (PlayerData.Data.filter == BlurGamma.Filter.None) return EffectResult.Retry;

                    PlayerData.Data.filter = BlurGamma.Filter.None;
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
