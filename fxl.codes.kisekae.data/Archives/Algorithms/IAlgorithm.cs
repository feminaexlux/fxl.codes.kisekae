namespace fxl.codes.kisekae.data.Archives.Algorithms;

public interface IAlgorithm
{
    byte[] Encode(ReadOnlySpan<byte> stream);
    byte[] Decode(ReadOnlySpan<byte> stream);
}