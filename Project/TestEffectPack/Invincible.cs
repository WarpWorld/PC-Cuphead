using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "Invulnerable",
        Name = "Invulnerable",
        Duration = 15,
        Description = "Prevents the player from taking damage briefly",
        Price = 50,
        Morality = Morality.Good
    )]
    class Invincible : MLCC_TimedEffect 
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            try
            {
                if (PlayerStatsManager.DebugInvincible) return EffectResult.Retry;

                    PlayerStatsManager.DebugToggleInvincible();

                    if (TestEffectPack.Base.isPlane())
                    {
                        PlanePlayerController lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                        if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                        lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
                        if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                    }
                    else
                    {
                        LevelPlayerController lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerOne);
                        if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                        lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerTwo);
                        if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                    }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }
            Base.invul = true;
            return EffectResult.Success;
        }


        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                if (!PlayerStatsManager.DebugInvincible) return true;
                PlayerStatsManager.DebugToggleInvincible();

                if (TestEffectPack.Base.isPlane())
                {
                    PlanePlayerController lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.white);
                    lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.white);
                }
                else
                {
                    LevelPlayerController lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerOne);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.white);
                    lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerTwo);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.white);
                }
            }
            catch(Exception e)
            {
                return false;
            }
            Base.invul = false;
            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
