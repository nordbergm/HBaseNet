using System.Collections.Generic;
using System.Linq;

namespace HBaseNet.Protocols.Thrift
{
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SequenceEqual(y);
        }

        public int GetHashCode(byte[] obj)
        {
            if (obj == null || obj.Length == 0)
            {
                return 0;
            }

            int i = obj[0].GetHashCode();

            for (int j = 1; j < obj.Length; j++)
            {
                i ^= j;
            }

            return i;
        }
    }
}
