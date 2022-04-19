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
    [MLCC_TimedEffectData(
        ID = 55,
        Name = "Tiny Player 2 (30 Seconds)",
        Duration = 30
        )]
    class Tiny2 : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;

            if(Base.giant2) return EffectResult.Retry;

            Base.tiny2 = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.tiny2 = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return false;
            return true;
        }
    }
}
