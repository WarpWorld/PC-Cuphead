namespace WarpWorld.CrowdControl
{
    public class PendingMessage
    {
        public byte[] _bytes;
        public byte _msgType;
        public int _size;

        public PendingMessage(byte[] bytes, byte msgType, int size)
        {
            _bytes = bytes;
            _msgType = msgType;
            _size = size;
        }
    }
}
