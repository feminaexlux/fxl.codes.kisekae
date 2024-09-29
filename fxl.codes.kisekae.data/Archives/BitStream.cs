namespace fxl.codes.kisekae.data.Archives;

public class BitStream : Stream
{
    private readonly Stream _stream;
    private uint _bitBuffer;
    private int _bitBufferCount;
    private long _length;

    public BitStream(Stream stream)
    {
        _stream = stream;
#if DEBUG
        var min = Math.Min(stream.Length, 4);
        var bytes = new byte[min];
        _stream.ReadExactly(bytes);
        var temp = BitConverter.ToUInt32(bytes);
        Console.WriteLine($"BitStream first 4 bytes: {temp:b}");
        _stream.Position = 0;
#endif
        _length = _stream.Length * 8;
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _length;

    public override long Position { get; set; }

    public uint ReadBits(int bitLength)
    {
        // Fill buffer
        if (_bitBufferCount <= 24)
            while (_bitBufferCount < 32 && _stream.Position < _stream.Length)
            {
                _bitBuffer <<= 8;
                _bitBuffer |= (uint)_stream.ReadByte();
                _bitBufferCount += 8;
            }

        var buffer = _bitBuffer;
        buffer >>= 32 - bitLength;
#if DEBUG
        Console.WriteLine($"BitStream buffer: {_bitBuffer:b}, extracted number: {buffer:b}");
        _stream.Position = 0;
#endif
        _bitBuffer <<= bitLength;
        Position += bitLength;
        _bitBufferCount -= bitLength;

        return buffer;
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        _length = value;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}