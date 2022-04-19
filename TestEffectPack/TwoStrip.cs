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
    ID = 42,
    Name = "Set Filter to Two Strip"
    )]
    class TwoStrip : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
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
