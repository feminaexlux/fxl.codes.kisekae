namespace fxl.codes.kisekae.data.Archives;

/**
 * Ported from https://github.com/kisekae/UltraKiss/blob/master/src/Kisekae/Lhhuf.java
 */
public class LhArchiveDecoder
{
    private const int Buffer = 4096;
    private const int BufferSize = 60;
    private const int Threshold = 2;
    private const int EndOfFile = -1;
    private const int NChar = 256 - Threshold + BufferSize; // For some reason????
    private const int TableSize = NChar * 2 - 1;
    private const int RootPosition = TableSize - 1;
    private const int MaxFreq = 0x8000;
    private readonly int[] _child = new int[TableSize];
    private readonly int[] _frequency = new int[TableSize];
    private readonly int[] _parent = new int[TableSize + NChar];

    private int _bytesProcessed;
    private int[] _data = new int[Buffer + 1];
    private int _lastBytesProcessed;
    private int[] _leftChild = new int[Buffer + 1];
    private int[] _rightChild = new int[Buffer * 2 + 1];
    private int[] _same = new int[Buffer + 1];
    private byte[] _textBuffer = new byte[Buffer + BufferSize - 1];
    private int putlen, getlen, putbuf, getbuf;

    private void Initialize()
    {
        for (var i = 0; i < NChar; i++)
        {
            _frequency[i] = 1;
            _child[i] = i + TableSize;
            _parent[i + TableSize] = i;
        }

        for (int i = 0, j = NChar; j <= RootPosition; i += 2, j++)
        {
            _frequency[j] = _frequency[i] + _frequency[i + 1];
            _child[j] = i;
            _parent[i] = _parent[i + 1] = j;
        }

        _frequency[TableSize] = 0xffff;
        _parent[RootPosition] = 0;
    }

    private void Reconstruct()
    {
        int k;
        for (int i = 0, j = 0; i < TableSize; i++, j++)
        {
            if (_child[i] < TableSize) continue;

            _frequency[j] = (_frequency[i] + 1) / 2;
            _child[j] = _child[i];
        }

        for (int i = 0, j = NChar; j < TableSize; i += 2, j++)
        {
            k = i + 1;
            var f = _frequency[j] = _frequency[i] + _frequency[k];
            for (k = j - 1; f < _frequency[k]; k--);
            k++;

            for (int p = j, e = k; p > e; p--) _frequency[p] = _frequency[p - 1];
            _frequency[k] = f;

            for (var p = j; p > k; p--) _child[p] = _child[p - 1];
            _child[k] = i;
        }

        /* link parents */
        for (var i = 0; i < TableSize; i++)
            if ((k = _child[i]) >= TableSize)
                _parent[k] = i;
            else
                _parent[k] = _parent[k + 1] = i;
    }
}