using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
          ID = "DamageUp",
          Name = "Increase Damage",
          Duration = 30,
          Description = "Increases your damage for a short time",
          Price = 25,
          Categories = new string[] { "Damage" },
          Morality = Morality.Good
    )]
    class DamageUp : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return EffectResult.Retry;

            if(Base.dmgdown) return EffectResult.Retry;

            Base.dmgup = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.dmgup = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return false;
            return true;
        }
    }
}
