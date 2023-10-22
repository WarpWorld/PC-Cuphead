using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
      ID = "DamageDown",
      Name = "Damage Down",
      Duration = 30,
      Description = "Decreases your damage for a short time",
      Price = 25,
      Categories = new string[] { "Damage" },
      Morality = Morality.Evil
    )]
    class DamageDown : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return EffectResult.Retry;

            if(Base.dmgup) return EffectResult.Retry;

            Base.dmgdown = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.dmgdown = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return false;
            return true;
        }
    }
}
