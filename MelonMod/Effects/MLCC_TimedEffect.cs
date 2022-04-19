using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects
{
    /// <summary>Base class for effects that are active for a given duration of time.</summary>
    public class MLCC_TimedEffect : MLCC_EffectBase
    {
        /// <summary>Attribute that Defined the Effect's Data.</summary>
        public Data.MLCC_TimedEffectData DataAttribute { get; internal set; }
        /// <summary>Wrapper for CrowdControl's Component.</summary>
        public Wrappers.MLCC_TimedEffectWrapper Wrapper { get; internal set; }

        internal override uint GetID()
            => (Wrapper == null) ? 0 : Wrapper.identifier;

        /// <summary>Is the timer paused?</summary>
        public bool IsPaused()
            => (Wrapper == null) ? false : Wrapper.paused;

        /// <summary>Invoked when an effect instance is paused.</summary>
        public virtual void OnPause() { }
        /// <summary>Invoked when an effect instance is reset.</summary>
        public virtual void OnReset() { }
        /// <summary>Invoked when an effect instance is resumed.</summary>
        public virtual void OnResume() { }

        /// <summary>Invoked when an effect instance is scheduled to start. The effect should only be applied when <see cref="EffectResult.Success"/> is returned.</summary>
        public virtual EffectResult OnStart(CCEffectInstance effectInstance) => default(EffectResult);

        /// <summary>
        /// Invoked when an effect instance is scheduled to stop after its time is expired.
        /// Return false if the effect cannot be stopped at the current time to retry in <see cref="CCEffectBase.retryDelay"/>
        /// seconds.
        /// </summary>
        /// <param name="effectInstance">The target instance.</param>
        /// <param name="force">Set to true when called from <see cref="CrowdControl.StopAllEffects"/>,
        /// in which case failure to stop the effect is ignored.</param>
        public virtual bool OnStop(CCEffectInstance effectInstance, bool force) => default(bool);

        /// <summary>Conditions to be ran.</summary>
        public virtual bool ShouldRun() => !IsPaused();

        /// <summary>Invoked when the behaviour is resumed.</summary>
        public void Resume()
        {
            if (Wrapper != null)
                CrowdControl.EnableEffect(Wrapper);
        }

        /// <summary>Invoked when the behaviour is paused.</summary>
        public void Pause()
        {
            if (Wrapper != null)
                CrowdControl.DisableEffect(Wrapper);
        }

        /// <summary>Invoked when the behaviour is reset.</summary>
        public void Reset()
        {
            if (Wrapper != null)
                CrowdControl.ResetEffect(Wrapper);
        }
    }
}
