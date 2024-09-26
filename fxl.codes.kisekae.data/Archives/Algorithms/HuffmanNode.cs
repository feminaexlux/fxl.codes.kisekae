namespace fxl.codes.kisekae.data.Archives.Algorithms;

public class HuffmanNode
{
    public HuffmanNode(char value)
    {
    }

    public HuffmanNode(HuffmanNode left, HuffmanNode right)
    {
        Left = left;
        Left.Side = HuffmanNodeSide.Left;
        Left.Parent = this;

        Right = right;
        Right.Side = HuffmanNodeSide.Right;
        Right.Parent = this;

        Weight = left.Weight + right.Weight;
    }

    public char? Value { get; set; }
    public int Weight { get; set; } = 1;

    public HuffmanNode? Parent { get; set; }
    public HuffmanNode? Left { get; set; }
    public HuffmanNode? Right { get; set; }
    public HuffmanNodeSide? Side { get; set; }
}

public enum HuffmanNodeSide
{
    Left,
    Right
}