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
    ID = 21,
    Name = "Low Jump (30 Seconds)",
    Duration = 30
    )]
    class LowJump : MLCC_TimedEffect
    {
   
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
                var p = levelPlayerController.motor.GetType().GetField("allowJumping", BindingFlags.Instance | BindingFlags.NonPublic);
                bool a = (bool)p.GetValue(levelPlayerController.motor);
                if (!a) return EffectResult.Retry;

                var p2 = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p2.GetValue(levelPlayerController.motor);
                if (prop.jumpPower != -0.755f) return EffectResult.Retry;

                prop.jumpPower = -0.5f;
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.lowjump = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;//disable jump
                var p2 = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p2.GetValue(levelPlayerController.motor);
                prop.jumpPower = -0.755f;
            }
            catch (Exception e)
            {

            }
            Base.lowjump = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            if (Base.isPlane()) return false;
            return true;
        }
    }
}
