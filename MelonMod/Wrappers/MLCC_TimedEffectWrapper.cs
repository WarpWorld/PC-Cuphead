#if ML_Il2Cpp
using System;
using UnhollowerBaseLib.Attributes;
#endif
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
#pragma warning disable 1591

namespace ML_CrowdControl.Wrappers
{
    /// <summary>Unity Component Wrapper for MLCC_TimedEffect.</summary>
    public class MLCC_TimedEffectWrapper : CCEffectTimed
    {
        internal MLCC_TimedEffect effect;

#if ML_Il2Cpp
        public MLCC_TimedEffectWrapper(IntPtr value) : base(value) { }
#endif

        public void Update()
            => effect?.OnUpdate();

        protected override void OnPauseEffect()
            => effect?.OnPause();

        protected override void OnResetEffect()
            => effect?.OnReset();

        protected override void OnResumeEffect()
            => effect?.OnResume();

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        protected override EffectResult OnStartEffect(CCEffectInstanceTimed effectInstance)
            => (effect == null) ? default(EffectResult) : effect.OnStart(effectInstance);

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        protected override bool OnStopEffect(CCEffectInstanceTimed effectInstance, bool force)
            => (effect == null) ? default(bool) : effect.OnStop(effectInstance, force);

        public override bool RunningCondition()
            => (effect == null) ? default(bool) : effect.ShouldRun();
    }
}
