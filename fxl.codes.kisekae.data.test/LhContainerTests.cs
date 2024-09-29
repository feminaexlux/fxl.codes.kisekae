using System.Reflection;
using fxl.codes.kisekae.data.Archives;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace fxl.codes.kisekae.data.test;

public class LhContainerTests
{
    private readonly Assembly _assembly = typeof(LhContainerTests).Assembly;
    private readonly List<string> _resourceNames = new();
    private LhCodecService _service;

    [SetUp]
    public void Setup()
    {
        foreach (var name in _assembly.GetManifestResourceNames()) _resourceNames.Add(name);
        var logger = new Logger<LhCodecService>(NullLoggerFactory.Instance);
        _service = new LhCodecService(logger);
    }

    [Test]
    public void TestHeader()
    {
        foreach (var resource in _resourceNames)
        {
            using var stream = _assembly.GetManifestResourceStream(resource);
            if (stream == null) continue;

            var files = LhCodecService.GetFiles(in stream);

            Assert.Multiple(() =>
            {
                Assert.That(files, Is.Not.Empty);

                foreach (var file in files)
                {
                    Assert.That(file.FileName, Is.Not.Null, "Filename is {0}", file.FileName);
                    Assert.That(file.CompressedSize, Is.GreaterThan(0), "Compressed size is {0}", file.CompressedSize);
                    Assert.That(file.UncompressedSize, Is.GreaterThan(0), "Uncompressed size is {0}", file.UncompressedSize);
                    Assert.That(file.UncompressedSize, Is.GreaterThan(file.CompressedSize), "Uncompressed is larger than compressed");
                    Assert.That(file.MethodId, Is.Not.Null, "Method id is {0}", file.MethodId);
                    Assert.That(file.MethodId, Does.StartWith("-l"));
                    Assert.That(file.MethodId, Does.EndWith("-"));
                    Console.WriteLine($"Archive name: {file.FileName}, compression method: {file.MethodId}, file at {file.FileDataPosition}");
                }
            });
        }
    }

    [Test]
    public void TestBody()
    {
        foreach (var resource in _resourceNames)
        {
            using var stream = _assembly.GetManifestResourceStream(resource);
            if (stream == null) continue;

            var files = LhCodecService.GetFiles(in stream);
            foreach (var file in files)
                Assert.Multiple(() => { Assert.That(file.CompressedFile, Is.Not.Null); });
        }
    }
}