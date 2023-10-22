using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
          ID = "GiantPlayer2",
          Name = "Giant Player 2",
          Duration = 30,
          Description = "Makes the player larger temporarily. The player may have to JUMP to reset the size after this wears off!",
          Price = 50,
          Categories = new string[] { "Player 2", "Giant" }
    )]
    class Giant2 : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;

            if(Base.tiny2) return EffectResult.Retry;

            Base.giant2 = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.giant2 = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return false;
            return true;
        }
    }
}
