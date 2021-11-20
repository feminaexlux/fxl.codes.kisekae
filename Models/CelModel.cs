using System;
using fxl.codes.kisekae.Entities;

namespace fxl.codes.kisekae.Models
{
    public class CelModel
    {
        public readonly int Fix;
        public readonly string Image;
        public readonly int Mark;
        public readonly int ZIndex;

        internal CelModel(CelConfig celConfig, Render render, int zIndex)
        {
            Mark = celConfig.Mark;
            Fix = celConfig.Fix;
            ZIndex = zIndex;
            Image = Convert.ToBase64String(render.Image);
        }
    }
}