using System.Text;

namespace fxl.codes.kisekae.data.Archives.Algorithms;

internal class LZW : IAlgorithm
{
    private static readonly Dictionary<uint, string> DecodeDictionary;
    private static readonly Dictionary<string, uint> EncodeDictionary;
    private readonly int Max;

    static LZW()
    {
        DecodeDictionary = new Dictionary<uint, string>();
        EncodeDictionary = new Dictionary<string, uint>();

        for (uint i = 0; i <= byte.MaxValue; i++)
        {
            var value = $"{(char)i}";
            DecodeDictionary.Add(i, value);
            EncodeDictionary.Add(value, i);
        }
    }

    internal LZW(int max = short.MaxValue)
    {
        Max = max;
    }

    public byte[] Decode(Stream stream, int bitLength = 32)
    {
        var dictionary = DecodeDictionary.ToDictionary(x => x.Key, x => x.Value);
        uint next = byte.MaxValue + 1;
        var buffer = "";
        var decoded = new List<string>();
        var bitStream = new BitStream(stream);
        while (bitStream.Position < bitStream.Length)
        {
            var code = bitStream.ReadBits(bitLength);
            if (!dictionary.ContainsKey(code)) dictionary[code] = buffer + buffer[0];

            decoded.Add(dictionary[code]);
            if (buffer.Length > 0 && next <= Max) dictionary[next++] = buffer + dictionary[code][0];

            buffer = dictionary[code];
        }

        return decoded.SelectMany(Encoding.UTF8.GetBytes).ToArray();
    }

    public byte[] Encode(Stream stream)
    {
        uint next = byte.MaxValue + 1;
        var dictionary = EncodeDictionary.ToDictionary(x => x.Key, x => x.Value);

        var buffer = "";
        var encoded = new List<uint>();
        while (true)
        {
            if (stream.Position >= stream.Length) break;

            var charValue = (char)stream.ReadByte();
            buffer += charValue;
            if (dictionary.ContainsKey(buffer)) continue;
            if (next <= Max) dictionary.Add(buffer, next++);
            encoded.Add(dictionary[$"{buffer[..^1]}"]);
            buffer = $"{charValue}";
        }

        encoded.Add(dictionary[buffer]);
        return encoded.SelectMany(BitConverter.GetBytes).ToArray();
    }
}