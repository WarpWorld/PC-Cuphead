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
        ID = "ShowBossHP",
        Name = "Show Boss HP",
        Duration = 60,
        Description = "Displays a health bar for bosses",
        Price = 50,
        Morality = Morality.Good,
        Categories = new string[] { "Boss" }
    )]
    class BossHP : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return EffectResult.Retry;
            if (TestEffectPack.Base.isRunAndGun()) return EffectResult.Retry;
            if (TestEffectPack.Base.isMausoleum()) return EffectResult.Retry;

            Base.bosshp = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.bosshp = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return false;
            if (TestEffectPack.Base.isRunAndGun()) return false;
            if (TestEffectPack.Base.isMausoleum()) return false;
            return true;
        }
    }
}
