using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;

namespace hashes
{
    public class ReadonlyBytes : IEnumerable<byte>
    {
        internal readonly int Length;

        internal readonly byte[] Data;

        private int? hash;


        private static int EvaluateHash(ReadonlyBytes data)
        {
            if ((data.Length == 0)) return 0;
            ulong h = 2166136261;
            ulong p = 16777619;
            unchecked
            {
                foreach (byte b in data.Data)
                {
                    h *= p;
                    h &= 0xffffffff;
                    h ^= b;
                }
            }
            return ConvertToInt(h);
        }

        public ReadonlyBytes(params byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("hollow data");
            Length = data.Length;
            Data = data;
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                return Data[index];
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
                yield return Data[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('[');
            foreach (byte b in Data)
                builder.Append(b.ToString() + ", ");
            if (Length > 0)
                builder.Remove(builder.Length - 2, 2);
            builder.Append(']');
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is not ReadonlyBytes) return false;
            if ((obj.GetType() != this.GetType())) return false;

            var data = obj as ReadonlyBytes;

            if (data.Length != Length)
                return false;

            bool result = true;
            for (int i = 0; i < data.Length; i++)
            {
                result &= (Data[i] == data[i]);
            }
            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (hash != null) return hash.Value;
                hash = EvaluateHash(this);
                return hash.Value;
            }
        }

        public static int ConvertToInt(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }

        //public static ulong CutElders(ulong value)
        //{
        //    var valueBytes = BitConverter.GetBytes(value);
        //    for (int i = 4; i < 8; i++)
        //        valueBytes[i] = 0;
        //    return BitConverter.ToUInt64(valueBytes);
        //}
    }
}