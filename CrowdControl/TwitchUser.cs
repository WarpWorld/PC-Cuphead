namespace WarpWorld.CrowdControl
{
    // TODO share with other WarpWorld components? WarpWorld.Twitch maybe?
    /// <summary>
    /// Information about a Twitch user profile.
    /// </summary>
    public class TwitchUser
    {
        /// <summary>Unique Twitch user identifier.</summary>
        public ulong id;
        /// <summary>Unique Twitch user name. Always lowercase.</summary>
        public string name;
        /// <summary>Pretty printed user name.</summary>
        public string displayName;
        /// <summary>URL to download the profile icon from.</summary>
        public string profileIconUrl;
    }
}
