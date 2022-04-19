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
      ID = 13,
      Name = "Flip Screen (15 Seconds)",
      Duration = 15
    )]
    class FlipScreen : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if(Base.isRunAndGun()) return EffectResult.Retry;

            try
            {
                CupheadLevelCamera.Current.baseTransform.Rotate(0, 0, 180.0F);
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.flipped = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                CupheadLevelCamera.Current.baseTransform.Rotate(0, 0, 180.0F);
            }
            catch (Exception e)
            {
                return true;
            }
            Base.flipped = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            if (Base.isRunAndGun()) return false;
            return true;
        }
    }
}
