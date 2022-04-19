namespace WarpWorld.CrowdControl
{
    /// <summary>
    /// State of a Crowd Control effect.
    /// <para>The instance is only valid from the time the effect is requested up to
    /// the time the effect is successfully triggered or failed.</para>
    /// </summary>
    public class CCEffectInstance
    {
        /// <summary>Target effect behaviour containing the logic.</summary>
        public CCEffectBase effect { get; internal set; }

        /// <summary>
        /// The Twitch user who triggered the effect.
        /// <para>This is set to the special <see cref="CrowdControl.crowdUser"/> value for pooled effects.</para>
        /// <para>For local test effect instances, this is set to the special <see cref="CrowdControl.testUser"/> value.</para>
        /// </summary>
        public TwitchUser user { get; internal set; }

        /// <summary>
        /// Unique identifier of this instance, shared with the server. ID value are never reused.
        /// <para>If the effect is tested locally, this ID will be negative.</para>
        /// </summary>
        public uint id { get; internal set; }

        /// <summary>
        /// How many times has the effect returned <see cref="EffectResult.Retry"/> when trying to start the instance.
        /// <para>If the associated <see cref="CCEffectBase.maxRetries"/> is reached, the instance is automatically
        /// terminated as if starting it returned <see cref="EffectResult.Failure"/>.</para>
        /// </summary>
        public int retryCount { get; internal set; }

        /// <summary>Unscaled game time when the effect was triggered.</summary>
        public float unscaledStartTime { get; internal set; }

        /// <summary>When <see langword="true"/>, the effect executes locally without talking to the server.</summary>
        public bool isTest = false;

        /// <summary>The parameters sent into the effect. Eg: Item Type, Quantity</summary>
        public string[] parameters { get; internal set; }

        /// <summary>The id for the effect that's being used</summary>
        public uint effectID { get { return effect.identifier; } }

        // TODO: public float completion;
    }
}
