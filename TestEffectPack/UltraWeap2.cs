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
        ID = 57,
        Name = "Ultra Weapon P2 (15 Seconds)",
        Duration = 15
        )]
    class UltraWeap2 : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;

            if(Base.noFire) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;

            Base.ultra2 = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.ultra2 = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return false;
            if (Base.isMausoleum()) return false;
            return true;
        }
    }
}
