namespace WarpWorld.CrowdControl
{
    /// <summary>State of a Time-Based Crowd Control effect. The instance is valid from the time the effect is requested and lives until the effect is successfully stopped or failed.</summary>
    public class CCEffectInstanceTimed : CCEffectInstance
    {
        /// <summary>Reference to the target timed effect, <see langword="null"/> if <see cref="effect"/> is not timed.</summary>
        new public CCEffectTimed effect { get; internal set; }

        /// <summary>Unscaled game time when the timed effect will end. Updated on resume.</summary>
        public float unscaledEndTime { get; internal set; }
        /// <summary>Unscaled game time left to execute.</summary>
        public float unscaledTimeLeft { get; internal set; }

        /// <summary>
        /// Whether the effect instance is active. <see langword="true"/> when the related Effect behaviour is enabled.
        /// <para>When <see langword="false"/>, pending instances will not start and running instances are paused.</para>
        /// </summary>
        public bool isActive = true;

        /// <summary>Whether the effect instance is paused or not. </summary>
        public bool isPaused => effect.paused;

        /// <summary> Checks if the effect should be running or not, then applies the paused state based on it. </summary>
        public bool shouldBeRunning => effect.ShouldBeRunning();
    }
}
