using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System.Reflection;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "ShortDashP2",
        Name = "Short Dash P2",
        Duration = 30,
        Description = "Decreases the length of the player's dash",
        Price = 45,
        Categories = new string[] { "Dashing", "Player 2" },
        Morality = Morality.Evil
    )]
    class ShortDash2 : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;

                var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                if (prop.dashSpeed != 1100f) return EffectResult.Retry;

                prop.dashSpeed = 200f;
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.shortdash2 = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    prop.dashSpeed = 1100f;
            }
            catch (Exception e)
            {

            }
            Base.shortdash2 = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return false;
            if (Base.isPlane()) return false;
            return true;
        }
    }
}
