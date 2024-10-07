namespace fxl.codes.kisekae.data.Archives.Algorithms;

public interface IAlgorithm
{
    byte[] Encode(Stream stream);
    byte[] Decode(Stream stream, int bitLength);
}