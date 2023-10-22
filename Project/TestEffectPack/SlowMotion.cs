using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "SlowMotion",
        Name = "Slow Motion",
        Duration = 15,
        Description = "Makes the game run in slow motion",
        Price = 100,
        Morality = Morality.Evil
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
