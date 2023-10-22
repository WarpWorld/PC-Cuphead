using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "TinyPlayer",
        Name = "Tiny Player",
        Duration = 30,
        Description = "Makes the player smaller temporarily. The player may have to JUMP to reset the size after this wears off!",
        Price = 50,
        Categories = new string[] { "Tiny Player", "Player 1" },
        Morality = Morality.Evil
        )]
    class Tiny : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if(Base.giant) return EffectResult.Retry;

            Base.tiny = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.tiny = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
