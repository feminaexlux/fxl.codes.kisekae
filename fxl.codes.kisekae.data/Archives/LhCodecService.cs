using fxl.codes.kisekae.data.Archives.LhHeaders;
using fxl.codes.kisekae.data.Entities;
using Microsoft.Extensions.Logging;

namespace fxl.codes.kisekae.data.Archives;

public class LhCodecService(ILogger<LhCodecService> logger)
{
    public async Task<Kisekae?> ReadArchive(Stream stream)
    {
        try
        {
            var header = new LhHeader(in stream);
            switch (header.MethodId)
            {
                case "-lh0-": break;
                case "-lzs-": break;
                case "-lz4-": break;
                case "-lh1-": break;
                case "-lh2-": break;
                case "-lh3-": break;
                case "-lh4-": break;
                case "-lh5-": break;
                case "-lh6-": break;
                case "-lh7-": break;
                default: throw new Exception("Unknown method");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to open archive");
        }

        return null;
    }
}