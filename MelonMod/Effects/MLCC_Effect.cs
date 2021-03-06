using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects
{
    /// <summary>Base class for effects with no extra properties.</summary>
    public class MLCC_Effect : MLCC_EffectBase
    {
        /// <summary>Attribute that Defined the Effect's Data.</summary>
        public Data.MLCC_EffectData DataAttribute { get; internal set; }
        /// <summary>Wrapper for CrowdControl's Component.</summary>
        public Wrappers.MLCC_EffectWrapper Wrapper { get; internal set; }

        internal override uint GetID()
            => (Wrapper == null) ? 0 : Wrapper.identifier;

        /// <summary>
        /// Called when an effect instance is scheduled for execution. The returned value is communicated back to the server.
        /// <para>If <see cref="EffectResult.Retry"/> is returned, will be called again in <see cref="Data.MLCC_EffectData.RetryDelay"/> seconds,
        /// unless <see cref="Data.MLCC_EffectData.MaxRetries"/> is reached.</para>
        /// </summary>
        public virtual EffectResult OnTrigger(CCEffectInstance effectInstance) => default(EffectResult);
    }
}
