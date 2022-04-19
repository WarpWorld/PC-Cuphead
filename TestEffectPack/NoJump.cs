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
    ID = 6,
    Name = "Disable Jump (5 Seconds)",
    Duration = 5
    )]
    class NoJump : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (Base.isPlane()) return EffectResult.Retry;

            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;

                var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                if (prop.jumpPower != -0.755f) return EffectResult.Retry;

                levelPlayerController.motor.DisableJump();
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                levelPlayerController.motor.EnableJump();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override bool ShouldRun()
        {
            try
            {

                if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
                if (Base.isPlane()) return false;

            }
            catch(Exception e)
            {
                Logger.Msg($"{e.ToString()}");
            }
            return true;
        }
    }
}
