using fxl.codes.kisekae.data.Archives;

namespace fxl.codes.kisekae.data.test;

public class BitStreamTests
{
    [Test]
    public void TestReadBasic()
    {
        var bytes = BitConverter.GetBytes(uint.MaxValue);
        var stream = new MemoryStream(bytes);

        var bitStream = new BitStream(stream);
        var value = bitStream.ReadBits(8);
        Assert.That(value, Is.EqualTo(byte.MaxValue), "8 bits match failed");

        value = bitStream.ReadBits(24);
        Assert.Multiple(() =>
        {
            Assert.That(value, Is.EqualTo(Math.Pow(2, 24) - 1), "24 bits match failed");
            Assert.That(bitStream.Position, Is.EqualTo(bitStream.Length), "Stream position is wrong");
        });
    }

    [Test]
    public void TestReadRandom()
    {
        var bytes = BitConverter.GetBytes(uint.MaxValue);
        var stream = new MemoryStream(bytes);

        var bitStream = new BitStream(stream);
        var list = GetRandomNumbersSummingTo(32);

        foreach (var num in list)
        {
            var value = bitStream.ReadBits(num);
            Assert.That(value, Is.EqualTo(Math.Pow(2, num) - 1), $"{num} bits match failed");
        }
    }

    [Test]
    public void TestReadLong()
    {
        var random = (uint)Random.Shared.Next();
        var bytes = BitConverter.GetBytes(random);
        var stream = new MemoryStream(bytes);

        var bitStream = new BitStream(stream);
        var list = GetRandomNumbersSummingTo(32);

        var runningTotal = 0;
        foreach (var num in list)
        {
            var temp = random;
            temp <<= runningTotal;
            temp >>= 32 - num;
            Console.WriteLine($"Original value: {random:b}, bit shifted number: {temp:b}");
            var value = bitStream.ReadBits(num);
            Assert.That(value, Is.EqualTo(temp), $"{num} bits match failed");
            runningTotal += num;
        }
    }

    private static List<int> GetRandomNumbersSummingTo(int total)
    {
        var list = new List<int>();
        var sum = 0;
        while (sum < total)
        {
            var temp = Random.Shared.Next(1, total / 3);
            if (sum + temp < total) list.Add(temp);
            sum += temp;
        }

        if (list.Sum() < total) list.Add(total - list.Sum());
        return list;
    }
}