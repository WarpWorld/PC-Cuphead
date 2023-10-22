using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "NoSwitch",
        Name = "Disable Weapon Switch",
        Duration = 30,
        Description = "Prevents the player from swapping weapons",
        Price = 50,
        Categories = new string[] { "Weapons" },
        Morality = Morality.Evil
    )]
    class NoSwitch : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (Base.ultra) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;

            Base.noSwitch = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.noSwitch = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
