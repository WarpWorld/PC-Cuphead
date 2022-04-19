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
        ID = 14,
        Name = "Spin Screen (15 Seconds)",
        Duration = 15
        )]
    class SpinScreen : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready() || Base.isRunAndGun()) return EffectResult.Retry;

            if (Base.rot != 0) return EffectResult.Retry;

            Base.spin = true;
            Base.rot = 0;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.spin = false;

            //CupheadLevelCamera.Current.baseTransform.Rotate(0, 0, -Base.rot);

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready() || Base.isRunAndGun()) return false;
            return true;
        }
    }
}
