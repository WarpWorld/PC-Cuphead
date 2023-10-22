using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System.Reflection;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "LowJumpP2",
        Name = "Low Jump P2",
        Duration = 30,
        Description = "Decreases the height of the player's jump",
        Price = 100,
        Categories = new string[] { "Jumping", "Player 2" },
        Morality = Morality.Evil
    )]
    class LowJump2 : MLCC_TimedEffect
    {

        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            try {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                if (levelPlayerController.weaponManager.FreezePosition || Base.highjump2 || Base.lowjump2) return EffectResult.Retry;

                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    prop.jumpPower += 0.3f;
                    var p2 = levelPlayerController.motor.GetType().GetField("jumpPower", BindingFlags.Instance | BindingFlags.NonPublic);
                    float val = (float)p2.GetValue(levelPlayerController.motor);
                    p2.SetValue(levelPlayerController.motor, val + 0.3f);
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.lowjump2 = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);

                    prop.jumpPower -= 0.3f;
                    var p2 = levelPlayerController.motor.GetType().GetField("jumpPower", BindingFlags.Instance | BindingFlags.NonPublic);
                    float val = (float)p2.GetValue(levelPlayerController.motor);
                    p2.SetValue(levelPlayerController.motor, val - 0.3f);
            }
            catch (Exception e)
            {

            }
            Base.lowjump2 = false;

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
