using System.Reflection;
using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "SwapCharacters",
        Name = "Swap Characters",
        Description = "Swaps Cuphead with Mugman",
        Price = 25
    )]
    class SwapChars : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            PlayerManager.player1IsMugman = !PlayerManager.player1IsMugman;

            try
            {
                    if (!Base.isPlane())
                    {
                        LevelPlayerController lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerOne);
                        if (lpc) lpc.animationController.LevelInit();
                        lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerTwo);
                        if (lpc) lpc.animationController.LevelInit();
                    }
                    else
                    {
                        PlanePlayerController lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                        if (lpc) lpc.animationController.LevelInit();
                        lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
                        if (lpc) lpc.animationController.LevelInit();
                    }

                    if (PlayerStatsManager.DebugInvincible)
                    {
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
                    else
                    {
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
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
