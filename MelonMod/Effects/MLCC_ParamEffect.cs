using System.Collections.Generic;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects
{
    /// <summary>Base class for effects that handles parameters.</summary>
    public class MLCC_ParamEffect : MLCC_EffectBase
    {
        /// <summary>Attribute that Defined the Effect's Data.</summary>
        public Data.MLCC_ParamEffectData DataAttribute { get; internal set; }
        /// <summary>Wrapper for CrowdControl's Component.</summary>
        public Wrappers.MLCC_ParamEffectWrapper Wrapper { get; internal set; }

        internal override uint GetID()
            => (Wrapper == null) ? 0 : Wrapper.identifier;

        /// <summary>Function for dynamically adding object(s) to the parameter list.</summary>
        public void AddParameters(params object[] prms)
            => Wrapper?.AddParameters(prms);

        /// <summary>Clearing the established parameter list.</summary>
        public void ClearParameters()
            => Wrapper?.ClearParameters();

        /// <summary>Parameters this Effect has.</summary>
        public List<string> GetParameters()
            => (Wrapper == null) ? null : Wrapper.GetParameters();

        /// <summary>Used for processing the newly received parameter array. Can be overridden by a derived class.</summary>
        public virtual void OnAssignParameters(string[] prms) { }

        /// <summary>
        /// Called when an effect instance is scheduled for execution. The returned value is communicated back to the server.
        /// <para>If <see cref="EffectResult.Retry"/> is returned, will be called again in <see cref="Data.MLCC_EffectData.RetryDelay"/> seconds,
        /// unless <see cref="Data.MLCC_EffectData.MaxRetries"/> is reached.</para>
        /// </summary>
        public virtual EffectResult OnTrigger(CCEffectInstance effectInstance) => default(EffectResult);
    }
}
