using ML_CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
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
        ID = 18,
        Name = "Damage Boss"
    )]
    class DamageBoss : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;
            if (Base.isRunAndGun()) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;

            if(!Base.damageBoss(Level.Current,100.0F))return EffectResult.Retry;

            return EffectResult.Success;
        }
    }
}
