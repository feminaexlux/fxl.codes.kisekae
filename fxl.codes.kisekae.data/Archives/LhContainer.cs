using System.Text;

namespace fxl.codes.kisekae.data.Archives;

internal class LhContainer
{
    public LhContainer(ref readonly Stream stream)
    {
        HeaderDataPosition = stream.Position;
        var bytes = new byte[21];
        stream.ReadExactly(bytes);

        var span = bytes.AsSpan();
        Level = span[20];
        MethodId = Encoding.ASCII.GetString(span[2..7]);
        CompressedSize = EndianUtility.ToLittleEndian(span[7..11]);
        CompressedFile = new byte[CompressedSize];
        UncompressedSize = EndianUtility.ToLittleEndian(span[11..15]);
        Created = EndianUtility.ToDateTime(span[15..19]);
        FileOrDirectory = span[19];

        switch (Level)
        {
            case 0:
                Level0(span, in stream);
                break;
            case 1:
                Level1(span, in stream);
                break;
            case 2:
                Level2(span, in stream);
                break;
            default: throw new Exception("Invalid LH Header");
        }
    }

    public int HeaderSize { get; private set; }
    public int HeaderCheckSum { get; private set; }
    public string MethodId { get; private set; }
    public int CompressedSize { get; }
    public int UncompressedSize { get; private set; }
    public DateTime Created { get; private set; }
    public int FileOrDirectory { get; private set; }
    public int Level { get; }
    public short Crc16 { get; private set; }

    public string? FileName { get; private set; }

    public long HeaderDataPosition { get; }
    public long FileDataPosition { get; private set; }
    public byte[] CompressedFile { get; }

    private void Level0(ReadOnlySpan<byte> span, ref readonly Stream stream)
    {
        HeaderSize = span[0];
        HeaderCheckSum = span[1];
        FileName = GetFilename(in stream);
        Crc16 = (short)stream.ReadLittleEndian(2);

        FileDataPosition = stream.Position;
        stream.ReadExactly(CompressedFile);
    }

    private void Level1(ReadOnlySpan<byte> span, ref readonly Stream stream)
    {
        HeaderSize = span[0];
        HeaderCheckSum = span[1];
        FileName = GetFilename(in stream);
        Crc16 = (short)stream.ReadLittleEndian(2);
        OperatingSystem = (char)stream.ReadByte();

        SetExtendedHeaders(in stream);

        FileDataPosition = stream.Position;
        stream.ReadExactly(CompressedFile);
    }

    private void Level2(ReadOnlySpan<byte> span, ref readonly Stream stream)
    {
        HeaderSize = span[..2].ToLittleEndian();
        Crc16 = (short)stream.ReadLittleEndian(2);
        OperatingSystem = (char)stream.ReadByte();

        SetExtendedHeaders(in stream);

        FileDataPosition = stream.Position;
        stream.ReadExactly(CompressedFile);
    }

    private static string GetFilename(ref readonly Stream stream)
    {
        var length = stream.ReadByte();
        var buffer = new byte[length];
        stream.ReadExactly(buffer);
        return Encoding.ASCII.GetString(buffer);
    }

    private void SetExtendedHeaders(ref readonly Stream stream)
    {
        var extendedHeaderSize = stream.ReadLittleEndian(2);
        while (extendedHeaderSize != 0)
        {
            var type = stream.ReadByte();
            var nextSize = stream.ReadLittleEndian(2);

            switch (type)
            {
                case 0x00:
                    Crc16 = (short)stream.ReadLittleEndian(extendedHeaderSize);
                    break;
                case 0x01:
                    FileName = GetString(in stream, extendedHeaderSize);
                    break;
                case 0x02:
                    DirectoryName = GetString(in stream, extendedHeaderSize);
                    break;
                case 0x3f:
                    Comment = GetString(in stream, extendedHeaderSize);
                    break;
                case 0x50:
                    FilePermission = stream.ReadLittleEndian(extendedHeaderSize);
                    break;
                case 0x51:
                    GroupId = stream.ReadLittleEndian(2);
                    UserId = stream.ReadLittleEndian(2);
                    break;
                case 0x52:
                    GroupName = GetString(in stream, extendedHeaderSize);
                    break;
                case 0x53:
                    UserName = GetString(in stream, extendedHeaderSize);
                    break;
                case 0x54:
                    LastModified = stream.ReadLittleEndianDateTime();
                    break;
                default: // Unknown, skip
                    stream.Position += extendedHeaderSize;
                    break;
            }

            extendedHeaderSize = nextSize;
        }
    }

    private static string GetString(ref readonly Stream stream, int length)
    {
        var buffer = new byte[length];
        stream.ReadExactly(buffer);
        return Encoding.ASCII.GetString(buffer);
    }

    #region Extended Headers

    public char? OperatingSystem { get; private set; }
    public string? DirectoryName { get; private set; }
    public string? Comment { get; private set; }
    public int? FilePermission { get; private set; }
    public int? GroupId { get; private set; }
    public string? GroupName { get; private set; }
    public int? UserId { get; private set; }
    public string? UserName { get; private set; }
    public DateTime? LastModified { get; private set; }

    #endregion
}