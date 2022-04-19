// NOTE: copy changes into MessageServer
using System;
using System.Text;

namespace WarpWorld.CrowdControl {
    public static class Protocol {
        // byte msgType
        // ushort size
        public const int FRAME_SIZE = 3;

        public const byte VERSION = 2;

        public const int PING_INTERVAL = 10;

        public enum EffectState : byte
        {
            Success = 0x10,
            DelayedSuccess = 0x11,
            FinalSuccess = 0x12,
            BidWarSuccess = 0x14,

            TemporaryFailure = 0x20,
            PermanentFailure = 0x21,
            BidWarFailure = 0x24,

            UnknownDelay = 0x30,
            ExactDelay = 0x31,
            EstimatedDelay = 0x32,
            ExtendedDelay = 0x33,
            BidWarDelay = 0x34,

            TimedEffectBegin = 0x40,
            TimedEffectEnd = 0x41,
            TimedPause = 0x42,
            TimedResume = 0x43,

            AvailableForOrder = 0x60,
            UnavailableForOrder = 0x61,
            VisibleOnMenu = 0x62,
            HiddenOnMenu = 0x63
        }

        // $00 - $3F Client to Server
        // $40 - $7F Server to Client
        // $80 - $FF Protocol

        public enum ResultType : byte {
            Success = 0,
            Failure = 1,
            Unavailable = 2,
            Retry = 3,
            Queue = 4,
            Running = 5
        }

        public enum Status : byte {
            Success = 0x00,

            // Common failures
            InvalidKey = 0x10,

            // Game Session failures
            GameActive = 0x20,
            GameInactive = 0x21,
            MissingEffectList = 0x22,
            InvalidEffectId = 0x23,

            // Authentication failures
            InvalidUser = 0xA0,
            Authenticated = 0xA1,
            Unauthenticated = 0xA2,

            // Server failures
            InternalError = 0xC0,

            // Protocol failures
            InvalidSize = 0xFE,
            InvalidVersion = 0xFF
        }

        public static void Read(byte[] buffer, ref int offset, out byte value) {
            value = buffer[offset++];
        }
        public static void Read(byte[] buffer, ref int offset, out ushort value) {
            value = unchecked((ushort)(
                buffer[offset + 0] << 8 |
                buffer[offset + 1]));
            offset += 2;
        }
        public static void Read(byte[] buffer, ref int offset, out uint value) {
            value = unchecked((uint)(
                buffer[offset + 0] << 24 |
                buffer[offset + 1] << 16 |
                buffer[offset + 2] << 8 |
                buffer[offset + 3]));
            offset += 4;
        }
        public static void Read(byte[] buffer, ref int offset, out int value)
        {
            value = unchecked((int)(
                buffer[offset + 0] << 24 |
                buffer[offset + 1] << 16 |
                buffer[offset + 2] << 8 |
                buffer[offset + 3]));
            offset += 4;
        }
        public static void Read(byte[] buffer, ref int offset, out ulong value) {
            value = unchecked((ulong)(
                buffer[offset + 0] << 56 |
                buffer[offset + 1] << 48 |
                buffer[offset + 2] << 40 |
                buffer[offset + 3] << 32 |
                buffer[offset + 4] << 24 |
                buffer[offset + 5] << 16 |
                buffer[offset + 6] << 8 |
                buffer[offset + 7]));
            offset += 8;
        }
        public static string Read(byte[] buffer, ref int offset) {
            var size = buffer[offset];
            var str = Encoding.UTF8.GetString(buffer, offset + 1, size);
            offset += size + 1;
            return str;
        }

        public static void Read(byte[] buffer, ref int offset, out string content)
        {
            content = string.Empty;

            while (offset < buffer.Length - 1)
            {
                Read(buffer, ref offset, out ushort value);

                if (value == 0)
                    break;

                content += Convert.ToChar(value);
            }
        }

        public static void Write(byte[] buffer, ref byte offset, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                Write(buffer, ref offset, Convert.ToUInt16(value[i]));
            }

            Write(buffer, ref offset, 0x00);
            Write(buffer, ref offset, 0x00);
        }

        public static void Write(byte[] buffer, ref byte offset, byte value) {
            buffer[offset++] = value;
        }

        public static void Write(byte[] buffer, ref byte offset, ushort value) {
            buffer[offset + 0] = unchecked((byte)(value >> 8));
            buffer[offset + 1] = unchecked((byte)value);
            offset += 2;
        }

        public static void Write(byte[] buffer, ref byte offset, uint value) {
            buffer[offset + 0] = unchecked((byte)(value >> 24));
            buffer[offset + 1] = unchecked((byte)(value >> 16));
            buffer[offset + 2] = unchecked((byte)(value >> 8));
            buffer[offset + 3] = unchecked((byte)value);
            offset += 4;
        }

        public static void Write(byte[] buffer, ref byte offset, ulong value) {
            buffer[offset + 0] = unchecked((byte)(value >> 56));
            buffer[offset + 1] = unchecked((byte)(value >> 48));
            buffer[offset + 2] = unchecked((byte)(value >> 40));
            buffer[offset + 3] = unchecked((byte)(value >> 32));
            buffer[offset + 4] = unchecked((byte)(value >> 24));
            buffer[offset + 5] = unchecked((byte)(value >> 16));
            buffer[offset + 6] = unchecked((byte)(value >> 8));
            buffer[offset + 7] = unchecked((byte)value);
            offset += 8;
        }

        public static void Write(byte [] buffer, ref byte offset, UUID value)
        {
            Write(buffer, ref offset, value.a);
            Write(buffer, ref offset, value.b);
            Write(buffer, ref offset, value.c);
            Write(buffer, ref offset, value.d);
            Write(buffer, ref offset, value.e);
            Write(buffer, ref offset, value.f);
            Write(buffer, ref offset, value.g);
            Write(buffer, ref offset, value.h);
            Write(buffer, ref offset, value.i);
            Write(buffer, ref offset, value.j);
            Write(buffer, ref offset, value.k);
        }

        public static void Write(byte[] buffer, ref byte offset, byte [] values)
        {
            foreach (Byte b in values)
                Write(buffer, ref offset, b);
        }

        public static void Write(byte[] buffer, ref byte offset, string value, int byteLength) {
            buffer[offset] = unchecked((byte)byteLength);

            var length = Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, offset);
            if (length != byteLength) throw new ArgumentException();

            offset += Convert.ToByte(length);
        }

        public static byte[] SplitByteArray(byte [] buffer, int offset, int size)
        {
            byte[] bufferSection = new byte[size];

            Array.Copy(buffer, offset, bufferSection, 0, size);

            return bufferSection;
        }
    }

#pragma warning disable 1591
    // Replacement for <see cref="Guid"/> with improvements for better memory usage.
    [Serializable]
    public struct UUID : IEquatable<UUID> {
        public const int SIZE = 16;

        public uint a;
        public ushort b, c;
        public byte d, e, f, g, h, i, j, k;

        public UUID(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k) {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
            this.h = h;
            this.i = i;
            this.j = j;
            this.k = k;
        }

        public bool isValid =>
            a != 0 &&
            b != 0 &&
            c != 0 &&
            d != 0 &&
            e != 0 &&
            f != 0 &&
            g != 0 &&
            h != 0 &&
            i != 0 &&
            j != 0 &&
            k != 0;

        bool IEquatable<UUID>.Equals(UUID o) {
            return a == o.a
                && b == o.b
                && c == o.c
                && d == o.d
                && e == o.e
                && f == o.f
                && g == o.g
                && h == o.h
                && i == o.i
                && j == o.j
                && k == o.k;
        }

        public void Read(byte[] buffer, ref int offset) {
            Protocol.Read(buffer, ref offset, out a);
            Protocol.Read(buffer, ref offset, out b);
            Protocol.Read(buffer, ref offset, out c);

            d = buffer[offset + 0];
            e = buffer[offset + 1];
            f = buffer[offset + 2];
            g = buffer[offset + 3];
            h = buffer[offset + 4];
            i = buffer[offset + 5];
            j = buffer[offset + 6];
            k = buffer[offset + 7];

            offset += 8;
        }

        public void Write(byte[] buffer, ref byte offset) {
            Protocol.Write(buffer, ref offset, a);
            Protocol.Write(buffer, ref offset, b);
            Protocol.Write(buffer, ref offset, c);

            buffer[offset + 0] = d;
            buffer[offset + 1] = e;
            buffer[offset + 2] = f;
            buffer[offset + 3] = g;
            buffer[offset + 4] = h;
            buffer[offset + 5] = i;
            buffer[offset + 6] = j;
            buffer[offset + 7] = k;

            offset += 8;
        }

        public override string ToString() => $"{a:x8}-{b:x4}-{c:x4}-{d:x2}{e:x2}-{f:x2}{g:x2}{h:x2}{i:x2}{j:x2}{k:x2}";

        public void FromString(string value) {
            if (value.Length != 36 || value[8] != '-' || value[13] != '-' || value[18] != '-' || value[23] != '-')
                throw new ArgumentException();

            a = unchecked((uint)(
                (ParseChar(value[0]) << 28) |
                (ParseChar(value[1]) << 24) |
                (ParseChar(value[2]) << 20) |
                (ParseChar(value[3]) << 16) |
                (ParseChar(value[4]) << 12) |
                (ParseChar(value[5]) << 8) |
                (ParseChar(value[6]) << 4) |
                (ParseChar(value[7]) << 0)));

            b = unchecked((ushort)(
                (ParseChar(value[9]) << 12) |
                (ParseChar(value[10]) << 8) |
                (ParseChar(value[11]) << 4) |
                (ParseChar(value[12]) << 0)));

            c = unchecked((ushort)(
                (ParseChar(value[14]) << 12) |
                (ParseChar(value[15]) << 8) |
                (ParseChar(value[16]) << 4) |
                (ParseChar(value[17]) << 0)));

            d = unchecked((byte)(
                (ParseChar(value[19]) << 4) |
                (ParseChar(value[20]) << 0)));
            e = unchecked((byte)(
                (ParseChar(value[21]) << 4) |
                (ParseChar(value[22]) << 0)));

            f = unchecked((byte)(
                (ParseChar(value[24]) << 4) |
                (ParseChar(value[25]) << 0)));
            g = unchecked((byte)(
                (ParseChar(value[26]) << 4) |
                (ParseChar(value[27]) << 0)));
            h = unchecked((byte)(
                (ParseChar(value[28]) << 4) |
                (ParseChar(value[29]) << 0)));
            i = unchecked((byte)(
                (ParseChar(value[30]) << 4) |
                (ParseChar(value[31]) << 0)));
            j = unchecked((byte)(
                (ParseChar(value[32]) << 4) |
                (ParseChar(value[33]) << 0)));
            k = unchecked((byte)(
                (ParseChar(value[34]) << 4) |
                (ParseChar(value[35]) << 0)));
        }

        static int ParseChar(char c) {
            if (c >= '0' && c <= '9') return c - '0';

            var n = c | 0x20;
            if (n < 'a' || n > 'f') throw new ArgumentException();

            return n - 'a' + 10;
        }
    }
#pragma warning restore 1591
}


// Protocol Information
// --------------------

// Frame Header, 3 bytes
// ---------------------
// - $00 byte msgType     (see MsgType)
// - $01 ushort frameSize (including header)
// - $03 Frame Data       (of size frameSize-3, or next message if frameSize==3)

// String Encoding, UTF-8 byte-length + 1
// --------------------------------------
// - $00 byte strSize     (maximum of 256 bytes)
// - $01 String data      (of length strSize)

// Ping, 0 bytes
// -------------
// Sent at regular intervals to ensure the connection is still alive.

// Version, 3 byte
// ---------------
// Exchanged starting from the client after a connection is established.
// Followed by the Start message after receiving a response from the server.
//
// - $00 byte version     (maximum 256 version numbers)
// - $01 byte platform    Windows, MacOS, Linux, etc
// - $02 byte engine      Unity, Unreal, etc

// Start, 2 bytes + strings
// ------------------------
// Sent from the client after a successful version exchange. Initiates the game session.
// Success: if the game session is initialized.
// Error: if a game session is already active.
// Error: if the game key is invalid or trying to reuse effect identifiers with a different game key.
// Error: if count==0 and no list was previously sent.
//
// - $00 UUID gameKey      game identifier
// - $10 ushort count      (number of effect identifiers, 0x00 to reuse previously sent list)
// - $12 strings data      (count strings: ordered list of effect identifiers)

// Stop, 0 bytes
// -------------
// Sent from the client to end the game session.
// Success: if the game session was terminated.
// Error: if no game session is currently active.

// Reset, 0 bytes
// --------------
// Sent from the client to restore all effects marked as Unavailable.
// Success: if the effects were restored.
// Error: if no game session is currently active.

// Result, 5 bytes
// ---------------
// Sent from the client to notify the server of an effect action.
//
// - $00 uint id      (the effect's unique ID)
// - $04 byte result  (see ResultType)


// Message, string
// ---------------
// Sent from the server to cause the client to display a message to the user.
//
// - $00 string message

// Maintenance, 4 bytes + string
// ------------------------
// Sent from the server to indicate a maintenance window is near.
//
// - $00 uint timestamp  (Unix UTC timestamp of when the maintenance window starts)
// - $04 uint estimated  (how long, in second, the maintenance window is estimated to last)
// - $04 string alert

// Effect, 14 bytes
// ----------------
// Sent from the server to enqueue an effect instance.
// TODO parameters
//
// - $00 ushort effect (index of the effect to trigger)
// - $02 uint id       (unique identifier of the effect instance, used to send its Result back)
// - $06 ulong userId  (Twitch user ID, ulong.MaxValue for Crowd pooled effects)

// Success, 1 byte
// -------------------------
// Sent from the server to indicate the result of a client command.
//
// - $00 byte code     (0x00 indicates success, otherwise an error code)
