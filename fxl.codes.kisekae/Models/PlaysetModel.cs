using System.Linq;
using System.Text.Json.Serialization;
using fxl.codes.kisekae.Entities;

namespace fxl.codes.kisekae.Models;

public class PlaysetModel
{
    [JsonIgnore] public readonly string BorderColor;

    internal PlaysetModel(Configuration configuration)
    {
        BorderColor = configuration.BackgroundColorHex;

        Name = configuration.Name;
        Height = configuration.Height;
        Width = configuration.Width;
        var totalCels = configuration.Cels.Count;
        Cels = configuration.Cels
            .Select((x, index) => new CelModel(x, (totalCels - index) * 10))
            .OrderBy(x => x.ZIndex)
            .ToArray();

        for (var index = 0; index < 10; index++) Sets[index] = configuration.Cels.Any(x => x.Positions.Any(y => y.Set == index));
    }

    public CelModel[] Cels { get; set; }
    public int Height { get; set; }
    public string Name { get; set; }
    public bool[] Sets { get; set; } = new bool[10];
    public int Width { get; set; }
}