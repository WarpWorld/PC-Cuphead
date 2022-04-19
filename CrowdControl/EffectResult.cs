namespace WarpWorld.CrowdControl
{
    /// <summary>Result of trying to trigger an effect. Determines the reply sent back to the server.</summary>
    public enum EffectResult
    {
        /// <summary>The effect executed successfully.</summary>
        Success = Protocol.ResultType.Success,
        /// <summary>The effect failed to trigger, but is still available for use. Viewer(s) will be refunded.</summary>
        Failure = Protocol.ResultType.Failure,
        /// <summary>Same as <see cref="Failure"/> but the effect is no longer available for use.</summary>
        Unavailable = Protocol.ResultType.Unavailable,
        /// <summary>The effect cannot be triggered right now, try again in a few seconds.</summary>
        Retry = Protocol.ResultType.Retry,
        /// <summary>INTERNAL USE ONLY. The effect has been queued for execution after the current one ends.</summary>
        Queue = Protocol.ResultType.Queue,
        /// <summary>INTERNAL USE ONLY. The effect triggered successfully and is now active until it ends.</summary>
        Running = Protocol.ResultType.Running
    }
}
