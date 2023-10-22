using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_TimedEffectData(
        ID = "ZoomOut",
        Name = "Zoom Out",
        Duration = 30,
        Description = "Zooms the camera out",
        Price = 200,
        Categories = new string[] { "Screen Effects" }
    )]
    class ZoomOut : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            try
            {
                CupheadLevelCamera.Current.Zoom(0.5F, 5.0F, EaseUtils.EaseType.linear);
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            Base.zoom = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            try
            {
                CupheadLevelCamera.Current.Zoom(1.0F, 5.0F, EaseUtils.EaseType.linear);
            }
            catch (Exception e)
            {

            }
            Base.zoom = false;
            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
