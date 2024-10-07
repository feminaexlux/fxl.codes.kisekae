namespace fxl.codes.kisekae.data.Archives.Algorithms;

public class Huffman
{
    private readonly List<HuffmanNode> Nodes;

    public Huffman()
    {
        Nodes = new List<HuffmanNode>();
        for (var i = char.MinValue; i < char.MaxValue; i++) Nodes.Add(new HuffmanNode(i, 1));
    }
}

internal class HuffmanNode
{
    public readonly char Value;
    public HuffmanNode Left;
    public HuffmanNode Parent;
    public HuffmanNode Right;
    public HuffmanSide Side;
    public int Weight;

    public HuffmanNode(char value, int weight = 0)
    {
        Value = value;
        Weight = weight;
    }

    public HuffmanNode(HuffmanNode left, HuffmanNode right)
    {
        Left = left;
        Left.Side = HuffmanSide.Left;
        Left.Parent = this;

        Right = right;
        Right.Side = HuffmanSide.Right;
        Right.Parent = this;

        Weight = left.Weight + right.Weight;
    }
}

internal enum HuffmanSide
{
    Left,
    Right
}