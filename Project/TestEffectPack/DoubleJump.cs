using System;
using System.Reflection;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "DoubleJump",
        Name = "Double Jump",
        Duration = 30,
        Description = "Allows the player to jump a second time in air",
        Price = 50,
        Categories = new string[] { "Jumping" }, 
        Morality = Morality.Good
    )]
    class DoubleJump : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;//disable jump
                var p = levelPlayerController.motor.GetType().GetField("allowJumping", BindingFlags.Instance | BindingFlags.NonPublic);
                bool a = (bool)p.GetValue(levelPlayerController.motor);
                if (!a) return EffectResult.Retry;

                if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.doublejump = true;
            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.doublejump = false;
            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return false;
            if (Base.isPlane()) return false;
            return true;
        }
    }
}
