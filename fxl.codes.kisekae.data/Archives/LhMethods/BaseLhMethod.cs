using fxl.codes.kisekae.data.Archives.Algorithms;

namespace fxl.codes.kisekae.data.Archives.LhMethods;

internal abstract class BaseLhMethod(LhContainer container, int slidingWindowBytes, int matchingWindowBytes, IAlgorithm? algorithm)
{
    public int SlidingWindow => slidingWindowBytes * 1024;

    public void InitializeAlgorithm()
    {
    }
}

internal class Lzs(LhContainer container) : BaseLhMethod(container, 2, 17, null);

internal class Lz4(LhContainer container) : BaseLhMethod(container, 0, 0, null);

internal class Lh0(LhContainer container) : BaseLhMethod(container, 0, 0, null);

internal class Lh1(LhContainer container) : BaseLhMethod(container, 4, 60, new HuffmanDynamic());

internal class Lh2(LhContainer container) : BaseLhMethod(container, 8, 256, new HuffmanDynamic());

internal class Lh3(LhContainer container) : BaseLhMethod(container, 8, 256, new HuffmanStatic());

internal class Lh4(LhContainer container) : BaseLhMethod(container, 4, 256, new HuffmanStatic());

internal class Lh5(LhContainer container) : BaseLhMethod(container, 8, 256, new HuffmanStatic());

internal class Lh6(LhContainer container) : BaseLhMethod(container, 32, 256, new HuffmanStatic());

internal class Lh7(LhContainer container) : BaseLhMethod(container, 64, 256, new HuffmanStatic());