using fxl.codes.kisekae.data.Archives.Algorithms;
using fxl.codes.kisekae.data.Archives.LhHeaders;

namespace fxl.codes.kisekae.data.Archives.LhMethods;

internal abstract class BaseLhMethod(ref readonly Stream stream, LhHeader header, int slidingWindowBytes, int matchingWindowBytes, IAlgorithm? algorithm)
{
    public int SlidingWindow => slidingWindowBytes * 1024;
}

internal class Lzs(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 2, 17, null);

internal class Lz4(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 0, 0, null);

internal class Lh0(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 0, 0, null);

internal class Lh1(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 4, 60, new HuffmanDynamic());

internal class Lh2(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 8, 256, new HuffmanDynamic());

internal class Lh3(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 8, 256, new HuffmanStatic());

internal class Lh4(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 4, 256, new HuffmanStatic());

internal class Lh5(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 8, 256, new HuffmanStatic());

internal class Lh6(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 32, 256, new HuffmanStatic());

internal class Lh7(ref readonly Stream stream, LhHeader header) : BaseLhMethod(in stream, header, 64, 256, new HuffmanStatic());