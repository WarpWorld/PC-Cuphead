using MelonLoader;
using System.Net.Sockets;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl
{
    /// <summary>Base class for UI Interfaces.</summary>
    public class MLCC_UI
    {
        /// <summary>MelonLogger Instance for the effect.</summary>
        public MelonLogger.Instance Logger { get; internal set; }

        /// <summary>Invoked when the UI Interface is loaded and registered.</summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Invoked when MonoBehaviour.Update is called.
        /// Called once every frame.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Invoked when MonoBehaviour.OnGUI is called.
        /// Called once every frame.
        /// </summary>
        public virtual void OnGUI() { }

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

        /// <summary>
        /// Invoked when Showing or Hiding the User Input Prompt for their Activation Code.
        /// If Overriden the Console Prompt will not be shown.
        /// </summary>
        public virtual void OnAuthenticationPrompt(bool isShown) { if (isShown) ModCore.AskForAuthToken(); }

        /// <summary>Invoked when disconnected from the Crowd Control server.</summary>
        public virtual void OnDisconnected() { }

        /// <summary>Invoked when a message is received from the Crowd Control server or api.</summary>
        public virtual void OnMessage(string msg) { }

        /// <summary>Wipes the currently set Authentication Token and Deauthenticates the Current User.</summary>
        public void DeauthorizeUser()
            => DeauthorizeUser(true);
        /// <summary>Wipes the currently set Authentication Token and Deauthenticates the Current User.</summary>
        public void DeauthorizeUser(bool shouldReconnect)
        {
            if ((CrowdControl.instance == null) || !CrowdControl.instance.isConnected)
            {
                MLCC_Config.AuthToken.ResetToDefault();
                return;
            }
            CrowdControl.instance.Disconnect();
            MLCC_Config.AuthToken.ResetToDefault();
            if (shouldReconnect)
            {
                CrowdControl.instance.Connect();
            }
        }
    }
}
