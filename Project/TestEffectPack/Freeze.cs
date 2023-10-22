using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System.Reflection;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
      ID = "FreezePlayer",
      Name = "Freeze Player",
      Duration = 5,
      Description = "Freezes the player in place briefly",
      Price = 100,
      Categories = new string[] { "Player 1", "Freeze" },
      Morality = Morality.Evil
    )]
    class Freeze : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {

            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (Base.invert) return EffectResult.Retry;

            try
            {
                
                if (!TestEffectPack.Base.isPlane())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    if (prop.moveSpeed != 490f) return EffectResult.Retry;
                    if (prop.jumpPower != -0.755f) return EffectResult.Retry;
                    if (prop.dashSpeed != 1100f) return EffectResult.Retry;

                        levelPlayerController.weaponManager.FreezePosition = true;
                }
                else
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                    if (prop.speed != 520f) return EffectResult.Retry;

                        prop.speed = 0;
                        prop.shrunkSpeed = 0;
                }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.frozen = true;

            

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {

            if (!TestEffectPack.Base.isPlane()) {
                try
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    levelPlayerController.weaponManager.FreezePosition = false;
                }
                catch (Exception e)
                {
                    return false;
                }
            } else {
                try
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                        prop.speed = 520f;
                        prop.shrunkSpeed = 720f;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            Base.frozen = false;


            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
