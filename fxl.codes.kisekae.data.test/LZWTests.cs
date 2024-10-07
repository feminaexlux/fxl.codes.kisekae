using fxl.codes.kisekae.data.Archives.Algorithms;

namespace fxl.codes.kisekae.data.test;

public class LZWTests
{
    private readonly byte[] _encoded = new[] { 65, 66, 66, 256, 257, 259, 65 }.SelectMany(BitConverter.GetBytes).ToArray();
    private readonly byte[] _unencoded = "ABBABBBABBA"u8.ToArray();
    private LZW _lzw;

    [SetUp]
    public void Setup()
    {
        _lzw = new LZW();
    }

    [Test]
    public void TestEncodingAndDecodingBasic()
    {
        Assert.Multiple(() =>
        {
            var streamUnencoded = new MemoryStream(_unencoded);
            var streamEncoded = new MemoryStream(_encoded);
            Assert.That(_lzw.Encode(streamUnencoded), Is.EqualTo(_encoded), "Encoding failed");
            Assert.That(_lzw.Decode(streamEncoded), Is.EqualTo(_unencoded), "Decoding failed");
        });
    }
}