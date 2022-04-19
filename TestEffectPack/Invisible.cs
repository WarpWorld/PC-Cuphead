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
     ID = 11,
     Name = "Invisible (15 Seconds)",
     Duration = 15
   )]
    class Invisible : MLCC_TimedEffect
    {
        public override EffectResult OnStart(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            Base.invis = true;

            return EffectResult.Success;
        }

        public override bool OnStop(CCEffectInstance effectInstance, bool force)
        {
            if (!TestEffectPack.Base.isPlane())
            {
                try
                {
                    LevelPlayerAnimationController component = PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerAnimationController>();

                    MethodInfo m = component.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    m.Invoke(component, new object[] { 1.0f });

                    LevelPlayerAnimationController component2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerAnimationController>();

                    if (component2)
                    {
                        m = component.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        m.Invoke(component2, new object[] { 1.0f });
                    }
                } catch (Exception e)
                {

                }
            } else
            {
                try
                {
                    PlanePlayerAnimationController component = PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<PlanePlayerAnimationController>();

                    MethodInfo m = component.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    m.Invoke(component, new object[] { 1.0f });

                    PlanePlayerAnimationController component2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<PlanePlayerAnimationController>();

                    if (component2)
                    {
                        m = component.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        m.Invoke(component2, new object[] { 1.0f });
                    }
                }
                catch (Exception e)
                {

                }

            }

            Base.invis = false;

            return true;
        }

        public override bool ShouldRun()
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return false;
            return true;
        }
    }
}
