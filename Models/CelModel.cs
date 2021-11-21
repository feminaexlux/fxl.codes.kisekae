using System;
using fxl.codes.kisekae.Entities;

namespace fxl.codes.kisekae.Models
{
    public class CelModel
    {
        internal CelModel(CelConfig celConfig, int zIndex)
        {
            Mark = celConfig.Mark;
            Fix = celConfig.Fix;
            ZIndex = zIndex;
            Image = Convert.ToBase64String(celConfig.Render.Image);
            Height = celConfig.Cel.Height;
            Width = celConfig.Cel.Width;
            Opacity = (double)(255 - celConfig.Transparency) / 255;

            foreach (var position in celConfig.Positions) InitialPositions[position.Set] = new CoordinateModel(position.X, position.Y);

            Offset = new CoordinateModel(celConfig.Cel.OffsetX, celConfig.Cel.OffsetY);
        }

        public int Fix { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public int Mark { get; set; }
        public double Opacity { get; set; }
        public int Width { get; set; }
        public int ZIndex { get; set; }
        public CoordinateModel[] InitialPositions { get; set; } = new CoordinateModel[10];
        public CoordinateModel Offset { get; set; }
    }
}