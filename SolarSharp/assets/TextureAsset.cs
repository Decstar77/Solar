using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    public enum TextureFormat
    {
        RGB,
        RGBA
    }

    public class TextureAsset : EngineAsset
    {
        public uint width;
        public uint height;
        public TextureFormat format;
        public List<char> pixels;
    }
}
