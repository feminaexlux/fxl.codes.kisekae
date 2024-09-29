using fxl.codes.kisekae.data.Archives;
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
            Assert.That(_lzw.Encode(_unencoded), Is.EqualTo(_encoded), "Encoding failed");
            Assert.That(_lzw.Decode(_encoded), Is.EqualTo(_unencoded), "Decoding failed");
        });
    }

    [Test]
    public void TestDecompressLzh()
    {
        var assembly = typeof(LZWTests).Assembly;
        foreach (var resource in assembly.GetManifestResourceNames())
        {
            using var stream = assembly.GetManifestResourceStream(resource);
            if (stream == null) continue;

            var files = LhCodecService.GetFiles(in stream);
            foreach (var file in files)
                Assert.Multiple(() =>
                {
                    Assert.That(file.CompressedFile, Is.Not.Null);
                    Assert.That(file.CompressedFile, Is.Not.Empty);

                    var decompressed = _lzw.Decode(file.CompressedFile);
                    Assert.That(decompressed, Is.Not.Null);
                });
        }
    }
}