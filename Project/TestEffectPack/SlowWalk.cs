using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System.Reflection;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "SlowWalk",
        Name = "Slow Walk",
        Duration = 30,
        Description = "Reduces the player's movement speed",
        Price = 100,
        Categories = new string[] { "Player 1", "Walking" }, 
        Morality = Morality.Evil
        )]
    class SlowWalk : MLCC_TimedEffect
    {
         public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            try
            {
                if (!Base.isPlane())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    if (prop.moveSpeed != 490f) return EffectResult.Retry;

                    prop.moveSpeed = 250f;
                }
                else
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                    if (prop.speed != 520f) return EffectResult.Retry;

                    prop.speed = 250f;
                    prop.shrunkSpeed = 375f;
                }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.slowwalk = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                if (!Base.isPlane())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    prop.moveSpeed = 490f;
                }
                else
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                    prop.speed = 520f;
                    prop.shrunkSpeed = 720f;
                }
            }
            catch (Exception e)
            {

            }
            Base.slowwalk = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
