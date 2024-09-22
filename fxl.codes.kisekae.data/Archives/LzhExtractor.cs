using System.Text;

namespace fxl.codes.kisekae.data.Archives;

public class LzhExtractor
{
    private int _crc16 = -1;
    private int _crc32 = -1;
    private DateTime _created;
    private string _method;
    private string _name;
    private int _sizeCompressed;
    private int _sizeCompressedExtended;
    private int _sizeExtension;
    private int _sizeHeader;
    private int _sizeUncompressed;
    public long DataPosition;

    public async Task ReadHeader(Stream stream)
    {
        _sizeHeader = stream.ReadByte();
        if (_sizeHeader <= 0) throw new Exception("Zero sized LHA file");

        var headerSum = stream.ReadByte();
        var methodBytes = new byte[5];
        await stream.ReadExactlyAsync(methodBytes);
        _method = Encoding.ASCII.GetString(methodBytes);
        if (string.IsNullOrEmpty(_method) || !_method.StartsWith('-')) throw new Exception("Invalid LHA file signature");

        _sizeExtension = 0;
        _sizeCompressedExtended = await ReadLittleEndian(stream);
        _sizeUncompressed = await ReadLittleEndian(stream);
        var time = await ReadLittleEndian(stream);
        _created = GetDateTime(time);

        var attribute = stream.ReadByte();
        var level = stream.ReadByte();
        int next;

        switch (level)
        {
            case 0:
                _sizeCompressed = _sizeCompressedExtended;
                _name = await GetStringValue(stream, stream.ReadByte());
                _crc16 = await ReadLittleEndian(stream, 2);
                next = 0;
                break;
            case 1:
                _name = await GetStringValue(stream, stream.ReadByte());
                _crc16 = await ReadLittleEndian(stream, 2);
                stream.Position++;
                next = await ReadLittleEndian(stream, 2);
                break;
            case 2:
                _sizeCompressed = _sizeCompressedExtended;
                _crc16 = await ReadLittleEndian(stream, 2);
                stream.Position++;
                next = await ReadLittleEndian(stream, 2);
                break;
            default: throw new Exception("Invalid LHA file header");
        }

        if (level > 0)
            while (next != 0)
            {
                _sizeExtension += next;
                var type = stream.ReadByte();
                var size = next - 3;

                switch (type)
                {
                    case 0: break;
                    case 1:
                        _name = "";
                        for (; size > 0; size--) _name += (char)stream.ReadByte();
                        break;
                    case 2:
                    case 0x52:
                    case 0x53:
                        stream.Position += size;
                        size = 0;
                        break;
                    case 0x3f: break;
                    case 0x40:
                    case 0x50:
                        stream.Position += 2;
                        size -= 2;
                        break;
                    case 0x51:
                    case 0x54:
                        stream.Position += 4;
                        size -= 4;
                        break;
                }

                stream.Position += size;
                next = await ReadLittleEndian(stream, 2);
            }

        if (level == 1) _sizeCompressed = _sizeCompressedExtended - _sizeExtension;

        DataPosition = level switch
        {
            0 => _sizeHeader + 2,
            1 => _sizeHeader + _sizeExtension + 2,
            2 => _sizeHeader,
            _ => DataPosition
        };
    }

    private static async Task<string> GetStringValue(Stream stream, int length)
    {
        var buffer = new byte[length];
        await stream.ReadExactlyAsync(buffer);
        return Encoding.ASCII.GetString(buffer);
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

    private static DateTime GetDateTime(int t)
    {
        var year = ((t >> 25) & 0x7f) + 1980;
        var month = (t >> 21) & 0x0f;
        var day = (t >> 16) & 0x1f;
        var hour = (t >> 11) & 0x1f;
        var minute = (t >> 5) & 0x3f;
        var second = (t & 0x1f) * 2;

        if (month is < 1 or > 12 || day is < 1 or > 31 || hour >= 24 || minute >= 60 || second > 60) return DateTime.MinValue;

        return new DateTime(year, month, day, hour, minute, second);
    }
}