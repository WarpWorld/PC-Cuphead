#if ML_Il2Cpp
using System;
using UnhollowerBaseLib.Attributes;
#endif
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
#pragma warning disable 1591

namespace ML_CrowdControl.Wrappers
{
    /// <summary>Unity Component Wrapper for MLCC_Effect.</summary>
    public class MLCC_EffectWrapper : CCEffect
    {
        internal MLCC_Effect effect;

#if ML_Il2Cpp
        public MLCC_EffectWrapper(IntPtr value) : base(value) { }
#endif

        public void Update()
            => effect?.OnUpdate();

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        protected override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
            => (effect == null) ? default(EffectResult) : effect.OnTrigger(effectInstance);
    }
}
