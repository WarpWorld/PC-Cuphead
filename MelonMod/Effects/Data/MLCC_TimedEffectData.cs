using System;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects.Data
{
    /// <summary>Effect data attribute for effects that are active for a given duration of time.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MLCC_TimedEffectData : MLCC_EffectData
    {
        private CCEffectTimed Effect;

        internal void ApplyData(CCEffectTimed effect)
        {
            Effect = effect;
            ApplyData((CCEffectBase)Effect);

            Effect.duration = _duration;
        }

        /// <summary>
        /// Duration in seconds before the effect is automatically ended.
        /// Must be a float value between 1 - 600
        /// </summary>
        public float Duration
        {
            get => (Effect == null) ? _duration : _duration = Effect.duration;
            set
            {
                if ((value < 1) || (value > 600))
                    throw new Exception("Duration must be set to a float value between 1 - 600!");

                _duration = value;
                if (Effect != null)
                    Effect.duration = value;
            }
        }
        private float _duration = 60;
    }
}
