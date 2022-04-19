using ML_CrowdControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = 52,
        Name = "Slow Walk P2 (30 Seconds)",
        Duration = 30
        )]
    class SlowWalk2 : MLCC_TimedEffect
    {
         public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;

            try
            {
                if (!Base.isPlane())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                    if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    if (prop.moveSpeed != 490f) return EffectResult.Retry;

                    prop.moveSpeed = 250f;
                }
                else
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
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

            Base.slowwalk2 = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                if (!Base.isPlane())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    prop.moveSpeed = 490f;
                }
                else
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                    prop.speed = 520f;
                    prop.shrunkSpeed = 720f;
                }
            }
            catch (Exception e)
            {

            }
            Base.slowwalk2 = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return false;
            return true;
        }
    }
}
