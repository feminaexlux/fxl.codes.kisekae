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
        var list = new List<int>();
        var sum = 0;
        while (sum < 32)
        {
            var temp = Random.Shared.Next(1, 10);
            if (sum + temp < 32) list.Add(temp);
            sum += temp;
        }

        if (list.Sum() < 32) list.Add(32 - list.Sum());

        foreach (var num in list)
        {
            var value = bitStream.ReadBits(num);
            Assert.That(value, Is.EqualTo(Math.Pow(2, num) - 1), $"{num} bits match failed");
        }
    }

    [Test]
    public void TestReadLong()
    {
        var random = (ulong)Random.Shared.NextInt64();
        var bytes = BitConverter.GetBytes(random);
        var stream = new MemoryStream(bytes);

        var bitStream = new BitStream(stream);
        var list = new List<int>();
        var sum = 0;
        while (sum < 64)
        {
            var temp = Random.Shared.Next(1, 20);
            if (sum + temp < 64) list.Add(temp);
            sum += temp;
        }

        if (list.Sum() < 64) list.Add(64 - list.Sum());

        var runningTotal = 0;
        foreach (var num in list)
        {
            var temp = random;
            temp <<= runningTotal;
            temp >>= 64 - num;
            Console.WriteLine($"Original value: {random:b}, bit shifted number: {temp:b}");
            var value = bitStream.ReadBits(num);
            Assert.That(value, Is.EqualTo(temp), $"{num} bits match failed");
            runningTotal += num;
        }
    }
}