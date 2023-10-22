using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "UltraWeapon",
        Name = "Ultra Weapon",
        Duration = 15,
        Description = "Causes the player to fire all weapons for a brief period",
        Price = 100,
        Categories = new string[] { "Weapons", "Player 1" }
        )]
    class UltraWeap : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if(Base.noFire) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;

            Base.ultra = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.ultra = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            if (Base.isMausoleum()) return false;
            return true;
        }
    }
}
