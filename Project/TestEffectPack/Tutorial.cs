using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "Tutorial",
        Name = "Send to Tutorial",
        Description = "Sends the player back to the tutorial",
        Price = 800,
        Categories = new string[] { "Warping" },
        Morality = Morality.Evil
    )]
    class Tutorial : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            try
            {
                SceneLoader.LoadLevel(Levels.Tutorial, SceneLoader.Transition.Iris);
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
