using System;
using System.Collections.Generic;
using fxl.codes.kisekae.data.Entities;

namespace fxl.codes.kisekae.Models
{
    public class CelModel
    {
        internal CelModel(CelConfig celConfig, int zIndex)
        {
            Mark = celConfig.Mark;
            Fix = celConfig.Fix;
            ZIndex = zIndex;
            Image = $"data:image/gif;base64,{Convert.ToBase64String(celConfig.Render.Image)}";
            Height = celConfig.Cel.Height;
            Width = celConfig.Cel.Width;
            Opacity = (double)(255 - celConfig.Transparency) / 255;
            Offset = new CoordinateModel(celConfig.Cel.OffsetX, celConfig.Cel.OffsetY);
            
            foreach (var position in celConfig.Positions) InitialPositions.Add(position.Set, new CoordinateModel(position.X, position.Y));
        }

        public int Fix { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public int Mark { get; set; }
        public double Opacity { get; set; }
        public int Width { get; set; }
        public int ZIndex { get; set; }
        public IDictionary<int, CoordinateModel> InitialPositions { get; set; } = new Dictionary<int, CoordinateModel>();
        public CoordinateModel Offset { get; set; }
    }
}