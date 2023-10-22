using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "HealBoss",
        Name = "Heal Boss",
        Description = "Restores one health to the boss",
        Price = 300,
        Categories = new string[] { "Boss", "Heal" },
        Morality = Morality.Good
    )]
    class HealBoss : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            try
            {
                if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;
                if (Base.isRunAndGun()) return EffectResult.Retry;
                if (Base.isMausoleum()) return EffectResult.Retry;

                if (!Base.damageBoss(Level.Current, -100.0F)) return EffectResult.Retry;
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
