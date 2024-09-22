using System.Buffers.Binary;

namespace fxl.codes.kisekae.data.Archives;

internal static class EndianUtility
{
    public static int ToLittleEndian(this ReadOnlySpan<byte> bytes)
    {
        var x = BitConverter.ToInt32(bytes);
        return BitConverter.IsLittleEndian ? x : BinaryPrimitives.ReverseEndianness(x);
    }

    public static int ReadLittleEndian(this Stream stream, int length = 4)
    {
        var buffer = new byte[length];
        stream.ReadExactly(buffer);
        return ToLittleEndian(buffer);
    }

    public static async Task<int> ReadLittleEndianAsync(this Stream stream, int length = 4)
    {
        var buffer = new byte[length];
        await stream.ReadExactlyAsync(buffer);
        return ToLittleEndian(buffer);
    }

    public static DateTime ToDateTime(this ReadOnlySpan<byte> bytes)
    {
        var time = ToLittleEndian(bytes);
        return ToDateTime(time);
    }

    private static DateTime ToDateTime(int time)
    {
        var year = ((time >> 25) & 0x7f) + 1980;
        var month = (time >> 21) & 0x0f;
        var day = (time >> 16) & 0x1f;
        var hour = (time >> 11) & 0x1f;
        var minute = (time >> 5) & 0x3f;
        var second = (time & 0x1f) * 2;

        if (month is < 1 or > 12 || day is < 1 or > 31 || hour >= 24 || minute >= 60 || second > 60) return DateTime.MinValue;

        return new DateTime(year, month, day, hour, minute, second);
    }

    public static DateTime ReadLittleEndianDateTime(this Stream stream)
    {
        var time = ReadLittleEndian(stream);
        return ToDateTime(time);
    }

    public static async Task<DateTime> ReadLittleEndianDateTimeAsync(this Stream stream)
    {
        var time = await ReadLittleEndianAsync(stream);
        return ToDateTime(time);
    }
}