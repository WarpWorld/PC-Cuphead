using System;
using UnityEngine;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects.Data
{
    /// <summary>Effect data attribute for single-trigger effects with no extra properties.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MLCC_EffectData : Attribute
    {
        private CCEffectBase Effect;

        internal void ApplyData(CCEffectBase effect)
        {
            Effect = effect;

            Effect.identifier = _id;
            Effect.displayName = _name;
            Effect.description = _description;
            Effect.maxRetries = _maxRetries;
            Effect.retryDelay = _retryDelay;
            Effect.pendingDelay = _pendingDelay;
        }

        /// <summary>
        /// Unique identifier of the effect.
        /// Must be a value greator than 0.
        /// Can't be set during Runtime.
        /// </summary>
        public uint ID
        {
            get => (Effect == null) ? _id : _id = Effect.identifier;
            set
            {
                if (value <= 0)
                    throw new Exception("ID must be set to a value greater than 0!");

                if (Effect != null)
                    throw new Exception("ID can't be set during Runtime!");

                _id = value;
            }
        }
        private uint _id = 0;

        /// <summary>
        /// Name of the effect displayed to the users.
        /// Can't be null or Empty.
        /// </summary>
        public string Name
        {
            get => (Effect == null) ? _name : _name = Effect.displayName;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Name must not be null or Empty!");

                _name = value;
                if (Effect != null)
                    Effect.displayName = _name;
            }
        }
        private string _name = string.Empty;

        /// <summary>Description of the effect, displayed in the extension.</summary>
        public string Description
        {
            get => (Effect == null) ? _description : _description = Effect.description;
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;
                _description = value;
                if (Effect != null)
                    Effect.description = _description;
            }
        }
        private string _description = string.Empty;

        /// <summary>
        /// Number of retries before the effect instance fails.
        /// Must be an int value between 0 - 60
        /// </summary>
        public int MaxRetries
        {
            get => (Effect == null) ? _maxRetries : _maxRetries = Effect.maxRetries;
            set
            {
                if ((value < 0) || (value > 60))
                    throw new Exception("MaxRetries must be set to an int value between 0 - 60!");

                _maxRetries = value;
                if (Effect != null)
                    Effect.maxRetries = _maxRetries;
            }
        }
        private int _maxRetries = 3;

        /// <summary>
        /// Delay in seconds before retrying to trigger an effect instance.
        /// Must be a float value between 0 - 10
        /// </summary>
        public float RetryDelay
        {
            get => (Effect == null) ? _retryDelay : _retryDelay = Effect.retryDelay;
            set
            {
                if ((value < 0) || (value > 10))
                    throw new Exception("RetryDelay must be set to a float value between 0 - 10!");

                _retryDelay = value;
                if (Effect != null)
                    Effect.retryDelay = _retryDelay;
            }
        }
        private float _retryDelay = 5;

        /// <summary>
        /// Delay in seconds to wait before triggering the next effect instance.
        /// Must be a float value between 0 - 10
        /// </summary>
        public float PendingDelay
        {
            get => (Effect == null) ? _pendingDelay : _pendingDelay = Effect.pendingDelay;
            set
            {
                if ((value < 0) || (value > 10))
                    throw new Exception("PendingDelay must be set to a float value between 0 - 10!");

                _pendingDelay = value;
                if (Effect != null)
                    Effect.pendingDelay = _pendingDelay;
            }
        }
        private float _pendingDelay = 0.5f;
    }
}
