using System.Text;

namespace fxl.codes.kisekae.data.Archives.Algorithms;

internal class LZW : IAlgorithm
{
    private readonly int Max;

    internal LZW(int max = short.MaxValue)
    {
        Max = max;
    }

    public byte[] Decode(ReadOnlySpan<byte> stream)
    {
        uint next = byte.MaxValue + 1;
        var dictionary = new Dictionary<uint, string>();
        for (uint i = 0; i <= byte.MaxValue; i++) dictionary.Add(i, $"{(char)i}");

        var buffer = "";
        var decoded = new List<string>();
        for (var i = 0; i < stream.Length; i += 4)
        {
            var code = BitConverter.ToUInt32(stream[i..(i + 4)]);
            if (!dictionary.ContainsKey(code)) dictionary[code] = buffer + buffer[0];

            decoded.Add(dictionary[code]);
            if (buffer.Length > 0 && next <= Max) dictionary[next++] = buffer + dictionary[code][0];

            buffer = dictionary[code];
        }

        return decoded.SelectMany(Encoding.UTF8.GetBytes).ToArray();
    }

    public byte[] Encode(ReadOnlySpan<byte> stream)
    {
        var next = byte.MaxValue + 1;
        var dictionary = new Dictionary<string, int>();
        for (var i = 0; i <= byte.MaxValue; i++) dictionary.Add($"{(char)i}", i);

        var buffer = "";
        var encoded = new List<int>();
        foreach (var part in stream)
        {
            var charValue = (char)part;
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