using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    [Flags]
    public enum MaterialFlag
    {
        SHADOW = 0x1,
        TRANSPARENT = 0x2
    }

    public class MaterialAsset
    {
        public string name;
        public MaterialFlag Flags { get; set; }        
        public Guid ShaderId { get; set; }
        public Guid AlbedoTexture { get; set; }
        public Vector4 AlbedoColour { get; set; }
        public Guid SpecularTexture { get; set; }
        public Vector4 SpecularColour { get; set; }
        public Guid NormalTexture { get; set; }
    }
}
