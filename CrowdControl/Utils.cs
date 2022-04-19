using System;

namespace WarpWorld.CrowdControl
{
    public static class Utils
    {
        public static UUID GenerateRandomUUID()
        {
            UUID newUUID = new UUID();
            newUUID.a = RandomUInt();
            newUUID.b = RandomUShort();
            newUUID.c = RandomUShort();
            newUUID.d = RandomByte();
            newUUID.e = RandomByte();
            newUUID.f = RandomByte();
            newUUID.g = RandomByte();
            newUUID.h = RandomByte();
            newUUID.i = RandomByte();
            newUUID.j = RandomByte();
            newUUID.k = RandomByte();

            return newUUID;
        }

        public static ushort RandomUShort()
        {
            return Convert.ToUInt16(UnityEngine.Random.Range(0, UInt16.MaxValue));
        }

        public static bool RandomBool()
        {
            return UnityEngine.Random.Range(0, 2) >= 1;
        }

        public static byte RandomByte()
        {
            return Convert.ToByte(UnityEngine.Random.Range(0, byte.MaxValue));
        }

        public static uint RandomUInt()
        {
            uint num = Convert.ToUInt32(UnityEngine.Random.Range(0, Int32.MaxValue));

            if (Utils.RandomBool())
                num += Int32.MaxValue;

            return num;
        }

        public static ulong Randomulong()
        {
            ulong value = Convert.ToUInt64(UnityEngine.Random.Range(0, int.MaxValue));
            value += Convert.ToUInt64(UnityEngine.Random.Range(0, int.MaxValue)) * 0x100000000;

            int final = UnityEngine.Random.Range(0, 4);

            if (final % 1 == 0)
                value += 0x80000000;

            if (final % 2 == 0)
                value += 0x8000000000000000;

            return value;
        }
    }
}
