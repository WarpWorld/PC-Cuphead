using System;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects.Data
{
    /// <summary>Effect data attribute for effects that handle parameters.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MLCC_ParamEffectData : MLCC_EffectData
    {
        private CCEffectParameters Effect;

        internal void ApplyData(CCEffectParameters effect)
        {
            Effect = effect;
            ApplyData((CCEffectBase)Effect);

            Effect.ClearParameters();
            if (_parameters.Length > 0)
                Effect.AddParameters(_parameters);
            _parameters = Effect.GetParameters().ToArray();
        }

        /// <summary>
        /// Parameters this Effect has.
        /// All Parameters are converted to boxed string objects upon set.
        /// </summary>
        public object[] Parameters
        {
            get => (Effect == null) ? _parameters : _parameters = Effect.GetParameters().ToArray();
            set
            {
                if (value == null)
                    value = new object[0];

                if (Effect != null)
                {
                    Effect.ClearParameters();
                    if (value.Length > 0)
                        Effect.AddParameters(value);
                    value = Effect.GetParameters().ToArray();
                }

                _parameters = value;
            }
        }
        private object[] _parameters = new object[0];
    }
}
