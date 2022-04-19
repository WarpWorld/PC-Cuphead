using ML_CrowdControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
    ID = 30,
    Name = "Disable Firing (10 Seconds)",
    Duration = 10
    )]
    class NoFire : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if(Base.ultra) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;

            try
            {
                if (!Base.isPlane())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    AbstractLevelWeapon lw = levelPlayerController.weaponManager.CurrentWeapon;
                    var m = lw.GetType().GetMethod("endFiring", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    m.Invoke(lw, new object[] { AbstractLevelWeapon.Mode.Basic });
                }
                else
                {
                    PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    AbstractPlaneWeapon lw = levelPlayerController.weaponManager.CurrentWeapon;
                    var m = lw.GetType().GetMethod("endFiring", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    m.Invoke(lw, new object[] { AbstractPlaneWeapon.Mode.Basic });
                }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.noFire = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.noFire = false;
            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
