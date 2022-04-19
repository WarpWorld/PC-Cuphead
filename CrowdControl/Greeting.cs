public enum Greeting
{
    Success = 0x10,

    FailUnsupportedVersion = 0x20,
    FailUnsupportedGame = 0x21,

    BadTempToken = 0x30,
    BadSessionToken = 0x31,
    AlreadyLoggedIn = 0x32,
    Banned = 0x33,
    ChannelMissing = 0x34,
    CouldnotStartSession = 0x35
}