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
       ID = 24,
       Name = "Unlimited Dashes (30 Seconds)",
       Duration = 30
   )]
    class EndlessDash : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            bool go = false;

            try
            {

                if (Base.P1Ready())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                    if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
                }
                if (Base.P2Ready())
                {
                    LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                    if (levelPlayerController.weaponManager.FreezePosition) return EffectResult.Retry;
                }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.dashes = true;
            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            Base.dashes = false;
            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return false;
            if (Base.isPlane()) return false;
            return true;
        }
    }
}
