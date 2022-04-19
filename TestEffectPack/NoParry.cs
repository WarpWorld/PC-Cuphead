using ML_CrowdControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using MelonLoader;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
    ID = 31,
    Name = "Disable Parrying (30 Seconds)",
    Duration = 30
    )]
    class NoParry : MLCC_TimedEffect
    {
         public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;

            try
            {
                if (!TestEffectPack.Base.isPlane())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
                }
                else
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;

                    if (prop.speed == 0) return EffectResult.Retry;
                }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.noParry = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.noParry = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready())
            {
                return false;
            }
            return true;
        }
    }
}
