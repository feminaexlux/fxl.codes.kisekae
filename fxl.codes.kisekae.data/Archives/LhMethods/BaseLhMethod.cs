using fxl.codes.kisekae.data.Archives.Algorithms;

namespace fxl.codes.kisekae.data.Archives.LhMethods;

internal abstract class BaseLhMethod(LhContainer container, int slidingWindowBytes, int matchingWindowBytes, IAlgorithm? algorithm = null)
{
    internal static readonly IAlgorithm Lzw = new LZW();
    public int SlidingWindow => slidingWindowBytes * 1024;

    public void InitializeAlgorithm()
    {
    }
}

internal class Lzs(LhContainer container) : BaseLhMethod(container, 2, 17);

internal class Lz4(LhContainer container) : BaseLhMethod(container, 0, 0);

internal class Lh0(LhContainer container) : BaseLhMethod(container, 0, 0);

internal class Lh1(LhContainer container) : BaseLhMethod(container, 4, 60, Lzw);

internal class Lh2(LhContainer container) : BaseLhMethod(container, 8, 256, Lzw);

internal class Lh3(LhContainer container) : BaseLhMethod(container, 8, 256, Lzw);

internal class Lh4(LhContainer container) : BaseLhMethod(container, 4, 256, Lzw);

internal class Lh5(LhContainer container) : BaseLhMethod(container, 8, 256, Lzw);

internal class Lh6(LhContainer container) : BaseLhMethod(container, 32, 256, Lzw);

internal class Lh7(LhContainer container) : BaseLhMethod(container, 64, 256, Lzw);