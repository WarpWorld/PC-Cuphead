using System.Net.Sockets;
using MelonLoader;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects
{
    /// <summary>
    /// Base class of All Effects.
    /// NEVER TOUCH THIS CLASS DIRECTLY!
    /// Always use the appropriate Effect Base Subclass to inherit from.
    /// </summary>
    public class MLCC_EffectBase
    {
        internal virtual uint GetID() => 0;

        /// <summary>MelonLogger Instance for the effect.</summary>
        public MelonLogger.Instance Logger { get; internal set; }

        /// <summary>Invoked when an effect is loaded and registered.</summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Invoked when MonoBehaviour.Update is called.
        /// Called once every frame regardless of state of effect.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>Invoked when attempting a connection to the Crowd Control server.</summary>
        public virtual void OnConnecting() { }

        /// <summary>Invoked when the connection to the Crowd Control server has failed.</summary>
        public virtual void OnConnectionError(SocketError socketError) { }

        /// <summary>Invoked when successfully connected to the Crowd Control server.</summary>
        public virtual void OnConnected() { }

        /// <summary>Invoked when the Authentication Token has been Authorized and Confirmed by the Crowd Control server.</summary>
        public virtual void OnAuthenticated() { }

        /// <summary>Invoked when Authentication with the Crowd Control server fails.</summary>
        public virtual void OnAuthenticationFailed() { }

        /// <summary>Invoked when disconnected from the Crowd Control server.</summary>
        public virtual void OnDisconnected() { }

        /// <summary>Invoked when a message is received from the Crowd Control server or api.</summary>
        public virtual void OnMessage(string msg) { }

        /// <summary>Toggles whether this effect can currently be sold during this session.</summary>
        public void ToggleSellable(bool sellable)
            => CrowdControl.instance.ToggleEffectSellable(GetID(), sellable);

        /// <summary>Toggles whether this effect is visible in the menu during this session.</summary>
        public void ToggleVisible(bool visible)
            => CrowdControl.instance.ToggleEffectVisible(GetID(), visible);
    }
}
