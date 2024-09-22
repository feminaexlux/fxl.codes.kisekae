using System.Text;

namespace fxl.codes.kisekae.data.Archives;

public class LzhExtractor
{
    public async void Open(Stream stream, string filename)
    {
        var headerSize = stream.ReadByte();
        var headerSum = stream.ReadByte();
        var methodBytes = new byte[5];
        await stream.ReadExactlyAsync(methodBytes);

        var method = Encoding.ASCII.GetString(methodBytes);
        var skipSize = await ReadLittleEndian(stream);
        var origSize = await ReadLittleEndian(stream);
        var time = await ReadLittleEndian(stream);
        var attribute = stream.ReadByte();
        var level = stream.ReadByte();
    }

    private static async Task<int> ReadLittleEndian(Stream stream, int length = 4)
    {
        var buffer = new byte[length];
        await stream.ReadExactlyAsync(buffer);
        var x = BitConverter.ToInt32(buffer);

        var b0 = (x >> 24) & 255;
        var b1 = (x >> 16) & 255;
        var b2 = (x >> 8) & 255;
        var b3 = (x >> 0) & 255;
        return (b0 << 0) + (b1 << 8) + (b2 << 16) + (b3 << 24);
    }
}