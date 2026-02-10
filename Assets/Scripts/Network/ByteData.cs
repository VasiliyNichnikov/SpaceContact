using System;
using Unity.Netcode;

namespace Network
{
    public struct ByteData : INetworkSerializable, IEquatable<ByteData>
    {
        private byte[]? _data;
        
        public ByteData(byte[]? data)
        {
            _data = data;
        }
        
        public byte[] Data => _data ?? Array.Empty<byte>();
        
        public bool IsEmpty => _data == null || _data.Length == 0;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            var length = 0;

            if (!serializer.IsReader)
            {
                length = _data?.Length ?? 0;
            }
            
            serializer.SerializeValue(ref length);

            if (serializer.IsReader)
            {
                _data = new byte[length];
            }

            if (length > 0)
            {
                serializer.SerializeValue(ref _data);
            }
        }

        public bool Equals(ByteData other)
        {
            if (_data == null && other._data == null)
            {
                return true;
            }

            if (_data == null || other._data == null)
            {
                return false;
            }

            if (_data.Length != other._data.Length)
            {
                return false;
            }

            // Побайтовое сравнением может быть дорогим, поэтому возвращаем false
            return false;
        }
    }
}