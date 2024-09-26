using fxl.codes.kisekae.data.Entities;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.data.Archives;

public class LhCodecService(ILogger<LhCodecService> logger)
{
    public async Task<Kisekae?> ReadArchive(Stream stream)
    {
        throw new NotImplementedException();
    }

    internal static IEnumerable<LhContainer> GetFiles(ref readonly Stream stream)
    {
        var files = new List<LhContainer>();
        while (stream.Position + 21 < stream.Length) files.Add(new LhContainer(in stream));
        return files;
    }
}