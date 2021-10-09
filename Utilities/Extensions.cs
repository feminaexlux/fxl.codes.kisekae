using System.Collections;

namespace fxl.codes.kisekae.Utilities
{
    public static class Extensions
    {
        public static bool[] ToBoolArray(this BitArray bitArray)
        {
            var data = new bool[bitArray.Length];
            bitArray.CopyTo(data, 0);
            return data;
        }

        public static int IntValue(this bool[] data)
        {
            var bits = new BitArray(data);
            var value = new int[1];
            bits.CopyTo(value, 0);

            return value[0];
        }
    }
}