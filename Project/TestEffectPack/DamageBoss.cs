using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System;
using WarpWorld.CrowdControl;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "DamageBoss",
        Name = "Damage Boss",
        Categories = new string[] { "Damage", "Boss" },
        Morality = Morality.Good
    )]
    class DamageBoss : MLCC_Effect {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance) {
            try {
                if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;
                if (Base.isRunAndGun()) return EffectResult.Retry;
                if (Base.isMausoleum()) return EffectResult.Retry;
                if (!Base.damageBoss(Level.Current, 100.0F)) return EffectResult.Retry;
            }
            catch (Exception e) {
                return EffectResult.Failure;
            }

            return EffectResult.Success;
        }
    }
}
