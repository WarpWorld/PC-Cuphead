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
        ID = 43,
        Name = "Set Filter to None"
    )]
    class NoFilter : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
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
