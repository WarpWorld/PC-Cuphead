using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System.Reflection;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "FastWalk",
        Name = "Fast Walk",
        Description = "Increases the player's movement speed",
        Price = 50,
        Duration = 30,
        Categories = new string[] { "Player 1", "Walking" }
    )]
    class FastWalk : MLCC_TimedEffect
    {
         public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (!Base.isPlane())
            {
                try
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    if (prop.moveSpeed != 490f) return EffectResult.Retry;

 

                        prop.moveSpeed = 1000f;
                }
                catch (Exception e)
                {

                }
            } else
            {
                try { 
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                    if (prop.speed != 520f) return EffectResult.Retry;

                    prop.speed = 1000f;
                    prop.shrunkSpeed = 1500f;
                }
                catch (Exception e)
                {

                }

            }
            Base.fastwalk = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            if (!Base.isPlane())
            {
                try
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    prop.moveSpeed = 490f;
                }
                catch (Exception e)
                {
                    return true;
                }
            } else
            {
                try
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                    prop.speed = 520f;
                    prop.shrunkSpeed = 720f;
                }
                catch (Exception e)
                {
                    return true;
                }
            }

            Base.fastwalk = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
