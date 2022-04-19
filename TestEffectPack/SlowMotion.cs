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
    ID = 17,
    Name = "Slow Motion (15 Seconds)",
    Duration = 15
    )]
    class SlowMotion : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            CupheadTime.GlobalSpeed = 0.25F;
            Base.slowmo = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            CupheadTime.GlobalSpeed = 1.0F;
            Base.slowmo = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
